using Azure;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using PracticeSession3.Context;
using PracticeSession3.Models;
using PracticeSession3.Services;

namespace PracticeSession3.Controllers
{
    [ApiController]
    [Route("api/Customers")]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerRepository _customerRepository;
        public CustomersController(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }
        [HttpGet()]
        public async Task<IActionResult> GetCustomers()
        {

            var customers = await _customerRepository.GetCustomers();

            return Ok(customers);

        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCustomer(int id)
        {

            var customer = await _customerRepository.GetCustomerById(id);

            if (customer == null)
                return NotFound("customer not exist");


            return Ok(customer);

        }

        [HttpGet("{id}/orders")]
        public async Task<IActionResult> GetCustomerOrders(int id)
        {

            var customer = await _customerRepository.GetCustomerById(id);

            if (customer == null)
                return NotFound();


            return Ok(customer.Orders);

        }

        [HttpPost]
        public async Task<IActionResult> PostCustomer(Customer customer)
        {
            await _customerRepository.Create(customer);
            await _customerRepository.Save();
            return CreatedAtAction("GetCustomer", new { id = customer.CustomerId }, customer);
        }

        [HttpPost("{id}/Orders")]
        public async Task<IActionResult> PostOrder(int id, Order order)
        {
            var customer = await _customerRepository.GetCustomerById(id);
            if (customer == null) return NotFound();

            customer.Orders.Add(order);
            await _customerRepository.Save();

            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCustomer(int id, [FromBody] Customer customer)
        {
            var oldCustomer = await _customerRepository.GetCustomerById(id);
            if (oldCustomer == null)
                return NotFound();

            oldCustomer.Name = customer.Name;
            oldCustomer.Email = customer.Email;
            oldCustomer.Orders = customer.Orders;

            await _customerRepository.Save();
            return Ok(oldCustomer);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchCustomer(int id, [FromBody] JsonPatchDocument<Customer> customerDocument)
        {
            if (customerDocument == null)
                return BadRequest();

            var customer = await _customerRepository.GetCustomerById(id);

            if (customer == null) return NotFound();

            customerDocument.ApplyTo(customer);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _customerRepository.Save();
            return new ObjectResult(customer);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var customer = await _customerRepository.GetCustomerById(id);
            if (customer == null)
                return NotFound();

            _customerRepository.Delete(customer);
            await _customerRepository.Save();

            return Ok();
        }

        [HttpDelete("{id}/orders/{orderId}")]
        public async Task<IActionResult> DeleteOrder(int id, int orderId)
        {
            var customer = await _customerRepository.GetCustomerById(id);

            if (customer == null)
                return NotFound();

            var order = customer.Orders.FirstOrDefault(x => x.OrderId == orderId);

            if (order == null)
                return NotFound("order does not exist");

            customer.Orders.Remove(order);
            await _customerRepository.Save();
            return Ok();
        }
    }
}
