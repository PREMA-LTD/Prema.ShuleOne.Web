﻿using System.Diagnostics;
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
        await InsertGrades(dbContext, cancellationToken);
        await InsertFeeTypes(dbContext, cancellationToken);


    }

    private static async Task InsertFeeTypes(ShuleOneDatabaseContext dbContext, CancellationToken cancellationToken)
    {

        // Extract distinct counties, subcounties, and wards
        var feeType = Enum.GetValues(typeof(FeeTypes))
            .Cast<FeeTypes>()
            .Select(ft => new FeeType
            {
                id = (int)ft, // Assuming Id is the primary key
                name = ft.ToString() // Assuming Name is a column for the grade name
            }).ToList();

        if (dbContext.FeeType.Count() == 0)
        {
            // Seed the data into the database
            await dbContext.FeeType.AddRangeAsync(feeType, cancellationToken);

            // Save changes
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    private static async Task InsertGrades(ShuleOneDatabaseContext dbContext, CancellationToken cancellationToken)
    {

        // Extract distinct counties, subcounties, and wards
        var grades = Enum.GetValues(typeof(Grades))
            .Cast<Grades>()
            .Select(g => new Grade
            {
                id = (int)g, // Assuming Id is the primary key
                name = g.ToString() // Assuming Name is a column for the grade name
            }).ToList();

        if (dbContext.Grade.Count() == 0)
        {
            // Seed the data into the database
            await dbContext.Grade.AddRangeAsync(grades, cancellationToken);

            // Save changes
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

    //private async Task SeedAsync(CatalogDbContext dbContext, CancellationToken cancellationToken)
    //{
    //    logger.LogInformation("Seeding database");

    //    static List<CatalogBrand> GetPreconfiguredCatalogBrands()
    //    {
    //        return [
    //            new() { Brand = "Azure" },
    //            new() { Brand = ".NET" },
    //            new() { Brand = "Visual Studio" },
    //            new() { Brand = "SQL Server" },
    //            new() { Brand = "Other" }
    //        ];
    //    }

    //    static List<CatalogType> GetPreconfiguredCatalogTypes()
    //    {
    //        return [
    //            new() { Type = "Mug" },
    //            new() { Type = "T-Shirt" },
    //            new() { Type = "Sheet" },
    //            new() { Type = "USB Memory Stick" }
    //        ];
    //    }

    //    static List<CatalogItem> GetPreconfiguredItems(DbSet<CatalogBrand> catalogBrands, DbSet<CatalogType> catalogTypes)
    //    {
    //        var dotNet = catalogBrands.First(b => b.Brand == ".NET");
    //        var other = catalogBrands.First(b => b.Brand == "Other");

    //        var mug = catalogTypes.First(c => c.Type == "Mug");
    //        var tshirt = catalogTypes.First(c => c.Type == "T-Shirt");
    //        var sheet = catalogTypes.First(c => c.Type == "Sheet");

    //        return [
    //            new() { CatalogType = tshirt, CatalogBrand = dotNet, AvailableStock = 100, Description = ".NET Bot Black Hoodie", Name = ".NET Bot Black Hoodie", Price = 19.5M, PictureFileName = "1.png" },
    //            new() { CatalogType = mug, CatalogBrand = dotNet, AvailableStock = 100, Description = ".NET Black & White Mug", Name = ".NET Black & White Mug", Price= 8.50M, PictureFileName = "2.png" },
    //            new() { CatalogType = tshirt, CatalogBrand = other, AvailableStock = 100, Description = "Prism White T-Shirt", Name = "Prism White T-Shirt", Price = 12, PictureFileName = "3.png" },
    //            new() { CatalogType = tshirt, CatalogBrand = dotNet, AvailableStock = 100, Description = ".NET Foundation T-shirt", Name = ".NET Foundation T-shirt", Price = 12, PictureFileName = "4.png" },
    //            new() { CatalogType = sheet, CatalogBrand = other, AvailableStock = 100, Description = "Roslyn Red Sheet", Name = "Roslyn Red Sheet", Price = 8.5M, PictureFileName = "5.png" },
    //            new() { CatalogType = tshirt, CatalogBrand = dotNet, AvailableStock = 100, Description = ".NET Blue Hoodie", Name = ".NET Blue Hoodie", Price = 12, PictureFileName = "6.png" },
    //            new() { CatalogType = tshirt, CatalogBrand = other, AvailableStock = 100, Description = "Roslyn Red T-Shirt", Name = "Roslyn Red T-Shirt", Price = 12, PictureFileName = "7.png" },
    //            new() { CatalogType = tshirt, CatalogBrand = other, AvailableStock = 100, Description = "Kudu Purple Hoodie", Name = "Kudu Purple Hoodie", Price = 8.5M, PictureFileName = "8.png" },
    //            new() { CatalogType = mug, CatalogBrand = other, AvailableStock = 100, Description = "Cup<T> White Mug", Name = "Cup<T> White Mug", Price = 12, PictureFileName = "9.png" },
    //            new() { CatalogType = sheet, CatalogBrand = dotNet, AvailableStock = 100, Description = ".NET Foundation Sheet", Name = ".NET Foundation Sheet", Price = 12, PictureFileName = "10.png" },
    //            new() { CatalogType = sheet, CatalogBrand = dotNet, AvailableStock = 100, Description = "Cup<T> Sheet", Name = "Cup<T> Sheet", Price = 8.5M, PictureFileName = "11.png" },
    //            new() { CatalogType = tshirt, CatalogBrand = other, AvailableStock = 100, Description = "Prism White TShirt", Name = "Prism White TShirt", Price = 12, PictureFileName = "12.png" }
    //        ];
    //    }

    //    if (!dbContext.CatalogBrands.Any())
    //    {
    //        var brands = GetPreconfiguredCatalogBrands();
    //        await dbContext.CatalogBrands.AddRangeAsync(brands, cancellationToken);

    //        logger.LogInformation("Seeding {CatalogBrandCount} catalog brands", brands.Count);

    //        await dbContext.SaveChangesAsync(cancellationToken);
    //    }

    //    if (!dbContext.CatalogTypes.Any())
    //    {
    //        var types = GetPreconfiguredCatalogTypes();
    //        await dbContext.CatalogTypes.AddRangeAsync(types, cancellationToken);

    //        logger.LogInformation("Seeding {CatalogTypeCount} catalog item types", types.Count);

    //        await dbContext.SaveChangesAsync(cancellationToken);
    //    }

    //    if (!dbContext.CatalogItems.Any())
    //    {
    //        var items = GetPreconfiguredItems(dbContext.CatalogBrands, dbContext.CatalogTypes);
    //        await dbContext.CatalogItems.AddRangeAsync(items, cancellationToken);

    //        logger.LogInformation("Seeding {CatalogItemCount} catalog items", items.Count);

    //        await dbContext.SaveChangesAsync(cancellationToken);
    //    }
    //}
}
