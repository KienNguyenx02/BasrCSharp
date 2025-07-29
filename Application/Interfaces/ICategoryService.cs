using System.Threading.Tasks;
using WebApplication1.Application.DTOs.Categories;
using WebApplication1.Shared.Results;

namespace WebApplication1.Application.Interfaces
{
    public interface ICategoryService
    {
        Task<PaginatedResult<CategoryDto>> GetCategoriesAsync(FilterParams filterParams);
        Task<CategoryDto> GetCategoryByIdAsync(Guid id);
        Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto createCategoryDto);
        Task<bool> UpdateCategoryAsync(Guid id, UpdateCategoryDto updateCategoryDto);
        Task<bool> DeleteCategoryAsync(Guid id);
    }
}