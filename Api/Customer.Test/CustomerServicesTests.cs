using AutoMapper;
using Customer.Business.Services;
using Customer.Data.ICoreService;
using Customer.Dto.Customer.Request;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using System.Net;

namespace Customer.Test
{
    public class CustomerServicesTests
    {

        [Fact]
        public async Task Create_ValidCustomer_CreatesCustomerWithProfileImage()
        {
            // Arrange
            var customers = new Dto.Customer.Request.Customers
            {
                CustomerId = Guid.NewGuid(),
                FullName = "Biren Modi",
                DateOfBirth = new DateOnly(1990, 1, 1)
            };

            var profileImageBytes = new byte[] { 0x00, 0x01, 0x02 };

            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                                 .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                                 .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
                                 {
                                     Content = new ByteArrayContent(profileImageBytes)
                                 });

            var httpClient = new HttpClient(mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("https://ui-avatars.com/")
            };

            var mockCoreServices = new Mock<ICoreCustomerServices>();
            mockCoreServices.Setup(core => core.Create(It.IsAny<Customer.Data.Model.Customers>()))
                            .Returns(Task.CompletedTask);

            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(x => x["ProfileImage"]).Returns("https://ui-avatars.com/api/?name={FULL_NAME}&format=svg");

            var mockMapper = new Mock<IMapper>();
            mockMapper
            .Setup(m => m.Map<Customer.Data.Model.Customers>(It.IsAny<Customers>()))
            .Returns((Customers source) => new Customer.Data.Model.Customers
            {
                CustomerId = source.CustomerId.Value,
                FullName = source.FullName,
                DateOfBirth = source.DateOfBirth,
                ProfileImage = source.ProfileImage
            });


            var customerServices = new CustomerServices(mockCoreServices.Object, httpClient, mockConfiguration.Object, mockMapper.Object);

            // Act
            await customerServices.Create(customers);

            // Assert
            mockHttpMessageHandler.Protected().Verify<Task<HttpResponseMessage>>("SendAsync", Times.Once(),
                ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>());

            mockCoreServices.Verify(core => core.Create(It.Is<Customer.Data.Model.Customers>(c =>
                c.CustomerId != Guid.Empty &&
                c.FullName == customers.FullName &&
                c.DateOfBirth == customers.DateOfBirth &&
                c.ProfileImage.SequenceEqual(profileImageBytes))), Times.Once);
        }

