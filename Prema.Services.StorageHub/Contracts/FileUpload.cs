namespace Prema.Services.StorageHub.Contracts
{
    public class FileUpload
    {
        public string key { get; set; }
        public byte[] fileContent { get; set; }
        public string fileName { get; set; }
    }
}
