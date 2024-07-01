using Customer.Data.Context;
using Customer.Data.ICoreService;
using Microsoft.EntityFrameworkCore;

namespace Customer.Data.CoreService
{
    public class CoreCustomerServices : ICoreCustomerServices
    {
        private readonly CustomerApiDbContext _context;

        public CoreCustomerServices(CustomerApiDbContext context)
        {
            _context = context;
        }

        public async Task Create(Model.Customers customer)
        {
            await _context.AddAsync(customer);
            await _context.SaveChangesAsync();
        }

        public async Task<Model.Customers?> GetCustomer(Guid customerId)
        {
            return await Customers().FirstOrDefaultAsync(x => x.CustomerId == customerId);

        }

        public async Task<List<Model.Customers>> GetCustomers()
        {
            return await Customers().ToListAsync();
        }

        public async Task<List<Model.Customers>> GetCustomerByAge(int age)
        {
            var today = DateTime.Today;
            return await Customers()
                .Where(customer =>
                    (today.Year - customer.DateOfBirth.Year) -
                    ((today.Month < customer.DateOfBirth.Month) ||
                    (today.Month == customer.DateOfBirth.Month && today.Day < customer.DateOfBirth.Day) ? 1 : 0) == age).ToListAsync();
        }

        public async Task Update(Model.Customers customer)
        {
            _context.Update(customer);
            await _context.SaveChangesAsync();
        }

        private IQueryable<Model.Customers> Customers()
        {
            return _context.Customers.Select(customer => new Model.Customers
            {
                CustomerId = customer.CustomerId,
                DateOfBirth = customer.DateOfBirth,
                FullName = customer.FullName
            });
        }
    }
}
