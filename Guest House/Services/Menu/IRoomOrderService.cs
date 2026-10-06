using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Guest_House.DTOs.Menu;

namespace Guest_House.Services.Menu
{
    public interface IRoomOrderService
    {
        Task<RoomOrderResponseDto> CreateOrderAsync(CreateRoomOrderDto dto, CancellationToken cancellationToken = default);
        Task<List<RoomOrderResponseDto>> GetOrdersAsync(int? stayId, int? roomId, string? orderStatus, CancellationToken cancellationToken = default);
        Task<RoomOrderResponseDto> GetOrderByIdAsync(int orderId, CancellationToken cancellationToken = default);
        Task<List<RoomOrderResponseDto>> GetOrdersByRoomIdAsync(int roomId, CancellationToken cancellationToken = default);
        Task<RoomOrderResponseDto> UpdateOrderStatusAsync(int orderId, UpdateRoomOrderDto dto, CancellationToken cancellationToken = default);
        Task DeleteOrderAsync(int orderId, CancellationToken cancellationToken = default);

        Task<RoomOrderResponseDto> AddOrderItemAsync(int orderId, CreateRoomOrderItemRequestDto dto, CancellationToken cancellationToken = default);
        Task<RoomOrderResponseDto> UpdateOrderItemAsync(int orderId, int orderItemId, UpdateRoomOrderItemDto dto, CancellationToken cancellationToken = default);
        Task<RoomOrderResponseDto> RemoveOrderItemAsync(int orderId, int orderItemId, CancellationToken cancellationToken = default);
    }
}