        [Fact]
        public async Task Create_AvatarApiFailure_ThrowsException()
        {
            // Arrange
            var customers = new Dto.Customer.Request.Customers
            {
                CustomerId = Guid.NewGuid(),
                FullName = "Biren Modi",
                DateOfBirth = new DateOnly(1990, 1, 1)
            };

            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                                 .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                                 .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.InternalServerError));

            var httpClient = new HttpClient(mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("https://ui-avatars.com/")
            };

            var mockCoreServices = new Mock<ICoreCustomerServices>();

            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(x => x["ProfileImage"]).Returns("https://ui-avatars.com/api/?name={FULL_NAME}&format=svg");

            var mockMapper = new Mock<IMapper>();

            var customerServices = new CustomerServices(mockCoreServices.Object, httpClient, mockConfiguration.Object, mockMapper.Object);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => customerServices.Create(customers));
        }

        [Fact]
        public async Task GetCustomer_ValidCustomerId_ReturnsCustomerDto()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var coreCustomer = new Customer.Data.Model.Customers
            {
                CustomerId = customerId,
                FullName = "Biren Modi",
                DateOfBirth = new DateOnly(1990, 1, 1)
            };

            var mockCoreServices = new Mock<ICoreCustomerServices>();
            mockCoreServices.Setup(core => core.GetCustomer(customerId))
                            .ReturnsAsync(coreCustomer);

            var mockMapper = new Mock<IMapper>();

            var customerServices = new CustomerServices(mockCoreServices.Object, null, null, mockMapper.Object);

            // Act
            var result = await customerServices.GetCustomer(customerId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(customerId, result.CustomerId);
            Assert.Equal(coreCustomer.FullName, result.FullName);
            Assert.Equal(coreCustomer.DateOfBirth, result.DateOfBirth);
        }

        [Fact]
        public async Task GetCustomer_ExceptionThrown_ReturnsNull()
        {
            // Arrange
            var customerId = Guid.NewGuid();

            var mockCoreServices = new Mock<ICoreCustomerServices>();
            mockCoreServices.Setup(core => core.GetCustomer(customerId))
                            .ThrowsAsync(new Exception("Exception"));

            var customerServices = new CustomerServices(mockCoreServices.Object, null, null, null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => customerServices.GetCustomer(customerId));
        }

        [Fact]
        public async Task GetCustomerByAge_ValidAge_ReturnsCustomerDtoList()
        {
            // Arrange
            var age = 30;
            var coreCustomers = new List<Customer.Data.Model.Customers>
            {
                new Customer.Data.Model.Customers
                {
                    CustomerId = Guid.NewGuid(),
                    FullName = "Biren Modi",
                    DateOfBirth = new DateOnly(1990, 1, 1)
                },
                new Customer.Data.Model.Customers
                {
                    CustomerId = Guid.NewGuid(),
                    FullName = "Developer Testing",
                    DateOfBirth = new DateOnly(1991, 5, 15)
                }
            };

            var mockCoreServices = new Mock<ICoreCustomerServices>();
            mockCoreServices.Setup(core => core.GetCustomerByAge(age))
                            .ReturnsAsync(coreCustomers);

            var customerServices = new CustomerServices(mockCoreServices.Object, null, null, null);

            // Act
            var result = await customerServices.GetCustomerByAge(age);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(coreCustomers.Count, result.Count);

            foreach (var customerDto in result)
            {
                var correspondingCoreCustomer = coreCustomers.FirstOrDefault(c => c.CustomerId == customerDto.CustomerId);
                Assert.NotNull(correspondingCoreCustomer);
                Assert.Equal(correspondingCoreCustomer.FullName, customerDto.FullName);
                Assert.Equal(correspondingCoreCustomer.DateOfBirth, customerDto.DateOfBirth);
            }
        }

        [Fact]
        public async Task GetCustomerByAge_NoCustomersFound_ReturnsEmptyList()
        {
            // Arrange
            var age = 20;
            var mockCoreServices = new Mock<ICoreCustomerServices>();
            mockCoreServices.Setup(core => core.GetCustomerByAge(age))
                            .ReturnsAsync(new List<Customer.Data.Model.Customers>());

            var customerServices = new CustomerServices(mockCoreServices.Object, null, null, null);

            // Act
            var result = await customerServices.GetCustomerByAge(age);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetCustomers_ReturnsCustomerDtoList()
        {
            // Arrange
            var coreCustomers = new List<Customer.Data.Model.Customers>
            {
                new Customer.Data.Model.Customers
                {
                    CustomerId = Guid.NewGuid(),
                    FullName = "Biren Modi",
                    DateOfBirth = new DateOnly(1990, 1, 1)
                },
                new Customer.Data.Model.Customers
                {
                    CustomerId = Guid.NewGuid(),
                    FullName = "Developer Testing",
                    DateOfBirth = new DateOnly(1991, 5, 15)
                }
            };

            var mockCoreServices = new Mock<ICoreCustomerServices>();
            mockCoreServices.Setup(core => core.GetCustomers())
                            .ReturnsAsync(coreCustomers);

            var customerServices = new CustomerServices(mockCoreServices.Object, null, null, null);

            // Act
            var result = await customerServices.GetCustomers();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(coreCustomers.Count, result.Count);

            foreach (var customerDto in result)
            {
                var correspondingCoreCustomer = coreCustomers.FirstOrDefault(c => c.CustomerId == customerDto.CustomerId);
                Assert.NotNull(correspondingCoreCustomer);
                Assert.Equal(correspondingCoreCustomer.FullName, customerDto.FullName);
                Assert.Equal(correspondingCoreCustomer.DateOfBirth, customerDto.DateOfBirth);
            }
        }

        [Fact]
        public async Task GetCustomers_NoCustomersFound_ReturnsEmptyList()
        {
            // Arrange
            var mockCoreServices = new Mock<ICoreCustomerServices>();
            mockCoreServices.Setup(core => core.GetCustomers())
                            .ReturnsAsync(new List<Customer.Data.Model.Customers>());

            var customerServices = new CustomerServices(mockCoreServices.Object, null, null, null);

            // Act
            var result = await customerServices.GetCustomers();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task Update_ValidCustomer_ReturnsUpdatedCustomer()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var updateCustomer = new UpdateCustomer
            {
                CustomerId = customerId,
                FullName = "Updated Name",
                DateOfBirth = new DateOnly(1985, 3, 15)
            };

            var existingCustomer = new Customer.Data.Model.Customers
            {
                CustomerId = customerId,
                FullName = "Biren Modi",
                DateOfBirth = new DateOnly(1990, 1, 1)
            };

            var mockCoreServices = new Mock<ICoreCustomerServices>();
            mockCoreServices.Setup(core => core.GetCustomer(customerId))
                            .ReturnsAsync(existingCustomer);


            var customerServices = new CustomerServices(mockCoreServices.Object, null, null, null);

            // Act
            await customerServices.Update(updateCustomer);

            // Assert
            Assert.Equal(updateCustomer.FullName, existingCustomer.FullName);
            Assert.Equal(updateCustomer.DateOfBirth, existingCustomer.DateOfBirth);
            mockCoreServices.Verify(core => core.Update(existingCustomer), Times.Once);
        }

        [Fact]
        public async Task Update_NonExistentCustomer_ThrowsArgumentException()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var updateCustomer = new UpdateCustomer
            {
                CustomerId = customerId,
                FullName = "Updated Name",
                DateOfBirth = new DateOnly(1985, 3, 15)
            };

            var mockCoreServices = new Mock<ICoreCustomerServices>();
            mockCoreServices.Setup(core => core.GetCustomer(customerId))
                            .ReturnsAsync((Customer.Data.Model.Customers)null);

            var customerServices = new CustomerServices(mockCoreServices.Object, null, null, null);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => customerServices.Update(updateCustomer));
            mockCoreServices.Verify(core => core.Update(It.IsAny<Customer.Data.Model.Customers>()), Times.Never);
        }
        [Fact]
        public async Task Update_PartialCustomerUpdate_ReturnsUpdatedCustomer()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var updateCustomer = new UpdateCustomer
            {
                CustomerId = customerId,
                FullName = "Updated Name"
            };

            var existingCustomer = new Customer.Data.Model.Customers
            {
                CustomerId = customerId,
                FullName = "Biren Modi",
                DateOfBirth = new DateOnly(1990, 1, 1)
            };

            var mockCoreServices = new Mock<ICoreCustomerServices>();
            mockCoreServices.Setup(core => core.GetCustomer(customerId))
                            .ReturnsAsync(existingCustomer);

            var customerServices = new CustomerServices(mockCoreServices.Object, null, null, null);

            // Act
            await customerServices.Update(updateCustomer);

            // Assert
            Assert.Equal(updateCustomer.FullName, existingCustomer.FullName);
            Assert.Equal(existingCustomer.DateOfBirth, new DateOnly(1990, 1, 1));
            mockCoreServices.Verify(core => core.Update(existingCustomer), Times.Once);
        }

        [Fact]
        public async Task Update_ExceptionThrown_ThrowsException()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var updateCustomer = new UpdateCustomer
            {
                CustomerId = customerId,
                FullName = "Updated Name",
                DateOfBirth = new DateOnly(1985, 3, 15)
            };

            var mockCoreServices = new Mock<ICoreCustomerServices>();
            mockCoreServices.Setup(core => core.GetCustomer(customerId))
                            .ThrowsAsync(new Exception("Exception"));

            var customerServices = new CustomerServices(mockCoreServices.Object, null, null, null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => customerServices.Update(updateCustomer));
            mockCoreServices.Verify(core => core.Update(It.IsAny<Customer.Data.Model.Customers>()), Times.Never);
        }

    }
}
