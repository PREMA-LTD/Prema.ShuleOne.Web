using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using MassTransit;
using Microsoft.Extensions.Options;
using Prema.Services.StorageHub.AppSettings;
using Prema.Services.StorageHub.Services;
using Prema.Services.StorageHub.Workers;
using Prema.Services.UnifiedNotifier.Consumers;
using StackExchange.Redis;
using System.Security.Cryptography.X509Certificates;
using static Google.Apis.Drive.v3.DriveService;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.Configure<GoogleDriveApiSettings>(builder.Configuration.GetSection("GoogleDriveApiSettings"));

builder.Services.AddSingleton<FileStorageService>();

builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration.GetConnectionString("rabbitmq"));

        cfg.ReceiveEndpoint("file-created", e =>
        {
            e.Consumer<FileUploaderConsumer>(context);
        });
    });
});

builder.Services.AddSingleton<FileUploaderConsumer>();

// Register Redis
builder.AddRedisClient(connectionName: "redis");

builder.Services.AddSingleton<TokenStore>();
builder.Services.AddHostedService<GoogleAuthTokenRefreshService>();

builder.Services.AddSingleton<DriveService>(sp =>
{
    var googleDriveApiSettings = sp.GetRequiredService<IOptions<GoogleDriveApiSettings>>().Value;
    var tokenStore = sp.GetRequiredService<TokenStore>();
    tokenStore.SetRefreshToken(googleDriveApiSettings.RefreshToken);

    var applicationName = googleDriveApiSettings.ApplicationName; // Use the name of the project in Google Cloud
    var username = googleDriveApiSettings.Username; // Use your email

    var apiCodeFlow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
    {
        ClientSecrets = new ClientSecrets
        {
            ClientId = googleDriveApiSettings.ClientId,
            ClientSecret = googleDriveApiSettings.ClientSecret
        },
        Scopes = new[] { Scope.Drive },
        DataStore = new FileDataStore(applicationName)
    });

    // Use a factory pattern to retrieve tokens dynamically
    var credential = new UserCredential(apiCodeFlow, username, new TokenResponse
    {
        AccessToken = tokenStore.GetAccessToken(),
        RefreshToken = tokenStore.GetRefreshToken(),
    });

    var service = new DriveService(new BaseClientService.Initializer
    {
        HttpClientInitializer = credential,
        ApplicationName = applicationName
    });

    return service;
});

// Register FileStorageService
builder.Services.AddTransient<FileStorageService>();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
