using System.Diagnostics;
using MassTransit;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using Prema.ShuleOne.Web.Server.Database;
using Prema.ShuleOne.Web.Server.Database.LocationData;
using Prema.ShuleOne.Web.Server.Models;


namespace Prema.Services.ShuleOneDbManager;

internal class ShuleOneDbInitializer(IServiceProvider serviceProvider, ILogger<ShuleOneDbInitializer> logger)
    : BackgroundService
{
    public const string ActivitySourceName = "Migrations";

    private readonly ActivitySource _activitySource = new(ActivitySourceName);

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ShuleOneDatabaseContext>();

        using var activity = _activitySource.StartActivity("Initializing catalog database", ActivityKind.Client);
        await InitializeDatabaseAsync(dbContext, cancellationToken);
    }

    public async Task InitializeDatabaseAsync(ShuleOneDatabaseContext dbContext, CancellationToken cancellationToken = default)
    {
        var sw = Stopwatch.StartNew();

        var strategy = dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(dbContext.Database.MigrateAsync, cancellationToken);

        await SeedAsync(dbContext, cancellationToken);

        logger.LogInformation("Database initialization completed after {ElapsedMilliseconds}ms", sw.ElapsedMilliseconds);
    }

    public static async Task SeedAsync(ShuleOneDatabaseContext dbContext, CancellationToken cancellationToken)
    {
        // Ensure the database is created
        await dbContext.Database.EnsureCreatedAsync(cancellationToken);

        await InsertLocationData(dbContext, cancellationToken);

        //insert enums

        //insert grades
        await InsertEnumValues<Grades, Grade>(
            dbContext,
            dbContext.Grade,
            ft => new Grade { id = (int)ft, name = ft.ToString() },
            cancellationToken);

        //insert fee types
        await InsertEnumValues<FeeTypes, FeeType>(
            dbContext,
            dbContext.FeeType,
            ft => new FeeType { id = (int)ft, name = ft.ToString() },
            cancellationToken);

        //insert account types
        await InsertEnumValues<AccountType, AccountTypes>(
            dbContext,
            dbContext.AccountTypes,
            ft => new AccountTypes { id = (int)ft, name = ft.ToString() },
            cancellationToken);

        //insert transaction types
        await InsertEnumValues<TransactionType, TransactionTypes>(
            dbContext,
            dbContext.TransactionTypes,
            ft => new TransactionTypes { id = (int)ft, name = ft.ToString() },
            cancellationToken);

        //insert account types
        //await InsertEnumValues<InvoiceStatus, InvoiceStatuses>(
        //    dbContext,
        //    dbContext.InvoiceStatuses,
        //    ft => new InvoiceStatuses { id = (int)ft, name = ft.ToString() },
        //    cancellationToken);

        //insert account types
        await InsertEnumValues<PaymentMethod, PaymentMethods>(
            dbContext,
            dbContext.PaymentMethods,
            ft => new PaymentMethods { id = (int)ft, name = ft.ToString() },
            cancellationToken);

        //insert receipt itemm types
        await InsertEnumValues<ReceiptItemType, ReceiptItemTypes>(
            dbContext,
            dbContext.ReceiptItemTypes,
            ft => new ReceiptItemTypes { id = (int)ft, name = ft.ToString() },
            cancellationToken);

        //insert receipt itemm types
        await InsertEnumValues<FileLocationType, FileLocationTypes>(
            dbContext,
            dbContext.FileLocationTypes,
            ft => new FileLocationTypes { id = (int)ft, name = ft.ToString() },
            cancellationToken);
    }

    private static async Task InsertEnumValues<TEnum, TEntity>(
        ShuleOneDatabaseContext dbContext,
        DbSet<TEntity> dbSet,
        Func<TEnum, TEntity> entitySelector,
        CancellationToken cancellationToken)
            where TEnum : Enum
            where TEntity : class
    {
        // Check if there are any records without tracking
        if (!await dbSet.AnyAsync(cancellationToken))
        {
            var entities = Enum.GetValues(typeof(TEnum))
                .Cast<TEnum>()
                .Select(entitySelector)
                .ToList();

            await dbSet.AddRangeAsync(entities, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    private static async Task InsertLocationData(ShuleOneDatabaseContext dbContext, CancellationToken cancellationToken)
    {
        // Define the CSV file URL
        var csvFilePath = "https://raw.githubusercontent.com/enockkim/location-data-files/refs/heads/main/kenya-location-data.csv";

        // Download CSV content
        string csvContent;
        using (var httpClient = new HttpClient())
        {
            csvContent = await httpClient.GetStringAsync(csvFilePath);
        }

        // Parse CSV data
        var records = LoadLocationData.LoadCsvDataFromContent(csvContent);

        // Extract distinct counties, subcounties, and wards
        var counties = records.Select(r => r.Item1).DistinctBy(c => c.id).ToList();
        var subcounties = records.Select(r => r.Item2).DistinctBy(c => c.id).ToList();
        var wards = records.Select(r => r.Item3).DistinctBy(w => w.id).ToList();

        if (dbContext.County.Count() == 0)
        {
            // Seed the data into the database
            await dbContext.County.AddRangeAsync(counties, cancellationToken);
            await dbContext.Subcounty.AddRangeAsync(subcounties, cancellationToken);
            await dbContext.Ward.AddRangeAsync(wards, cancellationToken);

            // Save changes
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
