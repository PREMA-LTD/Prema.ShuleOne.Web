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
using Newtonsoft.Json;
using Adobe.PDFServicesSDK;
using Adobe.PDFServicesSDK.exception;
using Adobe.PDFServicesSDK.pdfjobs.parameters.documentmerge;
using Adobe.PDFServicesSDK.auth;
using Microsoft.AspNetCore.Http;
using Adobe.PDFServicesSDK.io;
using Adobe.PDFServicesSDK.pdfjobs.jobs;
using Adobe.PDFServicesSDK.pdfjobs.results;
using Newtonsoft.Json.Linq;
using Serilog;
using log4net.Config;
using log4net.Repository;
using log4net;
using System.Reflection;

namespace Prema.ShuleOne.Web.Server.Endpoints.Reports;

public static class ReportsEndpoint
{
    public static void MapReportsEndpoint(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Reports").WithTags("Reports");


        group.MapPost("/GenerateAdmissionLetter", async Task<Results<Ok<string>, NotFound, BadRequest<string>>> ([FromBody] AdmissionLetterDetails admissionLetterDetails, FileGeneratorService fileGeneratorService) =>
        {

            return await fileGeneratorService.GenerateFile(admissionLetterDetails);

        })
        .WithName("GenerateAdmissionLetter")
        .WithOpenApi();
    }
}

