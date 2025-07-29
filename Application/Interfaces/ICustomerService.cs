using System.Threading.Tasks;
using WebApplication1.Application.DTOs.Customers;
using WebApplication1.Shared.Results;

namespace WebApplication1.Application.Interfaces
{
    public interface ICustomerService
    {
        Task<PaginatedResult<CustomerDto>> GetCustomersAsync(FilterParams filterParams);
        Task<CustomerDto> GetCustomerByIdAsync(Guid id);
        Task<CustomerDto> CreateCustomerAsync(CreateCustomerDto createCustomerDto);
        Task<bool> UpdateCustomerAsync(Guid id, UpdateCustomerDto updateCustomerDto);
        Task<bool> DeleteCustomerAsync(Guid id);
    }
}