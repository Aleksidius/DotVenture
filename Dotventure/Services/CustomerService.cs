using Dotventure.Models;
using Dotventure.Services.DTO;
using Dotventure.Services.Interfaces;
using Microsoft.EntityFrameworkCore;


namespace Dotventure.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly AdventureWorksLt2022Context _context;

        public CustomerService(AdventureWorksLt2022Context context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CustomerDto>> GetAllAsync() =>
            await _context.Customers
                .Select(c => new CustomerDto { 
                
                    CustomerId = c.CustomerId,
                    EmailAddress = c.EmailAddress,
                    FullName = $"{c.FirstName} {c.LastName}",
                })
                .ToListAsync();

        public async Task<CustomerDto?> GetByIdAsync(int id)
        {
            var customer = await _context.Customers.FindAsync(id);

            if(customer == null)
            {
                return null;
            }

            return new CustomerDto
            {
                CustomerId = customer.CustomerId,
                EmailAddress = customer.EmailAddress,
                FullName = $"{customer.FirstName} {customer.LastName}",
            };
        }
            

    }

}
