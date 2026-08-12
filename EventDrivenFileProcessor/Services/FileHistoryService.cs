using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using EventDrivenFileProcessor.Models;

namespace EventDrivenFileProcessor.Services
{
    public class FileHistoryService
    {
        private readonly IDynamoDBContext _db;

        public FileHistoryService(IDynamoDBContext db)
        {
            _db = db;
        }

        public async Task SaveAsync(FileHistory file)
        {
            await _db.SaveAsync(file);
        }

        public async Task<List<FileHistory>> GetAllAsync()
        {
            var scan =
                _db.ScanAsync<FileHistory>(
                    new List<ScanCondition>());

            return await scan.GetRemainingAsync();
        }

        public async Task UpdateStatusAsync(
    string fileId,
    string status)
        {
            var file =
                await _db.LoadAsync<FileHistory>(fileId);

            if (file != null)
            {
                file.Status = status;

                await _db.SaveAsync(file);
            }
        }

        public async Task<FileHistory?> GetByS3KeyAsync(
    string s3Key)
        {
            var scan = _db.ScanAsync<FileHistory>(
                new List<ScanCondition>
                {
            new ScanCondition(
                "S3Key",
                ScanOperator.Equal,
                s3Key)
                });

            return (await scan.GetRemainingAsync())
                .FirstOrDefault();
        }
    }
}
