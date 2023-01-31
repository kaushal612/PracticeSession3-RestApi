using Microsoft.EntityFrameworkCore;
using PracticeSession3.Models;

namespace PracticeSession3.Context
{
    public class CustomerContext : DbContext
    {
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }

        public CustomerContext(DbContextOptions options)
            : base(options)
        {

        }

    }
}
