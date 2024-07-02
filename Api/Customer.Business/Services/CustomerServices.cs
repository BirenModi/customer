using AutoMapper;
using Customer.Business.IServices;
using Customer.Data.ICoreService;
using Customer.Dto.Customer.Request;
using Microsoft.Extensions.Configuration;

namespace Customer.Business.Services
{
    public class CustomerServices : ICustomerServices
    {
        private readonly ICoreCustomerServices _coreServices;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public CustomerServices(ICoreCustomerServices coreServices, HttpClient httpClient, IConfiguration configuration, IMapper mapper)
        {
            _coreServices = coreServices;
            _httpClient = httpClient;
            _configuration = configuration;
            _mapper = mapper;
        }
        public async Task Create(Customers customers)
        {
            string profileImageTemplate = _configuration["ProfileImage"];
            string url = profileImageTemplate.Replace("{FULL_NAME}", customers.FullName);
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("avatar api is down");
            }
            var profileImage = await response.Content.ReadAsByteArrayAsync();
            customers.CustomerId = Guid.NewGuid();
            customers.ProfileImage = profileImage;
            await _coreServices.Create(_mapper.Map<Customer.Data.Model.Customers>(customers));
        }

        public async Task<Dto.Customer.Response.Customers> GetCustomer(Guid customerId)
        {
            var customer = await _coreServices.GetCustomer(customerId);
            if (customer == null)
            {
                throw new Exception($"Customer with ID {customerId} not found");
            }
            return new Dto.Customer.Response.Customers
            {
                CustomerId = customerId,
                FullName = customer.FullName,
                DateOfBirth = customer.DateOfBirth
            };

        }

        public async Task<List<Dto.Customer.Response.Customers>> GetCustomerByAge(int age)
        {
            var customer = await _coreServices.GetCustomerByAge(age);
            if (customer == null)
            {
                throw new Exception($"Customer with age {age} not found");
            }
            return customer.Select(x => new Dto.Customer.Response.Customers
            {
                CustomerId = x.CustomerId,
                FullName = x.FullName,
                DateOfBirth = x.DateOfBirth
            }).ToList();

        }

        public async Task<List<Dto.Customer.Response.Customers>> GetCustomers()
        {
            var customer = await _coreServices.GetCustomers();
            if (customer == null)
            {
                throw new Exception("Customers not found");
            }

            return customer.Select(x => new Dto.Customer.Response.Customers
            {
                CustomerId = x.CustomerId,
                FullName = x.FullName,
                DateOfBirth = x.DateOfBirth
            }).ToList();
        }

        public async Task Update(UpdateCustomer customer)
        {
            var existingCustomer = await _coreServices.GetCustomer(customer.CustomerId.Value);

            if (existingCustomer == null)
            {
                throw new ArgumentException("Customer not found");
            }


            if (!string.IsNullOrEmpty(customer.FullName))
            {
                existingCustomer.FullName = customer.FullName;
            }

            if (customer.DateOfBirth.HasValue)
            {
                existingCustomer.DateOfBirth = customer.DateOfBirth.Value;
            }
            await _coreServices.Update(existingCustomer);
        }
    }
}
