using Microsoft.EntityFrameworkCore;
using CreditGuardAPI.Models;

namespace CreditGuardAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSet properties for all models
        public DbSet<User> Users { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Debt> Debts { get; set; }
        public DbSet<EmiTransaction> EmiTransactions { get; set; }
        public DbSet<CustomerGroup> CustomerGroups { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasAnnotation("RegularExpression", @"^[a-zA-Z0-9_]+$");
                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(100);
                entity.Property(e => e.PasswordHash)
                    .IsRequired()
                    .HasMaxLength(100);
                entity.HasIndex(e => e.Username).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
            });

            // Customer configuration
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.AadhaarNumber).IsRequired().HasMaxLength(12);
                entity.HasIndex(e => e.AadhaarNumber).IsUnique();
            });

            // Group configuration
            modelBuilder.Entity<Group>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            });

            // Debt configuration
            modelBuilder.Entity<Debt>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.TotalAmount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.RemainingAmount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.EMIAmount).HasColumnType("decimal(18,2)");
                
                entity.HasOne(d => d.Group)
                    .WithMany(g => g.Debts)
                    .HasForeignKey(d => d.GroupId);
            });

            // CustomerGroup configuration
            modelBuilder.Entity<CustomerGroup>()
                .HasKey(cg => new { cg.CustomerId, cg.GroupId });

            modelBuilder.Entity<CustomerGroup>()
                .HasOne(cg => cg.Customer)
                .WithMany(c => c.CustomerGroups)
                .HasForeignKey(cg => cg.CustomerId);

            modelBuilder.Entity<CustomerGroup>()
                .HasOne(cg => cg.Group)
                .WithMany(g => g.CustomerGroups)
                .HasForeignKey(cg => cg.GroupId);

            // EMI Transaction configuration
            modelBuilder.Entity<EmiTransaction>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.EMIAmount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.PaidAmount).HasColumnType("decimal(18,2)");

                entity.HasOne(et => et.Debt)
                    .WithMany(d => d.EmiTransactions)
                    .HasForeignKey(et => et.DebtId);

                entity.HasOne(et => et.Customer)
                    .WithMany(c => c.EmiTransactions)
                    .HasForeignKey(et => et.CustomerId);

                entity.HasOne(et => et.Group)
                    .WithMany(g => g.EmiTransactions)
                    .HasForeignKey(et => et.GroupId);
            });
        }

        // Optional method for database migration
        public void MigrateDatabase()
        {
            Database.EnsureCreated();
            var pendingMigrations = Database.GetPendingMigrations();
            
            if (pendingMigrations.Any())
            {
                Database.Migrate();
            }
        }
    }
}
