using Microsoft.EntityFrameworkCore;
using Prema.ShuleOne.Web.Server.Database;
using Prema.Services.ShuleOneDbManager;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
string connectionString = builder.Configuration.GetConnectionString("MySqlConnectionString")!;
var serverVersion = new MySqlServerVersion(new Version(8, 0, 29));

builder.Services.AddDbContext<ShuleOneDatabaseContext>(dbContextOptions =>
    dbContextOptions
        .UseMySql(
            connectionString,
            serverVersion,
            mySqlOptions => mySqlOptions
                .MigrationsAssembly(typeof(Program).Assembly.GetName().Name) // Specify the migrations assembly
                .EnableRetryOnFailure() // Enable transient error resiliency
        )
        .LogTo(Console.WriteLine, LogLevel.Information) // Log EF Core operations to the console
        .EnableSensitiveDataLogging() // Enable detailed logging (use only in development)
        .EnableDetailedErrors() // Provide detailed error information
);

builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing.AddSource(ShuleOneDbInitializer.ActivitySourceName));

builder.Services.AddSingleton<ShuleOneDbInitializer>();
builder.Services.AddHostedService(sp => sp.GetRequiredService<ShuleOneDbInitializer>());
builder.Services.AddHealthChecks()
    .AddCheck<ShuleOneDbInitializerHealthCheck>("DbInitializer", null);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapPost("/reset-db", async (ShuleOneDatabaseContext dbContext, ShuleOneDbInitializer dbInitializer, CancellationToken cancellationToken) =>
    {
        // Delete and recreate the database. This is useful for development scenarios to reset the database to its initial state.
        await dbContext.Database.EnsureDeletedAsync(cancellationToken);
        await dbInitializer.InitializeDatabaseAsync(dbContext, cancellationToken);
    });
}

app.MapDefaultEndpoints();

await app.RunAsync();
