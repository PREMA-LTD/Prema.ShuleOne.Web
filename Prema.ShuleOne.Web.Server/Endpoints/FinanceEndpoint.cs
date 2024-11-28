using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OpenApi;
using Prema.ShuleOne.Web.Server.Database;
using Prema.ShuleOne.Web.Server.Models;
using AutoMapper;
using Prema.ShuleOne.Web.Server.Caching.CacheServices;
using Prema.ShuleOne.Web.Server.Services;
namespace Prema.ShuleOne.Web.Server.Controllers;

public static class FinanceEndpint
{
    public static void MapFinanceEndpints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Finance").WithTags("Finance");

        group.MapPost("/InitiateMpesaPaymentPrompt", async (PaymentDetails paymentDetails, ShuleOneDatabaseContext db, MpesaRequestService mpesaRequestService) =>
        {
            await mpesaRequestService.SendMpesaRequestAsync(paymentDetails.mpesaNumber, paymentDetails.amount);
            return TypedResults.Created($"/api/Finance/{paymentDetails}", paymentDetails);
        })
        .WithName("InitiateMpesaPaymentPrompt")
        .WithOpenApi();


  
    }

    public class PaymentDetails
    {
        public long mpesaNumber { get; set; }
        public int amount { get; set; }
    }
}
