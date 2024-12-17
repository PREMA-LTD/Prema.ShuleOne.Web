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

namespace Prema.ShuleOne.Web.Server.Services
{
    public class FileGeneratorService(ILogger<FileGeneratorService> logger, IPublishEndpoint publishEndpoint)
    {
        public async Task<Results<Ok<string>, NotFound, BadRequest<string>>> GenerateFile(AdmissionLetterDetails admissionLetterDetails)
        {
            try
            {
                // Initial setup, create credentials instance
                ICredentials credentials = new ServicePrincipalCredentials("fbf649504ff042a282e3f31ae7d08859", "p8e-0uWYwgWny5mKqgHWqreKNzpgko5_C4pC");

                // Creates a PDF Services instance
                PDFServices pdfServices = new PDFServices(credentials);

                // Creates an asset from source file and upload
                //using Stream inputStream = File.OpenRead(@"documentMergeTemplate.docx");
                string basePath = Path.Combine(Directory.GetCurrentDirectory(), "Endpoints", "Reports");
                using Stream inputStream = File.OpenRead(Path.Combine(basePath, "Templates", "LifewayAdmissionLetterTemplate.docx"));
                IAsset asset = pdfServices.Upload(inputStream, PDFServicesMediaType.DOCX.GetMIMETypeValue());

                // Setup input data for the document merge process
                JObject jsonDataForMerge = JObject.FromObject(admissionLetterDetails);

                // Create parameters for the job
                DocumentMergeParams documentMergeParams = DocumentMergeParams.DocumentMergeParamsBuilder()
                    .WithJsonDataForMerge(jsonDataForMerge)
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
                string formattedDateTime = DateTime.UtcNow.ToString("ddMMyyHHmmss");
                string fileName = $"{admissionLetterDetails.AdmissionNumber} - {admissionLetterDetails.StudentOtherNames} {admissionLetterDetails.StudentFirstName}_AdmissionLetter{formattedDateTime}.pdf";
                string outputFilePath = $"{basePath}/GeneratedReports/AdmissionLeters/{fileName}";

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
                await publishEndpoint.Publish(message);

                return TypedResults.Ok<string>("Generated successfully.");
            }
            catch (ServiceUsageException ex)
            {
                logger.LogError("Exception encountered while executing operation", ex);
                return TypedResults.BadRequest<string>("ServiceUsageException: " + ex.Message);
            }
            catch (ServiceApiException ex)
            {
                logger.LogError("Exception encountered while executing operation", ex);
                return TypedResults.BadRequest<string>("ServiceApiException: " + ex.Message);
            }
            catch (SDKException ex)
            {
                logger.LogError("Exception encountered while executing operation", ex);
                return TypedResults.BadRequest<string>("SDKException: " + ex.Message);
            }
            catch (IOException ex)
            {
                logger.LogError("Exception encountered while executing operation", ex);
                return TypedResults.BadRequest<string>("IOException: " + ex.Message);
            }
            catch (Exception ex)
            {
                logger.LogError("Exception encountered while executing operation", ex);
                return TypedResults.BadRequest<string>("Exception: " + ex.Message);
            }
        }
    }
}
