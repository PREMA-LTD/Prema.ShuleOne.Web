using System.Diagnostics;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using Prema.ShuleOne.Web.Server.Models.Location;
using Prema.ShuleOne.Web.Server.Database;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Prema.ShuleOne.Web.Server.Models;

namespace Prema.ShuleOne.Web.Server.Caching.CacheServices
{
    public enum SingletonStatus
    {
        Initializing,
        ErrorInitializingRetrying,
        ErrorInitializingFailed,
        Ready,
    }

    public interface ILocationCacheService
    {
        SingletonStatus Status { get; set; }
        bool IsLoaded { get; }
        Task LoadCache(bool forceall = false);
        //Task ReloadCache(bool forceall = false);
        Result<List<CountyDto>> GetCounties();
        Result<List<SubcountyDto>> GetSubcounties(int countyId);
        Result<List<WardDto>> GetWards(int subcountyId);
        Result<LocationData> GetLocation(int wardId);
    }

    public class LocationCacheService : ILocationCacheService
    {
        private readonly ILogger<LocationCacheService> _logger;
        private readonly IServiceProvider _serviceProvider;
        public Dictionary<int, County> _county;
        public Dictionary<int, Subcounty> _subcounty;
        public Dictionary<int, Ward> _ward;
        public LocationData _location;
        private DateTime _lastReloadDateTime;
        private readonly DateTime _minReloadDateTime = new DateTime(2000, 1, 1);
        private readonly IMapper _mapper;

        public LocationCacheService(ILogger<LocationCacheService> logger, IServiceProvider serviceProvider, IMapper mapper)
        {
            Status = SingletonStatus.Initializing;
            //_timezoneOffsetSettingRepository = timezoneOffsetSettingRepository;
            _logger = logger;
            _serviceProvider = serviceProvider;
            _county = new Dictionary<int, County>();
            _subcounty = new Dictionary<int, Subcounty>();
            _ward = new Dictionary<int, Ward>();
            _location = new LocationData();
            _mapper = mapper;
        }

        public SingletonStatus Status { get; set; }
        public bool IsLoaded => _lastReloadDateTime > _minReloadDateTime;

        public async Task LoadCache(bool forceall = false)
        {
            var reloadDateTime = DateTime.UtcNow;
            try
            {
                if (forceall)
                {
                    _lastReloadDateTime = _minReloadDateTime;
                }

                Dictionary<int, County> counties;
                Dictionary<int, Subcounty> subcounties;
                Dictionary<int, Ward> wards;

                using (var scope = _serviceProvider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<ShuleOneDatabaseContext>();
                    counties = (await dbContext.County
                                .AsNoTracking()
                                .ToDictionaryAsync(county => county.id, county => county));
                    subcounties = (await dbContext.Subcounty
                                .AsNoTracking()
                                .ToDictionaryAsync(subcounty => subcounty.id, subcounty => subcounty));
                    wards = (await dbContext.Ward
                                .AsNoTracking()
                                .ToDictionaryAsync(ward => ward.id, ward => ward));
                }

                if (_county.Count == 0 || forceall)
                {
                    _county = counties;
                    _logger.LogInformation("Counties cache loaded. {}", DateTime.UtcNow);
                }

                if (_subcounty.Count == 0 || forceall)
                {
                    _subcounty = subcounties;
                    _logger.LogInformation("Subcounties cache loaded. {}", DateTime.UtcNow);
                }

                if (_ward.Count == 0 || forceall)
                {
                    _ward = wards;
                    _logger.LogInformation("Ward cache loaded. {}", DateTime.UtcNow);
                }

                _lastReloadDateTime = reloadDateTime;
                Status = SingletonStatus.Ready;
            }
            catch (Exception e)
            {
                Status = SingletonStatus.ErrorInitializingRetrying;

                _logger.LogError(e, "Error Loading Location Cache");

                throw;
            }
        }

        #region CacheReload
        //public async Task ReloadCache(bool forceall = false)
        //{
        //    var s = _serviceProvider.CreateScope();
        //    var svc = _serviceProvider.CreateScope().ServiceProvider.GetRequiredService<GpsTimeOffsetSettingRepository>();

        //    using var taskActivity = AppTracing.ActivitySource.StartActivity($"{nameof(GpsTimeOffsetSettingService)}.{nameof(ReloadCache)}");
        //    var reloadDateTime = DateTime.UtcNow;
        //    try
        //    {
        //        if (forceall)
        //        {
        //            _lastReloadDateTime = _minReloadDateTime;
        //        }


