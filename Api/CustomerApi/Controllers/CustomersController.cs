using Customer.Api.Attributes;
using Customer.Business.IServices;
using Customer.Dto.Customer.Request;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Customer.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ValidateModel]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerServices _customerServices;

        public CustomersController(ICustomerServices customerServices, IConfiguration configuration)
        {
            _customerServices = customerServices;
        }
        [HttpPost]
        public async Task<IActionResult> Create([Required] Customers customer)
        {
            if (ModelState.IsValid)
            {
                await _customerServices.Create(customer);
                return Ok();
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCustomers()
        {
            return Ok(await _customerServices.GetCustomers());
        }

        [HttpGet("{customerId}")]
        public async Task<IActionResult> GetCustomer([Required] Guid customerId)
        {
            return Ok(await _customerServices.GetCustomer(customerId));
        }

        [HttpGet("age/{age}")]
        public async Task<IActionResult> GetCustomerByAge([Range(1, 100)] int age)
        {
            return Ok(await _customerServices.GetCustomerByAge(age));
        }

        [HttpPatch("{customerId}")]
        public async Task<IActionResult> UpdateCustomer([Required] UpdateCustomer customer)
        {
            customer.CustomerId = customer.CustomerId;
            await _customerServices.Update(customer);
            return Ok();

        }

    }
}
