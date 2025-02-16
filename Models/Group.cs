using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CreditGuardAPI.Models
{
    public class Group
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;

        // Navigation properties
        // Many-to-Many relationship with Customers
        public ICollection<CustomerGroup> CustomerGroups { get; set; }

        // One-to-Many relationship with Debts
        public ICollection<Debt> Debts { get; set; }

        // Customers currently active in this group
        public ICollection<Customer> ActiveCustomers { get; set; }

        // One-to-Many relationship with EMI Transactions
        public ICollection<EmiTransaction> EmiTransactions { get; set; }
    }
}
