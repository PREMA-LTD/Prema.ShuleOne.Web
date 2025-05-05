using MassTransit;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Prema.Services.StorageHub.Contracts;
using Prema.Services.StorageHub.Services;

namespace Prema.Services.UnifiedNotifier.Consumers
{
    public class FileUploaderConsumer(GoogleFileStorageService fileStorageService) : IConsumer<FileUpload>
    {
        public async Task Consume(ConsumeContext<FileUpload> context)
        {
            await fileStorageService.SaveFileAsync(context.Message.key, context.Message.fileContent, context.Message.fileName);
        }

    }
}
