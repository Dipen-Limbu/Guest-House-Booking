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
    [Route("api/menu-categories")]
    [Authorize]
    public class MenuCategoriesController : ControllerBase
    {
        private readonly IMenuCategoryService _categoryService;

        public MenuCategoriesController(IMenuCategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        /// <summary>
        /// Retrieves menu categories
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<List<MenuCategoryResponseDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromQuery] int? hotelId, CancellationToken cancellationToken)
        {
            var data = await _categoryService.GetAllAsync(hotelId, cancellationToken);
            return Ok(ApiResponse<List<MenuCategoryResponseDto>>.SuccessResponse(data, "Menu categories retrieved successfully."));
        }

        /// <summary>
        /// Retrieves a specific menu category by ID
        /// </summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<MenuCategoryResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            var data = await _categoryService.GetByIdAsync(id, cancellationToken);
            return Ok(ApiResponse<MenuCategoryResponseDto>.SuccessResponse(data, "Menu category retrieved successfully."));
        }

        /// <summary>
        /// Creates a new menu category
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        [ProducesResponseType(typeof(ApiResponse<MenuCategoryResponseDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Create([FromBody] CreateMenuCategoryDto dto, CancellationToken cancellationToken)
        {
            var data = await _categoryService.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = data.MenuCategoryId },
                ApiResponse<MenuCategoryResponseDto>.SuccessResponse(data, "Menu category created successfully."));
        }

        /// <summary>
        /// Updates an existing menu category
        /// </summary>
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin,Manager")]
        [ProducesResponseType(typeof(ApiResponse<MenuCategoryResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateMenuCategoryDto dto, CancellationToken cancellationToken)
        {
            var data = await _categoryService.UpdateAsync(id, dto, cancellationToken);
            return Ok(ApiResponse<MenuCategoryResponseDto>.SuccessResponse(data, "Menu category updated successfully."));
        }

        /// <summary>
        /// Deletes a menu category
        /// </summary>
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin,Manager")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            await _categoryService.DeleteAsync(id, cancellationToken);
            return Ok(ApiResponse.SuccessResult("Menu category deleted successfully."));
        }
    }
}
