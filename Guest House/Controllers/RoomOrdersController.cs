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
    [Route("api/room-orders")]
    [Authorize]
    public class RoomOrdersController : ControllerBase
    {
        private readonly IRoomOrderService _roomOrderService;

        public RoomOrdersController(IRoomOrderService roomOrderService)
        {
            _roomOrderService = roomOrderService;
        }

        /// <summary>
        /// Creates a new food/service room order with items
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin,Manager,Staff")]
        [ProducesResponseType(typeof(ApiResponse<RoomOrderResponseDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Create([FromBody] CreateRoomOrderDto dto, CancellationToken cancellationToken)
        {
            var data = await _roomOrderService.CreateOrderAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = data.OrderId },
                ApiResponse<RoomOrderResponseDto>.SuccessResponse(data, "Room order created successfully."));
        }

        /// <summary>
        /// Retrieves room orders with optional filters
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin,Manager,Staff")]
        [ProducesResponseType(typeof(ApiResponse<List<RoomOrderResponseDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(
            [FromQuery] int? stayId,
            [FromQuery] int? roomId,
            [FromQuery] string? orderStatus,
            CancellationToken cancellationToken)
        {
            var data = await _roomOrderService.GetOrdersAsync(stayId, roomId, orderStatus, cancellationToken);
            return Ok(ApiResponse<List<RoomOrderResponseDto>>.SuccessResponse(data, "Room orders retrieved successfully."));
        }

        /// <summary>
        /// Retrieves a specific room order by ID
        /// </summary>
        [HttpGet("{id:int}")]
        [Authorize(Roles = "Admin,Manager,Staff")]
        [ProducesResponseType(typeof(ApiResponse<RoomOrderResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            var data = await _roomOrderService.GetOrderByIdAsync(id, cancellationToken);
            return Ok(ApiResponse<RoomOrderResponseDto>.SuccessResponse(data, "Room order retrieved successfully."));
        }

        /// <summary>
        /// Retrieves all room orders for a specific room
        /// </summary>
        [HttpGet("room/{roomId:int}")]
        [Authorize(Roles = "Admin,Manager,Staff")]
        [ProducesResponseType(typeof(ApiResponse<List<RoomOrderResponseDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByRoomId(int roomId, CancellationToken cancellationToken)
        {
            var data = await _roomOrderService.GetOrdersByRoomIdAsync(roomId, cancellationToken);
            return Ok(ApiResponse<List<RoomOrderResponseDto>>.SuccessResponse(data, "Room orders retrieved successfully."));
        }

        /// <summary>
        /// Updates the status or notes of a room order
        /// </summary>
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin,Manager,Staff")]
        [ProducesResponseType(typeof(ApiResponse<RoomOrderResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateRoomOrderDto dto, CancellationToken cancellationToken)
        {
            var data = await _roomOrderService.UpdateOrderStatusAsync(id, dto, cancellationToken);
            return Ok(ApiResponse<RoomOrderResponseDto>.SuccessResponse(data, "Room order updated successfully."));
        }

        /// <summary>
        /// Deletes a room order
        /// </summary>
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin,Manager")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            await _roomOrderService.DeleteOrderAsync(id, cancellationToken);
            return Ok(ApiResponse.SuccessResult("Room order deleted successfully."));
        }

        // ==========================================
        // ROOM ORDER ITEMS MANAGEMENT
        // ==========================================

        /// <summary>
        /// Adds a new menu item to an existing room order
        /// </summary>
        [HttpPost("{orderId:int}/items")]
        [Authorize(Roles = "Admin,Manager,Staff")]
        [ProducesResponseType(typeof(ApiResponse<RoomOrderResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddItem(int orderId, [FromBody] CreateRoomOrderItemRequestDto dto, CancellationToken cancellationToken)
        {
            var data = await _roomOrderService.AddOrderItemAsync(orderId, dto, cancellationToken);
            return Ok(ApiResponse<RoomOrderResponseDto>.SuccessResponse(data, "Item added to room order successfully."));
        }

        /// <summary>
        /// Updates quantity or special instructions for an item in a room order
        /// </summary>
        [HttpPut("{orderId:int}/items/{itemId:int}")]
        [Authorize(Roles = "Admin,Manager,Staff")]
        [ProducesResponseType(typeof(ApiResponse<RoomOrderResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateItem(int orderId, int itemId, [FromBody] UpdateRoomOrderItemDto dto, CancellationToken cancellationToken)
        {
            var data = await _roomOrderService.UpdateOrderItemAsync(orderId, itemId, dto, cancellationToken);
            return Ok(ApiResponse<RoomOrderResponseDto>.SuccessResponse(data, "Order item updated successfully."));
        }

        /// <summary>
        /// Removes an item from a room order
        /// </summary>
        [HttpDelete("{orderId:int}/items/{itemId:int}")]
        [Authorize(Roles = "Admin,Manager,Staff")]
        [ProducesResponseType(typeof(ApiResponse<RoomOrderResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemoveItem(int orderId, int itemId, CancellationToken cancellationToken)
        {
            var data = await _roomOrderService.RemoveOrderItemAsync(orderId, itemId, cancellationToken);
            return Ok(ApiResponse<RoomOrderResponseDto>.SuccessResponse(data, "Item removed from room order successfully."));
        }
    }
}
