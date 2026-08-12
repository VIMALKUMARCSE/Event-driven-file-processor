
using Amazon.S3;
using Amazon.S3.Transfer;

namespace EventDrivenFileProcessor.Services
{
    public class S3Service
    {
        private readonly IConfiguration _configuration;

        public S3Service(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task UploadFileAsync(Stream stream, string fileName)
        {
            var bucketName = _configuration["AWS:BucketName"];

            var region = Amazon.RegionEndpoint.APSouth1;

            using var client = new AmazonS3Client(region);

            var transferUtility = new TransferUtility(client);

            await transferUtility.UploadAsync(
                stream,
                bucketName,
                $"uploads/{fileName}"
            );
        }
    }
}
