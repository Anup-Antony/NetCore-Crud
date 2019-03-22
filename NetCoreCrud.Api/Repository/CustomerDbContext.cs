using Microsoft.EntityFrameworkCore;
using NetCoreCrud.Api.Models;

namespace NetCoreCrud.Api.Repository
{
    public class CustomerDbContext : DbContext
    {
        public CustomerDbContext(DbContextOptions<CustomerDbContext> options) : base(options)
        {
            
        }

        public DbSet<Customer> CustomerItems { get; set; }
    }
}
