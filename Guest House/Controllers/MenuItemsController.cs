using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Guest_House.DTOs.Common;
using Guest_House.DTOs.Menu;
using Guest_House.Services.Menu;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Guest_House.Controllers
{
    [ApiController]
    [Route("api/menu-items")]
    [Authorize]
    public class MenuItemsController : ControllerBase
    {
        private readonly IMenuItemService _menuItemService;

        public MenuItemsController(IMenuItemService menuItemService)
        {
            _menuItemService = menuItemService;
        }

        /// <summary>
        /// Retrieves menu items with optional category, availability, or hotel filters
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<List<MenuItemResponseDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(
            [FromQuery] int? categoryId,
            [FromQuery] bool? isAvailable,
            [FromQuery] int? hotelId,
            CancellationToken cancellationToken)
        {
            var data = await _menuItemService.GetAllAsync(categoryId, isAvailable, hotelId, cancellationToken);
            return Ok(ApiResponse<List<MenuItemResponseDto>>.SuccessResponse(data, "Menu items retrieved successfully."));
        }

        /// <summary>
        /// Retrieves a specific menu item by ID
        /// </summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<MenuItemResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            var data = await _menuItemService.GetByIdAsync(id, cancellationToken);
            return Ok(ApiResponse<MenuItemResponseDto>.SuccessResponse(data, "Menu item retrieved successfully."));
        }

        /// <summary>
        /// Creates a new menu item
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        [ProducesResponseType(typeof(ApiResponse<MenuItemResponseDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Create([FromBody] CreateMenuItemDto dto, CancellationToken cancellationToken)
        {
            var data = await _menuItemService.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = data.MenuItemId },
                ApiResponse<MenuItemResponseDto>.SuccessResponse(data, "Menu item created successfully."));
        }

        /// <summary>
        /// Updates an existing menu item
        /// </summary>
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin,Manager")]
        [ProducesResponseType(typeof(ApiResponse<MenuItemResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateMenuItemDto dto, CancellationToken cancellationToken)
        {
            var data = await _menuItemService.UpdateAsync(id, dto, cancellationToken);
            return Ok(ApiResponse<MenuItemResponseDto>.SuccessResponse(data, "Menu item updated successfully."));
        }

        /// <summary>
        /// Deletes or deactivates a menu item
        /// </summary>
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin,Manager")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            await _menuItemService.DeleteAsync(id, cancellationToken);
            return Ok(ApiResponse.SuccessResult("Menu item deleted successfully."));
        }
    }
}
