using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OpenApi;
using Prema.ShuleOne.Web.Server.Database;
using Prema.ShuleOne.Web.Server.Models;
using AutoMapper;
using Prema.ShuleOne.Web.Server.Caching.CacheServices;
using Prema.ShuleOne.Web.Server.Services;
using Microsoft.AspNetCore.Mvc;
using static Prema.ShuleOne.Web.Server.Services.MpesaRequestService;

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

        group.MapPost("/mpesa/callback", ( [FromBody] MpesaCallback callback, MpesaRequestService mpesaRequestService) =>
        {
            return mpesaRequestService.ProcessCallback(callback);
        })
        .Produces(200)
        .Produces(400);

    }

    public class PaymentDetails
    {
        public long mpesaNumber { get; set; }
        public int amount { get; set; }
    }
}
