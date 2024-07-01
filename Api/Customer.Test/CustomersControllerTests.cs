using Customer.Api.Controllers;
using Customer.Business.IServices;
using Customer.Dto.Customer.Request;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Customer.Test
{
    public class CustomersControllerTests
    {
        private readonly Mock<ICustomerServices> _mockCustomerServices;
        private readonly CustomersController _controller;

        public CustomersControllerTests()
        {
            _mockCustomerServices = new Mock<ICustomerServices>();
            _controller = new CustomersController(_mockCustomerServices.Object, null); // Pass null for IConfiguration in this example
        }

        [Fact]
        public async Task Create_ValidCustomer_ReturnsOk()
        {
            // Arrange
            var validCustomer = new Customers
            {
                CustomerId = Guid.NewGuid(),
                FullName = "Biren Modi",
                DateOfBirth = new DateOnly(1990, 1, 1)
            };

            _controller.ModelState.Clear();

            _mockCustomerServices.Setup(services => services.Create(validCustomer));

            // Act
            var result = await _controller.Create(validCustomer);

            // Assert
            result.Should().BeOfType<OkResult>();

            _mockCustomerServices.Verify(services => services.Create(validCustomer), Times.Once);
        }


        [Fact]
        public async Task Create_InvalidCustomer_ReturnsBadRequest()
        {
            // Arrange
            var invalidCustomer = new Customers
            {
                CustomerId = Guid.NewGuid()
            };

            _controller.ModelState.Clear();

            if (invalidCustomer.FullName == null)
            {
                _controller.ModelState.AddModelError(nameof(invalidCustomer.FullName), "The FullName field is required.");
            }
            if (invalidCustomer.DateOfBirth == default)
            {
                _controller.ModelState.AddModelError(nameof(invalidCustomer.DateOfBirth), "The DateOfBirth field is required.");
            }

            // Act
            var result = await _controller.Create(invalidCustomer);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();

            _mockCustomerServices.Verify(services => services.Create(invalidCustomer), Times.Never);
        }

        [Fact]
        public async Task GetCustomers_ReturnsOkWithCustomers()
        {
            // Arrange
            var expectedCustomers = new List<Dto.Customer.Response.Customers>
    {
        new Dto.Customer.Response.Customers { CustomerId = Guid.NewGuid(), FullName = "Biren Modi",DateOfBirth=new DateOnly() },
        new Dto.Customer.Response.Customers { CustomerId = Guid.NewGuid(), FullName = "Developer Testing",DateOfBirth=new DateOnly() }
    };

            _mockCustomerServices.Setup(services => services.GetCustomers())
                                 .ReturnsAsync(expectedCustomers);

            // Act
            var result = await _controller.GetCustomers();

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var customers = okResult.Value.Should().BeAssignableTo<List<Dto.Customer.Response.Customers>>().Subject;

            customers.Should().BeEquivalentTo(expectedCustomers); ;
        }


    }
}
