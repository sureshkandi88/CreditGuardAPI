using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CreditGuardAPI.Models
{
    public class Customer
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }

        [StringLength(50)]
        public string? LastName { get; set; }

        [Required]
        [StringLength(12)]
        [RegularExpression(@"^\d{12}$", ErrorMessage = "Aadhaar number must be exactly 12 numeric digits")]
        public string AadhaarNumber { get; set; }

        [Required]
        [RegularExpression(@"^\+91\d{10}$", ErrorMessage = "Phone number must be in +91XXXXXXXXXX format")]
        public string PhoneNumber { get; set; }

        [Required]
        [StringLength(200)]
        public string Street { get; set; }

        [Required]
        [StringLength(50)]
        public string CityName { get; set; }

        [Required]
        [StringLength(50)]
        public string State { get; set; }

                
        public string? Location { get; set; }

        public int? ProfilePictureId { get; set; }
        public int? AadhaarPictureId { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation properties
        public int? ActiveGroupId { get; set; }
        public Group? ActiveGroup { get; set; }

        // Many-to-Many relationship with Groups
        public ICollection<CustomerGroup> CustomerGroups { get; set; }

        // One-to-Many relationship with EMI Transactions
        public ICollection<EmiTransaction> EmiTransactions { get; set; }

        // Optional: Add a computed full address property
        [NotMapped]
        public string FullAddress => 
            $"{Street}, {CityName}, {State}";
    }

    // Join entity for Many-to-Many relationship between Customer and Group
    public class CustomerGroup
    {
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }

        public int GroupId { get; set; }
        public Group Group { get; set; }

        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
    }
}
