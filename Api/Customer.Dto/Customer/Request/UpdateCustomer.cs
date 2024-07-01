namespace Customer.Dto.Customer.Request
{
    public class UpdateCustomer
    {
        public Guid? CustomerId { get; set; }
        public string? FullName { get; set; }
        public DateOnly? DateOfBirth { get; set; }

    }
}
