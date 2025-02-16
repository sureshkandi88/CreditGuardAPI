using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CreditGuardAPI.Models
{
    public enum EmiTransactionStatus
    {
        Pending,
        Paid,
        Overdue,
        Defaulted
    }

    public class EmiTransaction
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int DebtId { get; set; }
        public Debt Debt { get; set; }

        [Required]
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }

        [Required]
        public int GroupId { get; set; }
        public Group Group { get; set; }

        [Required]
        public int EMINumber { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal EMIAmount { get; set; }

        public EmiTransactionStatus Status { get; set; } = EmiTransactionStatus.Pending;

        public DateTime DueDate { get; set; }
        public DateTime? PaidDate { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? PaidAmount { get; set; }
    }
}
