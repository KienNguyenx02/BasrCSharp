using System.Threading.Tasks;
using WebApplication1.Application.DTOs.Products;
using WebApplication1.Shared.Results;

namespace WebApplication1.Application.Interfaces
{
    public interface IProductService
    {
        Task<PaginatedResult<ProductDto>> GetProductsAsync(FilterParams filterParams);
        Task<ProductDto> GetProductByIdAsync(Guid id);
        Task<ProductDto> CreateProductAsync(CreateProductDto createProductDto);
        Task<bool> UpdateProductAsync(Guid id, UpdateProductDto updateProductDto);
        Task<bool> DeleteProductAsync(Guid id);
    }
}