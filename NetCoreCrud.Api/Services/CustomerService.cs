using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NetCoreCrud.Api.Models;
using NetCoreCrud.Api.Repository;
using Serilog;

namespace NetCoreCrud.Api.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ILogger _logger;

        public CustomerService(ILogger logger)
        {
            _logger = logger;
        }

        public async Task<IEnumerable<Customer>> GetAllCustomers(CustomerDbContext context)
        {
            var allCustomers = await context.CustomerItems.ToListAsync();

            return allCustomers;
        }

        public async Task<IEnumerable<Customer>> GetCustomerByPartialName(CustomerDbContext context, string nameString)
        {
            var matchingCustomers = await context.CustomerItems
                .Where(c => c.FirstName.ToLower().Contains(nameString.ToLower()) ||
                            c.LastName.ToLower().Contains(nameString.ToLower())).ToListAsync();

            return matchingCustomers;
        }

        public async Task<bool> AddCustomer(CustomerDbContext context, Customer customer)
        {
            var hasExisting = CheckIfRecordAlreadyExists(context.CustomerItems.ToList(), customer.FirstName);
            if (hasExisting)
                return false;
            context.Add(customer);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<Customer> EditCustomerByFirstName(CustomerDbContext context, Customer inputCustomer)
        {
            var allCustomers = await GetAllCustomers(context);
            var customer = allCustomers.FirstOrDefault(c =>
                c.FirstName.Equals(inputCustomer.FirstName, StringComparison.OrdinalIgnoreCase));
            if (customer == null)
                return null;

            customer.FirstName = inputCustomer.FirstName;
            customer.LastName = inputCustomer.LastName;
            customer.DateOfBirth = inputCustomer.DateOfBirth;

            context.CustomerItems.Update(customer);
            context.SaveChanges();

            return customer;
        }

        public async Task<string> DeleteCustomerByFirstName(CustomerDbContext context, string name)
        {
            var allCustomers = await GetAllCustomers(context);
            var customer =
                allCustomers.FirstOrDefault(c => c.FirstName.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (customer == null)
                return null;

            context.CustomerItems.Remove(customer);
            context.SaveChanges();

            return name;
        }

        private bool CheckIfRecordAlreadyExists(List<Customer> existingCustomers, string firstName)
        {
            return existingCustomers.FirstOrDefault(customer =>
                       customer.FirstName.Equals(firstName, StringComparison.OrdinalIgnoreCase)) != null;
        }

        public void SeedSampleData(CustomerDbContext context)
        {
            var newCustomer1 = new Customer
            {
                FirstName = "Topsy",
                LastName = "Kretts",
                DateOfBirth = "12-May-1989"
            };
            var newCustomer2 = new Customer
            {
                FirstName = "Top",
                LastName = "Secrets",
                DateOfBirth = "27-Oct-1985"
            };
            context.CustomerItems.Add(newCustomer1);
            context.CustomerItems.Add(newCustomer2);
            context.SaveChanges();
        }
    }
}
