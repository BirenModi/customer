namespace Customer.Data.ICoreService
{
    public interface ICoreCustomerServices
    {
        Task Create(Model.Customers customer);
        Task Update(Model.Customers customer);
        Task<List<Model.Customers>> GetCustomers();
        Task<Model.Customers?> GetCustomer(Guid customerId);
        Task<List<Model.Customers>> GetCustomerByAge(int age);
    }
}
