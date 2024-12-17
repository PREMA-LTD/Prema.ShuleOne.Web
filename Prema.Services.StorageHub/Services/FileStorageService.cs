using StackExchange.Redis;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Upload;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Mail;
using Microsoft.AspNetCore.DataProtection.KeyManagement;


namespace Prema.Services.StorageHub.Services;

public class FileStorageService
{
    private readonly IConnectionMultiplexer _redis;
    private readonly DriveService _googleDriveService;

    public FileStorageService(IConnectionMultiplexer redis, DriveService googleDriveService)
    {
        _redis = redis;
        _googleDriveService = googleDriveService;
    }

    public async Task SaveFileAsync(string key, byte[] fileContent, string fileName)
    {
        // Save to Redis
        IDatabase db = _redis.GetDatabase();
        await db.StringSetAsync(key, fileContent, TimeSpan.FromHours(1)); // Cache for 1 hour
    
        // Save to Google Drive
        await UploadPdfToGoogleDrive(fileContent, fileName);   
    
    }

    public async Task UploadPdfToGoogleDrive(byte[] pdfBytes, string fileName)
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
            var fileMetadata = new Google.Apis.Drive.v3.Data.File
            {
                Name = fileName,
            };

            using (var fileStream = new MemoryStream(pdfBytes))
            {
                var request = _googleDriveService.Files.Create(
                    fileMetadata, fileStream, "application/pdf");
                request.Fields = "id";

                var progress = await request.UploadAsync();

                if (progress.Status == Google.Apis.Upload.UploadStatus.Completed)
                {
                    Console.WriteLine("Upload successful!");
                    Console.WriteLine("File ID: " + request.ResponseBody.Id);
                }
                else
                {
                    Console.WriteLine($"Upload failed: {progress.Exception}");
                }
            }

            // Find the 'leters' folder
            //var folderQuery = _googleDriveService.Files.List();
            //folderQuery.Q = $"name = 'leters' and mimeType = 'application/vnd.google-apps.folder'";
            //folderQuery.Spaces = "drive";
            //var folders = folderQuery.Execute();

            //string folderId = null;
            //if (folders.Files.Count == 0)
            //{
            //    // Create the folder if it doesn't exist
            //    var folderMetadata = new Google.Apis.Drive.v3.Data.File
            //    {
            //        Name = "leters",
            //        MimeType = "application/vnd.google-apps.folder"
            //    };
            //    var createRequest = _googleDriveService.Files.Create(folderMetadata);
            //    createRequest.Fields = "id";
            //    var createdFolder = createRequest.Execute();
            //    folderId = createdFolder.Id;
            //}
            //else
            //{
            //    folderId = folders.Files[0].Id;
            //}

            //// Prepare file metadata
            //var fileMetadata = new Google.Apis.Drive.v3.Data.File
            //{
            //    Name = fileName,
            //    Parents = new List<string> { folderId },
            //    MimeType = "application/pdf"
            //};

            //// Create file stream from byte array
            //using (var stream = new MemoryStream(pdfBytes))
            //{
            //    // Create and execute the file upload request
            //    var request = _googleDriveService.Files.Create(fileMetadata, stream, "application/pdf");
            //    request.Fields = "id";
            //    var uploadedFile = request.Upload();

            //    // Return the ID of the uploaded file
            //    var here = request.ResponseBody?.Id;

            //    // Create a shareable link
            //    var permission = new Google.Apis.Drive.v3.Data.Permission
            //    {
            //        Type = "anyone",
            //        Role = "reader"
            //    };

            //    var permissionRequest = _googleDriveService.Permissions.Create(permission, request.ResponseBody.Id);
            //    await permissionRequest.ExecuteAsync();
            //    Console.WriteLine($"File permanently stored in Google Drive with ID: {request.ResponseBody.Id}");
            //}


        }
        catch (Exception ex)
        {
            // Log the exception and rethrow
            Console.WriteLine($"Error uploading PDF to Google Drive: {ex.Message}");
            throw;
        }
    }
}


//using var memoryStream = new MemoryStream(fileContent);
//var fileMetadata = new Google.Apis.Drive.v3.Data.File
//{
//    Name = fileName,
//    Parents = new[] { "root" } // Specify folder ID if needed
//};
//var request = _googleDriveService.Files.Create(fileMetadata, memoryStream, "application/pdf");
//request.Fields = "id";
//        var uploadProgress = await request.UploadAsync();
    
//        if (uploadProgress.Status == UploadStatus.Failed)
//        {
//            throw new Exception($"Failed to upload file to Google Drive: {uploadProgress.Exception}");
//        }
    
//        // Create a shareable link
//        var permission = new Google.Apis.Drive.v3.Data.Permission
//        {
//            Type = "anyone",
//            Role = "reader"
//        };

//var permissionRequest = _googleDriveService.Permissions.Create(permission, request.ResponseBody.Id);
//await permissionRequest.ExecuteAsync();

//// Generate shareable link
//var fileId = request.ResponseBody.Id;
//string shareableLink = $"https://drive.google.com/file/d/{fileId}/view";

//Console.WriteLine($"File saved to Redis with key: {key}");
//Console.WriteLine($"File permanently stored in Google Drive with ID: {fileId}");
//Console.WriteLine($"Shareable link: {shareableLink}");