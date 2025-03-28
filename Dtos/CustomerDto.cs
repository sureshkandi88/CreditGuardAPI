using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace CreditGuardAPI.Dtos
{
    public class CustomerDto
    {
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }

        [StringLength(50)]
        public string? LastName { get; set; }

        [Required]
        [Phone]
        [StringLength(15)]
        public string PhoneNumber { get; set; }

        [Required]
        [StringLength(12)]
        [RegularExpression(@"^\d{12}$", ErrorMessage = "Aadhaar number must be 12 digits")]
        public string AadhaarNumber { get; set; }

        [Required]
        [StringLength(200)]
        public string Street { get; set; }

        [Required]
        [StringLength(50)]
        public string CityName { get; set; }

        [Required]
        [StringLength(50)]
        public string State { get; set; }

        [StringLength(50)]
        public string? Location { get; set; }

        
        public IFormFile? ProfilePhoto { get; set; }
        public IFormFile? AadhaarPhoto { get; set; }
    }
}
