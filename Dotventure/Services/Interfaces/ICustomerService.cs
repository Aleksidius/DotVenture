using Dotventure.Models;
using Dotventure.Services.DTO;

namespace Dotventure.Services.Interfaces
{
    public interface ICustomerService
    {
        Task<IEnumerable<CustomerDto>> GetAllAsync();
        Task<CustomerDto?> GetByIdAsync(int id);

    }
}
