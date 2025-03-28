using CreditGuardAPI.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace CreditGuardAPI.Services
{
    public class FileSizeLimitExceededException : Exception
    {
        public FileSizeLimitExceededException(string message) : base(message) { }
    }

    public interface IStaticFileService
    {
        Task<int> SaveFileAsync(byte[] content, string fileName, string fileType, string relatedEntityType, int relatedEntityId);
        Task<StaticFile> GetFileAsync(int fileId);
        Task<bool> DeleteFileAsync(int fileId);
    }

    public class StaticFileService : IStaticFileService
    {
        private readonly StaticFilesDbContext _context;
        private readonly IConfiguration _configuration;

        public StaticFileService(StaticFilesDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<int> SaveFileAsync(byte[] content, string fileName, string fileType, string relatedEntityType, int relatedEntityId)
        {
            var maxFileSize = _configuration.GetValue<int>("FileUpload:MaxFileSize");
            if (content.Length > maxFileSize)
            {
                throw new FileSizeLimitExceededException($"File size exceeds the maximum limit of {maxFileSize / 1024 / 1024} MB");
            }

            var file = new StaticFile
            {
                FileName = fileName,
                FileType = fileType,
                Content = content,
                UploadedAt = DateTime.UtcNow,
                RelatedEntityType = relatedEntityType,
                RelatedEntityId = relatedEntityId
            };

            _context.StaticFiles.Add(file);
            await _context.SaveChangesAsync();
            return file.Id;
        }

        public async Task<StaticFile> GetFileAsync(int fileId)
        {
            return await _context.StaticFiles.FindAsync(fileId);
        }

        public async Task<bool> DeleteFileAsync(int fileId)
        {
            var file = await _context.StaticFiles.FindAsync(fileId);
            if (file == null) return false;

            _context.StaticFiles.Remove(file);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}