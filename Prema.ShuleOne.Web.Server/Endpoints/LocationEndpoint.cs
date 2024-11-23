using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OpenApi;
using Prema.ShuleOne.Web.Server.Database;
using Prema.ShuleOne.Web.Server.Models;
using AutoMapper;
using Prema.ShuleOne.Web.Server.Caching.CacheServices;
namespace Prema.ShuleOne.Web.Server.Controllers;

public static class LocationEndpoints
{
    public static void MapLocationEndpoints (this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Location").WithTags("Location");

        group.MapGet("/County", async(ILocationCacheService locationCacheService, IMapper mapper) =>
        {
            var counties = locationCacheService.GetCounties();

            if (counties.IsSuccess)
            {
                return Results.Ok(counties.Value);
            }            

            return Results.NotFound();
        })
        .WithName("GetAllCounties")
        .WithOpenApi();

        group.MapGet("/Subcounty/{countyId}", async (ILocationCacheService locationCacheService, IMapper mapper, int countyId) =>
        {
            var subcounties = locationCacheService.GetSubcounties(countyId);

            if (subcounties.IsSuccess)
            {
                return Results.Ok(subcounties.Value);
            }

            return Results.NotFound();
        })
        .WithName("GetCountySubcounties")
        .WithOpenApi();

        group.MapGet("/Ward/{subcountyId}", async (ILocationCacheService locationCacheService, IMapper mapper, int subcountyId) =>
        {
            var counties = locationCacheService.GetWards(subcountyId);

            if (counties.IsSuccess)
            {
                return Results.Ok(counties.Value);
            }

            return Results.NotFound();
        })
        .WithName("GetSubcoutyWards")
        .WithOpenApi();


        group.MapGet("/{wardId}", async (ILocationCacheService locationCacheService, IMapper mapper, int wardId) =>
        {
            var counties = locationCacheService.GetLocation(wardId);

            if (counties.IsSuccess)
            {
                return Results.Ok(counties.Value);
            }

            return Results.NotFound();
        })
        .WithName("GetLocation")
        .WithOpenApi();
    }
}
