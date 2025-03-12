using StackExchange.Redis;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Upload;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Mail;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Prema.Services.StorageHub.AppSettings;
using Microsoft.Extensions.Options;


namespace Prema.Services.StorageHub.Services;

public class GoogleFileStorageService
{
    private readonly IConnectionMultiplexer _redis;
    private readonly DriveService _googleDriveService;
    private readonly GoogleDriveApiSettings _googleDriveApiSettings;

    public GoogleFileStorageService(IConnectionMultiplexer redis, DriveService googleDriveService, IOptionsMonitor<GoogleDriveApiSettings> googleDriveApiSettings)
    {
        _redis = redis;
        _googleDriveService = googleDriveService;
        _googleDriveApiSettings = googleDriveApiSettings.CurrentValue;
    }

    public async Task SaveFileAsync(string key, byte[] fileContent, string fileName)
    {
        // Save to Redis
        IDatabase db = _redis.GetDatabase();
        await db.StringSetAsync(key, fileContent, TimeSpan.FromHours(1)); // Cache for 1 hour
    
        // Save to Google Drive
        await UploadPdfToGoogleDrive(fileContent, fileName);   
    
    }

    public async Task UploadPdfToGoogleDrive(byte[] pdfBytes, string fileName, string fileType = "admission_letters")
    {
        // Validate inputs
        if (pdfBytes == null || pdfBytes.Length == 0)
            throw new ArgumentException("PDF content cannot be null or empty", nameof(pdfBytes));

        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException("File name cannot be empty", nameof(fileName));

        // Ensure the file has a .pdf extension
        if (!fileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
            fileName += ".pdf";

        try
        {
            string mainFolderName = _googleDriveApiSettings.MainFolderName;

            // Find or create the main folder (dev/prod)
            var mainFolderQuery = _googleDriveService.Files.List();
            mainFolderQuery.Q = $"name = '{mainFolderName}' and mimeType = 'application/vnd.google-apps.folder' and 'root' in parents";
            mainFolderQuery.Spaces = "drive";
            var mainFolders = mainFolderQuery.Execute();

            string mainFolderId;
            if (mainFolders.Files.Count == 0)
            {
                // Create the main folder if it doesn't exist
                var mainFolderMetadata = new Google.Apis.Drive.v3.Data.File
                {
                    Name = mainFolderName,
                    Parents = new List<string> { "root" },
                    MimeType = "application/vnd.google-apps.folder"
                };
                var createMainFolderRequest = _googleDriveService.Files.Create(mainFolderMetadata);
                createMainFolderRequest.Fields = "id";
                var createdMainFolder = createMainFolderRequest.Execute();
                mainFolderId = createdMainFolder.Id;
            }
            else
            {
                mainFolderId = mainFolders.Files[0].Id;
            }

            // Find or create the letters subfolder
            var lettersFolderQuery = _googleDriveService.Files.List();
            lettersFolderQuery.Q = $"name = '{fileType}' and mimeType = 'application/vnd.google-apps.folder' and '{mainFolderId}' in parents";
            lettersFolderQuery.Spaces = "drive";
            var lettersFolders = lettersFolderQuery.Execute();

            string lettersFolderId;
            if (lettersFolders.Files.Count == 0)
            {
                // Create the letters subfolder if it doesn't exist
                var lettersFolderMetadata = new Google.Apis.Drive.v3.Data.File
                {
                    Name = fileType,
                    Parents = new List<string> { mainFolderId },
                    MimeType = "application/vnd.google-apps.folder"
                };
                var createLettersFolderRequest = _googleDriveService.Files.Create(lettersFolderMetadata);
                createLettersFolderRequest.Fields = "id";
                var createdLettersFolder = createLettersFolderRequest.Execute();
                lettersFolderId = createdLettersFolder.Id;
            }
            else
            {
                lettersFolderId = lettersFolders.Files[0].Id;
            }

            // Prepare file metadata with the correct parent folder
            var fileMetadata = new Google.Apis.Drive.v3.Data.File
            {
                Name = fileName,
                Parents = new List<string> { lettersFolderId },
                MimeType = "application/pdf"
            };

            // Create file stream from byte array
            using (var stream = new MemoryStream(pdfBytes))
            {
                // Create and execute the file upload request
                var request = _googleDriveService.Files.Create(fileMetadata, stream, "application/pdf");
                request.Fields = "id";
                var uploadedFile = request.Upload();

                // Create a shareable link
                var permission = new Google.Apis.Drive.v3.Data.Permission
                {
                    Type = "anyone",
                    Role = "reader"
                };
                var permissionRequest = _googleDriveService.Permissions.Create(permission, request.ResponseBody.Id);
                await permissionRequest.ExecuteAsync();

                Console.WriteLine($"File permanently stored in Google Drive with ID: {request.ResponseBody.Id}");
            }
        }
        catch (Exception ex)
        {
            // Log the exception and rethrow
            Console.WriteLine($"Error uploading PDF to Google Drive: {ex.Message}");
            throw;
        }
    }
}