namespace Customer.Dto.Customer.Response
{
    public class Customers
    {
        public Guid CustomerId { get; set; }
        public string FullName { get; set; }
        public DateOnly DateOfBirth { get; set; }
    }
}
