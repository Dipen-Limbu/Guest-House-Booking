using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Guest_House.DTOs.Menu;

namespace Guest_House.Services.Menu
{
    public interface IMenuItemService
    {
        Task<List<MenuItemResponseDto>> GetAllAsync(int? categoryId, bool? isAvailable, int? hotelId, CancellationToken cancellationToken = default);
        Task<MenuItemResponseDto> GetByIdAsync(int menuItemId, CancellationToken cancellationToken = default);
        Task<MenuItemResponseDto> CreateAsync(CreateMenuItemDto dto, CancellationToken cancellationToken = default);
        Task<MenuItemResponseDto> UpdateAsync(int menuItemId, UpdateMenuItemDto dto, CancellationToken cancellationToken = default);
        Task DeleteAsync(int menuItemId, CancellationToken cancellationToken = default);
    }
}
