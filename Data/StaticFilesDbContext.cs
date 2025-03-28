using Microsoft.EntityFrameworkCore;
using System;

namespace CreditGuardAPI.Data
{
    public class StaticFile
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
        public byte[] Content { get; set; }
        public DateTime UploadedAt { get; set; }
        public string RelatedEntityType { get; set; }
        public int RelatedEntityId { get; set; }
    }

    public class StaticFilesDbContext : DbContext
    {
        public StaticFilesDbContext(DbContextOptions<StaticFilesDbContext> options)
            : base(options)
        {
        }

        public DbSet<StaticFile> StaticFiles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<StaticFile>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FileName).IsRequired();
                entity.Property(e => e.FileType).IsRequired();
                entity.Property(e => e.Content).IsRequired();
                entity.Property(e => e.UploadedAt).IsRequired();
                entity.Property(e => e.RelatedEntityType).IsRequired();
                
                // Create an index on RelatedEntityType and RelatedEntityId for faster lookups
                entity.HasIndex(e => new { e.RelatedEntityType, e.RelatedEntityId });
            });
        }
    }
}