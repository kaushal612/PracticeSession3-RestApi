using PracticeSession3.Models;

namespace PracticeSession3.Services
{
    public interface ICustomerRepository
    {
        Task<IEnumerable<Customer>> GetCustomers();
        Task<Customer> GetCustomerById(int id);

        Task Create(Customer customer);
        void Delete(Customer customer);
        Task Save();

    }
}
