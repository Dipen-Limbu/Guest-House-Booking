using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Guest_House.Data;
using Guest_House.DTOs.Menu;
using Guest_House.Models;
using Microsoft.EntityFrameworkCore;
using RoomOrderEntity = Guest_House.Models.RoomOrder;
using RoomOrderItemEntity = Guest_House.Models.RoomOrderItem;

namespace Guest_House.Services.Menu
{
    public class RoomOrderService : IRoomOrderService
    {
        private readonly GuestHouseContext _context;

        public RoomOrderService(GuestHouseContext context)
        {
            _context = context;
        }

        public async Task<RoomOrderResponseDto> CreateOrderAsync(CreateRoomOrderDto dto, CancellationToken cancellationToken = default)
        {
            var stay = await _context.Stays
                .FirstOrDefaultAsync(s => s.StayId == dto.StayId, cancellationToken)
                ?? throw new KeyNotFoundException($"Stay record with ID {dto.StayId} was not found.");

            if (stay.StayStatus.ToLower() != "active")
                throw new InvalidOperationException($"Cannot create room order for stay #{dto.StayId} because stay status is '{stay.StayStatus}'. Stay must be active.");

            var room = await _context.Rooms
                .FirstOrDefaultAsync(r => r.RoomId == dto.RoomId, cancellationToken)
                ?? throw new KeyNotFoundException($"Room with ID {dto.RoomId} was not found.");

            if (!dto.Items.Any())
                throw new ArgumentException("Order must contain at least one menu item.");

            var itemIds = dto.Items.Select(i => i.MenuItemId).Distinct().ToList();
            var menuItems = await _context.MenuItems
                .Where(m => itemIds.Contains(m.MenuItemId))
                .ToDictionaryAsync(m => m.MenuItemId, cancellationToken);

            foreach (var itemReq in dto.Items)
            {
                if (!menuItems.TryGetValue(itemReq.MenuItemId, out var menuItem))
                    throw new KeyNotFoundException($"Menu item with ID {itemReq.MenuItemId} was not found.");

                if (!menuItem.IsAvailable)
                    throw new InvalidOperationException($"Menu item '{menuItem.ItemName}' is currently unavailable.");

                if (itemReq.Quantity <= 0)
                    throw new ArgumentException($"Quantity for '{menuItem.ItemName}' must be greater than zero.");
            }

            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                var now = DateTime.Now;
                var orderNumber = $"ORD-{now:yyyyMMddHHmmss}-{Random.Shared.Next(100, 999)}";

                var order = new RoomOrderEntity
                {
                    StayId = dto.StayId,
                    RoomId = dto.RoomId,
                    OrderNumber = orderNumber,
                    OrderStatus = "pending",
                    Notes = dto.Notes?.Trim(),
                    OrderedAt = now
                };

                _context.RoomOrders.Add(order);
                await _context.SaveChangesAsync(cancellationToken);

                foreach (var itemReq in dto.Items)
                {
                    var menuItem = menuItems[itemReq.MenuItemId];
                    var orderItem = new RoomOrderItemEntity
                    {
                        OrderId = order.OrderId,
                        MenuItemId = itemReq.MenuItemId,
                        Quantity = itemReq.Quantity,
                        UnitPrice = menuItem.Price,
                        TotalPrice = menuItem.Price * itemReq.Quantity,
                        SpecialInstruction = itemReq.SpecialInstruction?.Trim()
                    };
                    _context.RoomOrderItems.Add(orderItem);
                }

                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                return await GetOrderByIdAsync(order.OrderId, cancellationToken);
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }

        public async Task<List<RoomOrderResponseDto>> GetOrdersAsync(
            int? stayId,
            int? roomId,
            string? orderStatus,
            CancellationToken cancellationToken = default)
        {
            var query = _context.RoomOrders
                .AsNoTracking()
                .Include(o => o.Room)
                .Include(o => o.RoomOrderItems)
                    .ThenInclude(roi => roi.MenuItem)
                .AsQueryable();

            if (stayId.HasValue)
                query = query.Where(o => o.StayId == stayId.Value);

            if (roomId.HasValue)
                query = query.Where(o => o.RoomId == roomId.Value);

            if (!string.IsNullOrWhiteSpace(orderStatus))
                query = query.Where(o => o.OrderStatus.ToLower() == orderStatus.Trim().ToLower());

            var orders = await query
                .OrderByDescending(o => o.OrderedAt)
                .ToListAsync(cancellationToken);

            return orders.Select(ToDto).ToList();
        }

