using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CreditGuardAPI.Models
{
    public enum DebtStatus
    {
        Pending,
        Active,
        Completed,
        Defaulted
    }

    public class Debt
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int GroupId { get; set; }
        public Group Group { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal RemainingAmount { get; set; }

        [Required]
        public int TotalEMICount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal EMIAmount { get; set; }

        public DebtStatus Status { get; set; } = DebtStatus.Pending;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        // One-to-Many relationship with EMI Transactions
        public ICollection<EmiTransaction> EmiTransactions { get; set; }
    }
}
