using System.Collections.Generic;
using System.Threading.Tasks;
using NetCoreCrud.Api.Models;
using NetCoreCrud.Api.Repository;

namespace NetCoreCrud.Api.Services
{
    public interface ICustomerService
    {
        Task<IEnumerable<Customer>> GetCustomerByPartialName(CustomerDbContext context, string nameString);
        Task<bool> AddCustomer(CustomerDbContext context, Customer customer);
        Task<string> DeleteCustomerByFirstName(CustomerDbContext context, string name);
        Task<Customer> EditCustomerByFirstName(CustomerDbContext context, Customer customer);
        void SeedSampleData(CustomerDbContext context);
        Task<IEnumerable<Customer>> GetAllCustomers(CustomerDbContext context);
    }
}