        //        taskActivity?.AddEvent(new("Timezone LoadingFromDB"));

        //        Dictionary<long, GpsTimeOffsetSetting> newOrUpdatedTimezoneOffsetSettings;
        //        using (var scope = _serviceProvider.CreateScope())
        //        {
        //            var timezoneOffsetSettingRepository = scope.ServiceProvider.GetRequiredService<GpsTimeOffsetSettingRepository>();
        //            newOrUpdatedTimezoneOffsetSettings = (await timezoneOffsetSettingRepository.GetTimezoneOffsetSettings(_lastReloadDateTime))
        //                .ToDictionary(i => i.imei, timezoneOffsetSetting => timezoneOffsetSetting);
        //        }

        //        if (_cachedDeviceTimezoneOffsetSetting.Count == 0 || forceall)
        //        {
        //            _cachedDeviceTimezoneOffsetSetting = newOrUpdatedTimezoneOffsetSettings;
        //            taskActivity?.AddEvent(new("Timezone Cache Reloaded"));
        //        }
        //        else
        //        {
        //            foreach (var newOrUpdatedTimezoneOffsetSetting in newOrUpdatedTimezoneOffsetSettings)
        //            {
        //                _cachedDeviceTimezoneOffsetSetting[newOrUpdatedTimezoneOffsetSetting.Key] = newOrUpdatedTimezoneOffsetSetting.Value;
        //            }
        //            taskActivity?.AddEvent(new("Timezone Cache Updated"));
        //        }
        //        _lastReloadDateTime = reloadDateTime;
        //        Status = SingletonStatus.Ready;
        //        taskActivity?.SetStatus(ActivityStatusCode.Ok, "Timezone Complete");
        //    }
        //    catch (Exception e)
        //    {
        //        if (_appSettings.CacheUnavailableAction == CacheUnavailableAction.DontAlterTime)
        //        {
        //            // Consider Ready even though we have had an exception
        //            Status = SingletonStatus.Ready;
        //            _lastReloadDateTime = reloadDateTime;
        //        }
        //        else
        //        {
        //            Status = SingletonStatus.ErrorInitializingRetrying;
        //        }

        //        _logger.LogError(e, "Error Loading Device Information");
        //        taskActivity?.SetStatus(ActivityStatusCode.Error, e.Message);
        //        throw;
        //    }
        //}
        #endregion

        public Result<List<CountyDto>> GetCounties()
        {
            if (!_county.Any())
            {
                return Result.Failure<List<CountyDto>>($"Counties Not Found");
            }
            
            return _county == null
                ? Result.Failure<List<CountyDto>>($"Missing couties")
                : Result.Success<List<CountyDto>>(_county.Select(c => _mapper.Map<CountyDto>(c.Value)).ToList());
        }

        public Result<List<SubcountyDto>> GetSubcounties(int countyId)
        {
            if (!_subcounty.Any())
            {
                return Result.Failure<List<SubcountyDto>>($"Subcounties Not Found");
            }

            return _subcounty == null
                ? Result.Failure<List<SubcountyDto>>($"Missing subcouties")
                : Result.Success<List<SubcountyDto>>(_subcounty.Where(sc => sc.Value.fk_county_id == countyId).Select(sc => _mapper.Map<SubcountyDto>(sc.Value)).ToList());
        }

        public Result<List<WardDto>> GetWards(int subcountyId)
        {
            if (!_ward.Any())
            {
                return Result.Failure<List<WardDto>>($"Wards Not Found");
            }

            return _ward == null
                ? Result.Failure<List<WardDto>>($"Missing wards")
                : Result.Success<List<WardDto>>(_ward.Where(sc => sc.Value.fk_subcounty_id == subcountyId).Select(sc => _mapper.Map<WardDto>(sc.Value)).ToList());
        }

        public Result<LocationData> GetLocation(int wardId)
        {
            if (!_subcounty.Any() || !_county.Any())
            {
                return Result.Failure<LocationData>($"Location Not Found");
            }

            return _location == null
                ? Result.Failure<LocationData>("Missing location")
                : Result.Success<LocationData>(new LocationData()
                {
                    subcountyId = _ward.Where(w => w.Key == wardId).Select(w => w.Value.fk_subcounty_id).First(),
                    countyId = _subcounty.Where(sc => sc.Key == _ward.Where(w => w.Key == wardId).Select(w => w.Value.fk_subcounty_id).First()).Select(sc => sc.Value.fk_county_id).First()
                });

        }
    }

    public class LocationData
    {
        public int countyId { get; set; }
        public int subcountyId { get; set; }
    }
}