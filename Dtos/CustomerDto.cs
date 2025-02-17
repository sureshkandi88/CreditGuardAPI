using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace CreditGuardAPI.Dtos
{
    public class CustomerDto
    {
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        public string LastName { get; set; }

        [Required]
        [Phone]
        [StringLength(15)]
        public string PhoneNumber { get; set; }

        [Required]
        [StringLength(12)]
        [RegularExpression(@"^\d{12}$", ErrorMessage = "Aadhaar number must be 12 digits")]
        public string AadhaarNumber { get; set; }

        public AddressDto Address { get; set; }

        public IFormFile? ProfilePhoto { get; set; }
        public IFormFile? AadhaarPhoto { get; set; }
    }

    public class AddressDto
    {
        [Required]
        [StringLength(200)]
        public string Street { get; set; }

        [Required]
        [StringLength(50)]
        public string City { get; set; }

        [Required]
        [StringLength(50)]
        public string State { get; set; }

        [Required]
        [RegularExpression(@"^\d{6}$", ErrorMessage = "Pin code must be 6 digits")]
        public string PinCode { get; set; }
    }
}
