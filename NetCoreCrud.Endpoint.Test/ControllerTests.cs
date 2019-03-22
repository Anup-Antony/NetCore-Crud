using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetCoreCrud.Api.Controllers;
using NetCoreCrud.Api.Models;
using NetCoreCrud.Api.Repository;
using NetCoreCrud.Api.Services;
using NSubstitute;
using Serilog;
using Xunit;

namespace NetCoreCrud.Endpoint.Test
{
    public class ControllerTests : IDisposable
    {
        private readonly CustomerDbContext _context;
        private readonly CustomerController _controller;

        public ControllerTests()
        {
            var logger = Substitute.For<ILogger>();
            ICustomerService service = new CustomerService(logger);

            var options = new DbContextOptionsBuilder<CustomerDbContext>()
                .UseInMemoryDatabase("CustomerTest")
                .Options;

            _context = new CustomerDbContext(options);
            _context.Database.EnsureCreated();

            _controller = new CustomerController(logger, service, _context);

            SeedTestData(_context);
        }

        [Fact]
        public async Task GetAllCustomers_Success()
        {
            var result = (ObjectResult) await _controller.GetAll();

            var response = (List<Customer>) result.Value;
            response.Should().NotBeNull();
            response.Count.Should().Be(2);

        }

        [Fact]
        public async Task GetCustomerByKeyword_Success()
        {
            var result = (ObjectResult) await _controller.GetByName("Jord");
            var response = (List<Customer>) result.Value;
            response.Count.Should().Be(1);
        }

        [Fact]
        public async Task AddCustomer_NoData_Return400()
        {
            var result = (ObjectResult) await _controller.AddCustomer(null);
            result.StatusCode.Should().Be(400);
        }

        [Fact]
        public async Task AddACustomer_Success()
        {
            var newCustomer = new Customer()
            {
                FirstName = "John",
                LastName = "Bond",
                DateOfBirth = "22-Jan-1980"
            };

            var existingDbSet = _context.CustomerItems.ToListAsync();
            var existingRecordCount = existingDbSet.Result.Count;

            var addResult = (ObjectResult) await _controller.AddCustomer(newCustomer);
            var getResult = (ObjectResult) await _controller.GetAll();
            var getNewDbResult = (List<Customer>) getResult.Value;

            addResult.StatusCode.Should().Be(200);
            getNewDbResult.Count.Should().Be(existingRecordCount + 1);
        }

        [Fact]
        public async Task EditCustomer_Success()
        {
            var editCustomer = new Customer()
            {
                FirstName = "Jordan",
                LastName = "Peterson",
                DateOfBirth = "22-Jan-1980"
            };

            var editResult = (ObjectResult) await _controller.EditCustomer(editCustomer);
            var response = (CommonResponse) editResult.Value;

            editResult.StatusCode.Should().Be(200);
            response.Code.Should().Be(200);
        }

        [Fact]
        public async Task EditCustomer_NotExisting_ReturnFailMessage()
        {
            var editCustomer = new Customer()
            {
                FirstName = "Goat",
                LastName = "Peterson",
                DateOfBirth = "22-Jan-1980"
            };

            var editResult = (ObjectResult) await _controller.EditCustomer(editCustomer);
            var response = (CommonResponse) editResult.Value;

            editResult.StatusCode.Should().Be(200);
            response.Code.Should().Be(404);
        }

        [Fact]
        public async Task DeleteCustomer_Success()
        {
            var existingDbSet = _context.CustomerItems.ToListAsync();
            var existingRecordCount = existingDbSet.Result.Count;

            var deleteResult = (ObjectResult) await _controller.Delete("Jordan");
            var response = (CommonResponse) deleteResult.Value;

            var getResult = (ObjectResult) await _controller.GetAll();
            var getNewDbResult = (List<Customer>) getResult.Value;

            deleteResult.StatusCode.Should().Be(200);
            getNewDbResult.Count.Should().Be(existingRecordCount - 1);
        }

        private void SeedTestData(CustomerDbContext context)
        {
            var newCustomer1 = new Customer
            {
                FirstName = "Jordan",
                LastName = "Peterson",
                DateOfBirth = "12-May-1989"
            };
            var newCustomer2 = new Customer
            {
                FirstName = "Robert",
                LastName = "Langdon",
                DateOfBirth = "27-Oct-1985"
            };
            context.CustomerItems.Add(newCustomer1);
            context.CustomerItems.Add(newCustomer2);
            context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
