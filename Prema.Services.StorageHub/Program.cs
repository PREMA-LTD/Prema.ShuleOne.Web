using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using MassTransit;
using Prema.Services.StorageHub.Services;
using Prema.Services.UnifiedNotifier.Consumers;
using StackExchange.Redis;
using System.Security.Cryptography.X509Certificates;
using static Google.Apis.Drive.v3.DriveService;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

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


var googleCredentialsPath = builder.Configuration["GoogleCredentialsPath"];

// Register Redis
builder.AddRedisClient(connectionName: "redis");

builder.Services.AddSingleton<DriveService>(sp =>
{
    var tokenResponse = new TokenResponse
    {
        AccessToken = "1//040GgdrnJ8uP7CgYIARAAGAQSNwF-L9IrmUT0nUWISKxVvTQfGgmtGaTG_66LkOS1DD_snBnymYlcc9IgdUIYSYpu6i5-uXSZ1KY",
        RefreshToken = "ya29.a0ARW5m74N_cXQGdU65o3AcRYbrvN1jyRQYP76hFR8bj9eICH3v3HigBDfhOc58IEe-SBH_mJiF-T5EHgnX4z3gmIt23pKl_PZ8aZ4LT9oZMMLREhae9lIDTry7U4BO7-auQthyBvcUWdiVj7zRt_2MM9CD2_wYKnPu19XyDAoaCgYKAfQSARMSFQHGX2MiWkkg9eIswApuEUoj8EG7jw0175",
    };


    var applicationName = "StorageHub"; // Use the name of the project in Google Cloud
    var username = "lifewayfiles@gmail.com"; // Use your email


    var apiCodeFlow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
    {
        ClientSecrets = new ClientSecrets
        {
            ClientId = "446582447081-kvn54ekfd6hlfntbr9a8ebig6nvglh0e.apps.googleusercontent.com",
            ClientSecret = "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"
        },
        Scopes = new[] { Scope.Drive },
        DataStore = new FileDataStore(applicationName)
    });


    var credential = new UserCredential(apiCodeFlow, username, tokenResponse);


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
