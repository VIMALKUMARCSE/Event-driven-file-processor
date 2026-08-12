using Amazon.DynamoDBv2.DataModel;

namespace EventDrivenFileProcessor.Models
{
    [DynamoDBTable("FileHistory")]
    public class FileHistory
    {
        [DynamoDBHashKey]
        public string FileId { get; set; } = "";

        public string FileName { get; set; } = "";

        public string UserEmail { get; set; } = "";

        public string Status { get; set; } = "";

        public DateTime UploadDate { get; set; }

        public string S3Key { get; set; } = "";
    }
}
