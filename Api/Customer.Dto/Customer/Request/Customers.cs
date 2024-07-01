using Customer.Dto.Attributes;
using System.ComponentModel.DataAnnotations;
namespace Customer.Dto.Customer.Request
{
    public class Customers
    {
        public Guid? CustomerId { get; set; }

        [Required(ErrorMessage = "FullName is required.")]
        [StringLength(100, ErrorMessage = "FullName can't be longer than 100 characters.")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Date of birth is required.")]
        [AgeRange(1, 100, ErrorMessage = "Age must be between 1 and 100.")]
        public DateOnly DateOfBirth { get; set; }
        public byte[]? ProfileImage { get; set; }

    }
}