        public async Task<RoomOrderResponseDto> GetOrderByIdAsync(int orderId, CancellationToken cancellationToken = default)
        {
            var order = await _context.RoomOrders
                .AsNoTracking()
                .Include(o => o.Room)
                .Include(o => o.RoomOrderItems)
                    .ThenInclude(roi => roi.MenuItem)
                .FirstOrDefaultAsync(o => o.OrderId == orderId, cancellationToken)
                ?? throw new KeyNotFoundException($"Room order with ID {orderId} was not found.");

            return ToDto(order);
        }

        public async Task<List<RoomOrderResponseDto>> GetOrdersByRoomIdAsync(int roomId, CancellationToken cancellationToken = default)
        {
            return await GetOrdersAsync(null, roomId, null, cancellationToken);
        }

        public async Task<RoomOrderResponseDto> UpdateOrderStatusAsync(int orderId, UpdateRoomOrderDto dto, CancellationToken cancellationToken = default)
        {
            var order = await _context.RoomOrders
                .FirstOrDefaultAsync(o => o.OrderId == orderId, cancellationToken)
                ?? throw new KeyNotFoundException($"Room order with ID {orderId} was not found.");

            if (!string.IsNullOrWhiteSpace(dto.OrderStatus))
            {
                var validStatuses = new[] { "pending", "preparing", "delivered", "cancelled" };
                var status = dto.OrderStatus.Trim().ToLower();
                if (!validStatuses.Contains(status))
                    throw new ArgumentException($"Invalid order status '{dto.OrderStatus}'. Allowed: {string.Join(", ", validStatuses)}.");

                order.OrderStatus = status;
            }

            if (dto.Notes != null)
                order.Notes = dto.Notes.Trim();

            await _context.SaveChangesAsync(cancellationToken);

            return await GetOrderByIdAsync(orderId, cancellationToken);
        }

