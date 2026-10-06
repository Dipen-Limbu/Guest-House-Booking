using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Guest_House.DTOs.Menu;

namespace Guest_House.Services.Menu
{
    public interface IMenuCategoryService
    {
        Task<List<MenuCategoryResponseDto>> GetAllAsync(int? hotelId, CancellationToken cancellationToken = default);
        Task<MenuCategoryResponseDto> GetByIdAsync(int categoryId, CancellationToken cancellationToken = default);
        Task<MenuCategoryResponseDto> CreateAsync(CreateMenuCategoryDto dto, CancellationToken cancellationToken = default);
        Task<MenuCategoryResponseDto> UpdateAsync(int categoryId, UpdateMenuCategoryDto dto, CancellationToken cancellationToken = default);
        Task DeleteAsync(int categoryId, CancellationToken cancellationToken = default);
    }
}
