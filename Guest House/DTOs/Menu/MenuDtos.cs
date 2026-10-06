using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Guest_House.DTOs.Menu
{
    // ==========================================
    // MENU CATEGORY DTOs
    // ==========================================

    public class CreateMenuCategoryDto
    {
        [Required(ErrorMessage = "Hotel ID is required.")]
        public int HotelId { get; set; }

        [Required(ErrorMessage = "Category name is required.")]
        [StringLength(100, ErrorMessage = "Category name cannot exceed 100 characters.")]
        public string CategoryName { get; set; } = null!;

        [StringLength(255, ErrorMessage = "Description cannot exceed 255 characters.")]
        public string? Description { get; set; }
    }

    public class UpdateMenuCategoryDto
    {
        [Required(ErrorMessage = "Category name is required.")]
        [StringLength(100, ErrorMessage = "Category name cannot exceed 100 characters.")]
        public string CategoryName { get; set; } = null!;

        [StringLength(255, ErrorMessage = "Description cannot exceed 255 characters.")]
        public string? Description { get; set; }
    }

    public class MenuCategoryResponseDto
    {
        public int MenuCategoryId { get; set; }
        public int HotelId { get; set; }
        public string CategoryName { get; set; } = null!;
        public string? Description { get; set; }
        public int ItemCount { get; set; }
    }

    // ==========================================
    // MENU ITEM DTOs
    // ==========================================

    public class CreateMenuItemDto
    {
        [Required(ErrorMessage = "Menu Category ID is required.")]
        public int MenuCategoryId { get; set; }

        [Required(ErrorMessage = "Item name is required.")]
        [StringLength(100, ErrorMessage = "Item name cannot exceed 100 characters.")]
        public string ItemName { get; set; } = null!;

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Price is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
        public decimal Price { get; set; }

        [StringLength(255, ErrorMessage = "Image URL cannot exceed 255 characters.")]
        public string? ImageUrl { get; set; }

        public bool IsAvailable { get; set; } = true;
    }

    public class UpdateMenuItemDto
    {
        [Required(ErrorMessage = "Menu Category ID is required.")]
        public int MenuCategoryId { get; set; }

        [Required(ErrorMessage = "Item name is required.")]
        [StringLength(100, ErrorMessage = "Item name cannot exceed 100 characters.")]
        public string ItemName { get; set; } = null!;

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Price is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
        public decimal Price { get; set; }

        [StringLength(255, ErrorMessage = "Image URL cannot exceed 255 characters.")]
        public string? ImageUrl { get; set; }

        public bool IsAvailable { get; set; }
    }

    public class MenuItemResponseDto
    {
        public int MenuItemId { get; set; }
        public int MenuCategoryId { get; set; }
        public string CategoryName { get; set; } = null!;
        public string ItemName { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsAvailable { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    // ==========================================
    // ROOM ORDER DTOs
    // ==========================================

    public class CreateRoomOrderItemRequestDto
    {
        [Required(ErrorMessage = "Menu item ID is required.")]
        public int MenuItemId { get; set; }

        [Required(ErrorMessage = "Quantity is required.")]
        [Range(1, 100, ErrorMessage = "Quantity must be between 1 and 100.")]
        public int Quantity { get; set; }

        [StringLength(255, ErrorMessage = "Special instruction cannot exceed 255 characters.")]
        public string? SpecialInstruction { get; set; }
    }

    public class CreateRoomOrderDto
    {
        [Required(ErrorMessage = "Stay ID is required.")]
        public int StayId { get; set; }

        [Required(ErrorMessage = "Room ID is required.")]
        public int RoomId { get; set; }

        [StringLength(255, ErrorMessage = "Notes cannot exceed 255 characters.")]
        public string? Notes { get; set; }

        [Required(ErrorMessage = "Order items are required.")]
        [MinLength(1, ErrorMessage = "At least one order item is required.")]
        public List<CreateRoomOrderItemRequestDto> Items { get; set; } = new();
    }

    public class UpdateRoomOrderDto
    {
        [StringLength(50, ErrorMessage = "Order status cannot exceed 50 characters.")]
        public string? OrderStatus { get; set; }

        [StringLength(255, ErrorMessage = "Notes cannot exceed 255 characters.")]
        public string? Notes { get; set; }
    }

    public class UpdateRoomOrderItemDto
    {
        [Required(ErrorMessage = "Quantity is required.")]
        [Range(1, 100, ErrorMessage = "Quantity must be between 1 and 100.")]
        public int Quantity { get; set; }

        [StringLength(255, ErrorMessage = "Special instruction cannot exceed 255 characters.")]
        public string? SpecialInstruction { get; set; }
    }

    public class RoomOrderItemResponseDto
    {
        public int OrderItemId { get; set; }
        public int OrderId { get; set; }
        public int MenuItemId { get; set; }
        public string MenuItemName { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public string? SpecialInstruction { get; set; }
    }

    public class RoomOrderResponseDto
    {
        public int OrderId { get; set; }
        public int StayId { get; set; }
        public int RoomId { get; set; }
        public string RoomNumber { get; set; } = null!;
        public string OrderNumber { get; set; } = null!;
        public string OrderStatus { get; set; } = null!;
        public string? Notes { get; set; }
        public DateTime OrderedAt { get; set; }
        public decimal GrandTotal { get; set; }
        public List<RoomOrderItemResponseDto> Items { get; set; } = new();
    }
}
