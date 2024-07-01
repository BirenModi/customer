using Customer.Dto.Customer.Request;

namespace Customer.Business.IServices
{
    public interface ICustomerServices
    {
        Task Create(Customers customers);
        Task Update(UpdateCustomer customers);
        Task<Customer.Dto.Customer.Response.Customers> GetCustomer(Guid customerId);
        Task<List<Customer.Dto.Customer.Response.Customers>> GetCustomers();
        Task<List<Customer.Dto.Customer.Response.Customers>> GetCustomerByAge(int age);
    }
}
