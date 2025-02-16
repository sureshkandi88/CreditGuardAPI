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

        // Method to safely migrate database without data loss
        public void MigrateDatabase()
        {
            // Ensure the database is created
            Database.EnsureCreated();

            // Check if there are any pending migrations
            var pendingMigrations = Database.GetPendingMigrations();
            
            // If there are pending migrations, apply them
            if (pendingMigrations.Any())
            {
                Database.Migrate();
            }
        }

        // Override OnModelCreating to configure model relationships and constraints
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Ensure Aadhaar number is unique
            modelBuilder.Entity<Customer>()
                .HasIndex(c => c.AadhaarNumber)
                .IsUnique();

            // Configure Many-to-Many relationship between Customer and Group
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

            // Configure Group to Debt relationship
            modelBuilder.Entity<Debt>()
                .HasOne(d => d.Group)
                .WithMany(g => g.Debts)
                .HasForeignKey(d => d.GroupId);

            // Configure Customer to EMI Transaction relationship
            modelBuilder.Entity<EmiTransaction>()
                .HasOne(et => et.Customer)
                .WithMany(c => c.EmiTransactions)
                .HasForeignKey(et => et.CustomerId);

            // Configure Debt to EMI Transaction relationship
            modelBuilder.Entity<EmiTransaction>()
                .HasOne(et => et.Debt)
                .WithMany(d => d.EmiTransactions)
                .HasForeignKey(et => et.DebtId);

            // Configure Group to EMI Transaction relationship
            modelBuilder.Entity<EmiTransaction>()
                .HasOne(et => et.Group)
                .WithMany(g => g.EmiTransactions)
                .HasForeignKey(et => et.GroupId);

            // Configure Active Group for Customer
            modelBuilder.Entity<Customer>()
                .HasOne(c => c.ActiveGroup)
                .WithMany(g => g.ActiveCustomers)
                .HasForeignKey(c => c.ActiveGroupId)
                .IsRequired(false);
        }
    }
}
