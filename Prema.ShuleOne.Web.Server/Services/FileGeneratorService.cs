using Adobe.PDFServicesSDK.auth;
using Adobe.PDFServicesSDK.exception;
using Adobe.PDFServicesSDK.io;
using Adobe.PDFServicesSDK.pdfjobs.jobs;
using Adobe.PDFServicesSDK.pdfjobs.parameters.documentmerge;
using Adobe.PDFServicesSDK.pdfjobs.results;
using Adobe.PDFServicesSDK;
using Newtonsoft.Json.Linq;
using Prema.ShuleOne.Web.Server.Endpoints.Reports;
using Microsoft.AspNetCore.Http.HttpResults;
using MassTransit;
using MassTransit.Transports;
using Prema.Services.StorageHub.Contracts;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.Options;
using Prema.ShuleOne.Web.Server.AppSettings;

namespace Prema.ShuleOne.Web.Server.Services
{
    public class FileGeneratorService(ILogger<FileGeneratorService> logger, IPublishEndpoint publishEndpoint, IOptionsMonitor<ReportSettings> reportSettings)
    {
        public async Task<Results<Ok<string>, NotFound, BadRequest<string>>> GenerateFile(JObject reportDetails, string fileName, string outputFilePath, string templateFileName)
        {
            try
            {
                // Initial setup, create credentials instance
                ICredentials credentials = new ServicePrincipalCredentials("fbf649504ff042a282e3f31ae7d08859", "p8e-0uWYwgWny5mKqgHWqreKNzpgko5_C4pC");

                // Creates a PDF Services instance
                PDFServices pdfServices = new PDFServices(credentials);

                // Creates an asset from source file and upload
                //using Stream inputStream = File.OpenRead(@"documentMergeTemplate.docx");
                string storageBasePath = reportSettings.CurrentValue.FileStoragePath;
                using Stream inputStream = File.OpenRead(Path.Combine(reportSettings.CurrentValue.ReportTemplatePath, templateFileName));
                IAsset asset = pdfServices.Upload(inputStream, PDFServicesMediaType.DOCX.GetMIMETypeValue());

                // Create parameters for the job
                DocumentMergeParams documentMergeParams = DocumentMergeParams.DocumentMergeParamsBuilder()
                    .WithJsonDataForMerge(reportDetails)
                    .WithOutputFormat(OutputFormat.PDF)
                    .Build();

                // Creates a new job instance
                DocumentMergeJob documentMergeJob = new DocumentMergeJob(asset, documentMergeParams);

                // Submits the job and gets the job result
                string location = pdfServices.Submit(documentMergeJob);
                PDFServicesResponse<DocumentMergeResult> pdfServicesResponse =
                    pdfServices.GetJobResult<DocumentMergeResult>(location, typeof(DocumentMergeResult));

                // Get content from the resulting asset(s)
                IAsset resultAsset = pdfServicesResponse.Result.Asset;
                StreamAsset streamAsset = pdfServices.GetContent(resultAsset);

                // Creating output streams and copying stream asset's content to it   
                logger.LogInformation($"storageBasePath: {storageBasePath}");  
                logger.LogInformation($"outputFilePath: {outputFilePath}");
                //outputFilePath = $"{storageBasePath}{outputFilePath}";
                outputFilePath = Path.Combine(storageBasePath, outputFilePath.TrimStart(Path.DirectorySeparatorChar));

                logger.LogInformation($"combined outputFilePath: {outputFilePath}");

                // Ensure directory exists
                new FileInfo(outputFilePath).Directory.Create();

                // Reset stream to beginning before any operations
                streamAsset.Stream.Position = 0;

                // Save to file
                using (Stream outputStream = File.OpenWrite(outputFilePath))
                {
                    streamAsset.Stream.CopyTo(outputStream);
                }

                // Reset stream again before creating byte array
                streamAsset.Stream.Position = 0;

                // Convert the StreamAsset's Stream to a byte array
                byte[] fileBytes = new byte[streamAsset.Stream.Length];
                streamAsset.Stream.Read(fileBytes, 0, fileBytes.Length);

                var message = new FileUpload
                {
                    fileName = fileName,
                    fileContent = fileBytes,
                    key = fileName
                };
                //await publishEndpoint.Publish(message);

                return TypedResults.Ok<string>("Generated successfully.");
            }
            catch (ServiceUsageException ex)
            {
                logger.LogError("ServiceUsageException encountered while executing operation: "+ ex.Message);
                return TypedResults.BadRequest<string>("ServiceUsageException: " + ex.Message);
            }
            catch (ServiceApiException ex)
            {
                logger.LogError("ServiceApiException encountered while executing operation: " + ex.Message);
                return TypedResults.BadRequest<string>("ServiceApiException: " + ex.Message);
            }
            catch (SDKException ex)
            {
                logger.LogError("SDKException encountered while executing operation: " + ex.Message);
                return TypedResults.BadRequest<string>("SDKException: " + ex.Message);
            }
            catch (IOException ex)
            {
                logger.LogError("IOException encountered while executing operation: " + ex.Message);
                return TypedResults.BadRequest<string>("IOException: " + ex.Message);
            }
            catch (Exception ex)
            {
                logger.LogError("Exception encountered while executing operation: " + ex.Message);
                return TypedResults.BadRequest<string>("Exception: " + ex.Message);
            }
        }
    }
}
