using Microsoft.EntityFrameworkCore;
using PracticeSession3.Context;
using PracticeSession3.Models;

namespace PracticeSession3.Services
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly CustomerContext _context;


        public CustomerRepository(CustomerContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));

        }


        public async Task Create(Customer customer)
        {
            await _context.Customers.AddAsync(customer);
        }



        public void Delete(Customer customer)
        {
            _context.Customers.Remove(customer);
        }

        public async Task<Customer> GetCustomerById(int id)
        {
            var customer = await _context.Customers.Include(x => x.Orders).FirstOrDefaultAsync(x => x.CustomerId == id);
            return customer;
        }

        public async Task<IEnumerable<Customer>> GetCustomers()
        {
            var customers = await _context.Customers.ToListAsync();

            return customers;
        }

        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }


    }
}
