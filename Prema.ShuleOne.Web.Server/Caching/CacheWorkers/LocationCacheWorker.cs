using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Prema.ShuleOne.Web.Server.Caching;
using Prema.ShuleOne.Web.Server.Caching.CacheServices;
using Prema.ShuleOne.Web.Server.Database;
using Prema.ShuleOne.Web.Server.Models.Location;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

public class LocationCacheWorker : BackgroundService
{
    private readonly ILogger<LocationCacheWorker> _logger;
    private readonly ILocationCacheService _locationCacheService;

    public LocationCacheWorker(ILogger<LocationCacheWorker> logger, ILocationCacheService locationCacheService)
    {
        _logger = logger;
        _locationCacheService = locationCacheService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            _logger.LogInformation("Started loading location cache running at: {time}", DateTimeOffset.Now);
            await _locationCacheService.LoadCache();
            _logger.LogInformation("Finished loading location cache running at: {time}", DateTimeOffset.Now);
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Location Cache Error Occured Executing {nameof(LocationCacheWorker)}");
        }

    }


}
