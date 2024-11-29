using MassTransit;
using Microsoft.EntityFrameworkCore;
using Prema.ShuleOne.Web.Server.Caching.CacheServices;
using Prema.ShuleOne.Web.Server.BulkSms;
using Prema.ShuleOne.Web.Server.Database;
using Prema.ShuleOne.Web.Server.AppSettings;
using Prema.ShuleOne.Web.Server.BulkSms;
using Prema.ShuleOne.Web.Server.Endpoints;
using Prema.ShuleOne.Web.Server.Logging;
using Prema.ShuleOne.Web.Server.Telegram;
using Prema.ShuleOne.Web.Server.AutoMapper;
using Prema.ShuleOne.Web.Server.Controllers;
using Prema.ShuleOne.Web.Server.Services;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder
            .WithOrigins("https://localhost:4200", "https://fintrack.shangilia.africa") // Update this with your Angular app's URL
            .AllowAnyHeader()
            .AllowAnyMethod());
});


// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

string connectionString = builder.Configuration.GetConnectionString("MySqlConnectionString")!;
var serverVersion = new MySqlServerVersion(new Version(8, 0, 29));

builder.Services.AddDbContext<ShuleOneDatabaseContext>(
    dbContextOptions => dbContextOptions
        .UseMySql(connectionString, serverVersion,
            options => options.EnableRetryOnFailure()) // Enable transient error resiliency
        .LogTo(Console.WriteLine, LogLevel.Information)
        .EnableSensitiveDataLogging()
        .EnableDetailedErrors()
);


builder.Services.AddSingleton<IBulkSms, MobileSasa>();
builder.Services.Configure<MobileSasaSettings>(builder.Configuration.GetSection("MobileSasa"));
builder.Services.Configure<TelegramBotSettings>(builder.Configuration.GetSection("TelegramBot"));
builder.Services.AddSingleton<TelegramBot>();
builder.Services.AddSingleton<Logger>();
builder.Services.AddSingleton<MpesaRequestService>();

builder.Services.AddAutoMapper(typeof(MappingProfile));

//cache
builder.Services.AddSingleton<ILocationCacheService, LocationCacheService>();
builder.Services.AddHostedService<LocationCacheWorker>();


// Ensure IConfiguration is available
var rabbitMqConfig = builder.Configuration.GetSection("RabbitMQ");

builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        // Use configuration values for RabbitMQ connection
        cfg.Host(rabbitMqConfig["Host"], Convert.ToUInt16(rabbitMqConfig["Port"]), rabbitMqConfig["VirtualHost"], h =>
        {
            h.Username(rabbitMqConfig["Username"]);
            h.Password(rabbitMqConfig["Password"]);
        });
    });
});

var app = builder.Build();

app.MapDefaultEndpoints();

app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowSpecificOrigin");

app.MapFallbackToFile("/index.html");

app.MapStudentEndpoints();

app.MapLocationEndpoints();

app.MapFinanceEndpints();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
