using System.Threading.Tasks;
using WebApplication1.Application.DTOs.Orders;
using WebApplication1.Shared.Results;

namespace WebApplication1.Application.Interfaces
{
    public interface IOrderService
    {
        Task<PaginatedResult<OrderDto>> GetOrdersAsync(FilterParams filterParams);
        Task<OrderDto> GetOrderByIdAsync(int id);
        Task<OrderDto> CreateOrderAsync(string userId, CreateOrderDto createOrderDto);
        Task<bool> UpdateOrderStatusAsync(int id, UpdateOrderDto updateOrderDto);
        Task<bool> DeleteOrderAsync(int id);
    }
}