        public async Task DeleteOrderAsync(int orderId, CancellationToken cancellationToken = default)
        {
            var order = await _context.RoomOrders
                .Include(o => o.RoomOrderItems)
                .FirstOrDefaultAsync(o => o.OrderId == orderId, cancellationToken)
                ?? throw new KeyNotFoundException($"Room order with ID {orderId} was not found.");

            if (order.OrderStatus.ToLower() == "delivered")
                throw new InvalidOperationException("Cannot delete a delivered room order.");

            _context.RoomOrderItems.RemoveRange(order.RoomOrderItems);
            _context.RoomOrders.Remove(order);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<RoomOrderResponseDto> AddOrderItemAsync(int orderId, CreateRoomOrderItemRequestDto dto, CancellationToken cancellationToken = default)
        {
            var order = await _context.RoomOrders
                .Include(o => o.RoomOrderItems)
                .FirstOrDefaultAsync(o => o.OrderId == orderId, cancellationToken)
                ?? throw new KeyNotFoundException($"Room order with ID {orderId} was not found.");

            if (order.OrderStatus.ToLower() == "delivered" || order.OrderStatus.ToLower() == "cancelled")
                throw new InvalidOperationException($"Cannot modify items on order #{orderId} because order status is '{order.OrderStatus}'.");

            var menuItem = await _context.MenuItems
                .FirstOrDefaultAsync(m => m.MenuItemId == dto.MenuItemId, cancellationToken)
                ?? throw new KeyNotFoundException($"Menu item with ID {dto.MenuItemId} was not found.");

            if (!menuItem.IsAvailable)
                throw new InvalidOperationException($"Menu item '{menuItem.ItemName}' is currently unavailable.");

            if (dto.Quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero.");

            var existingItem = order.RoomOrderItems.FirstOrDefault(roi => roi.MenuItemId == dto.MenuItemId);
            if (existingItem != null)
            {
                existingItem.Quantity += dto.Quantity;
                existingItem.TotalPrice = existingItem.UnitPrice * existingItem.Quantity;
                if (!string.IsNullOrWhiteSpace(dto.SpecialInstruction))
                    existingItem.SpecialInstruction = dto.SpecialInstruction.Trim();
            }
            else
            {
                var orderItem = new RoomOrderItemEntity
                {
                    OrderId = orderId,
                    MenuItemId = dto.MenuItemId,
                    Quantity = dto.Quantity,
                    UnitPrice = menuItem.Price,
                    TotalPrice = menuItem.Price * dto.Quantity,
                    SpecialInstruction = dto.SpecialInstruction?.Trim()
                };
                _context.RoomOrderItems.Add(orderItem);
            }

            await _context.SaveChangesAsync(cancellationToken);

            return await GetOrderByIdAsync(orderId, cancellationToken);
        }

        public async Task<RoomOrderResponseDto> UpdateOrderItemAsync(int orderId, int orderItemId, UpdateRoomOrderItemDto dto, CancellationToken cancellationToken = default)
        {
            var orderItem = await _context.RoomOrderItems
                .Include(roi => roi.Order)
                .FirstOrDefaultAsync(roi => roi.OrderItemId == orderItemId && roi.OrderId == orderId, cancellationToken)
                ?? throw new KeyNotFoundException($"Order item with ID {orderItemId} was not found in order #{orderId}.");

            if (orderItem.Order.OrderStatus.ToLower() == "delivered" || orderItem.Order.OrderStatus.ToLower() == "cancelled")
                throw new InvalidOperationException($"Cannot modify items on order #{orderId} because order status is '{orderItem.Order.OrderStatus}'.");

            if (dto.Quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero.");

            orderItem.Quantity = dto.Quantity;
            orderItem.TotalPrice = orderItem.UnitPrice * dto.Quantity;

            if (dto.SpecialInstruction != null)
                orderItem.SpecialInstruction = dto.SpecialInstruction.Trim();

            await _context.SaveChangesAsync(cancellationToken);

            return await GetOrderByIdAsync(orderId, cancellationToken);
        }

        public async Task<RoomOrderResponseDto> RemoveOrderItemAsync(int orderId, int orderItemId, CancellationToken cancellationToken = default)
        {
            var orderItem = await _context.RoomOrderItems
                .Include(roi => roi.Order)
                .FirstOrDefaultAsync(roi => roi.OrderItemId == orderItemId && roi.OrderId == orderId, cancellationToken)
                ?? throw new KeyNotFoundException($"Order item with ID {orderItemId} was not found in order #{orderId}.");

            if (orderItem.Order.OrderStatus.ToLower() == "delivered" || orderItem.Order.OrderStatus.ToLower() == "cancelled")
                throw new InvalidOperationException($"Cannot modify items on order #{orderId} because order status is '{orderItem.Order.OrderStatus}'.");

            _context.RoomOrderItems.Remove(orderItem);
            await _context.SaveChangesAsync(cancellationToken);

            return await GetOrderByIdAsync(orderId, cancellationToken);
        }

        private static RoomOrderResponseDto ToDto(RoomOrderEntity o)
        {
            var itemDtos = o.RoomOrderItems?.Select(roi => new RoomOrderItemResponseDto
            {
                OrderItemId = roi.OrderItemId,
                OrderId = roi.OrderId,
                MenuItemId = roi.MenuItemId,
                MenuItemName = roi.MenuItem?.ItemName ?? string.Empty,
                Quantity = roi.Quantity,
                UnitPrice = roi.UnitPrice,
                TotalPrice = roi.TotalPrice,
                SpecialInstruction = roi.SpecialInstruction
            }).ToList() ?? new List<RoomOrderItemResponseDto>();

            var grandTotal = itemDtos.Sum(i => i.TotalPrice);

            return new RoomOrderResponseDto
            {
                OrderId = o.OrderId,
                StayId = o.StayId,
                RoomId = o.RoomId,
                RoomNumber = o.Room?.RoomNumber ?? string.Empty,
                OrderNumber = o.OrderNumber,
                OrderStatus = o.OrderStatus,
                Notes = o.Notes,
                OrderedAt = o.OrderedAt,
                GrandTotal = grandTotal,
                Items = itemDtos
            };
        }
    }
}
