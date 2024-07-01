using System.ComponentModel.DataAnnotations;

namespace Customer.Data.Model
{
    public class Customers
    {
        [Key]
        public Guid CustomerId { get; set; }
        [Required]
        [MaxLength(256)]
        public string FullName { get; set; }
        [Required]
        public DateOnly DateOfBirth { get; set; }
        public byte[]? ProfileImage { get; set; }
    }
}
