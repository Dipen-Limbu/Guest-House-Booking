using System;
using System.Collections.Generic;
using Guest_House.Models;
using Microsoft.EntityFrameworkCore;

namespace Guest_House.Data;

public partial class GuestHouseContext : DbContext
{
    public GuestHouseContext()
    {
    }

    public GuestHouseContext(DbContextOptions<GuestHouseContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<BookingRoom> BookingRooms { get; set; }

    public virtual DbSet<ExpenseCharge> ExpenseCharges { get; set; }

    public virtual DbSet<Guest> Guests { get; set; }

    public virtual DbSet<Hotel> Hotels { get; set; }

    public virtual DbSet<HotelExpense> HotelExpenses { get; set; }

    public virtual DbSet<Invoice> Invoices { get; set; }

    public virtual DbSet<InvoiceItem> InvoiceItems { get; set; }

    public virtual DbSet<MenuCategory> MenuCategories { get; set; }

    public virtual DbSet<MenuItem> MenuItems { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Room> Rooms { get; set; }

    public virtual DbSet<RoomCategory> RoomCategories { get; set; }

    public virtual DbSet<RoomMedium> RoomMedia { get; set; }

    public virtual DbSet<RoomOrder> RoomOrders { get; set; }

    public virtual DbSet<RoomOrderItem> RoomOrderItems { get; set; }

    public virtual DbSet<StaffUser> StaffUsers { get; set; }

    public virtual DbSet<Stay> Stays { get; set; }

    public virtual DbSet<WebsiteSyncLog> WebsiteSyncLogs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost;Database=GuestHouse;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.BookingId).HasName("PK__booking__5DE3A5B1A1852AEF");

            entity.Property(e => e.BookingStatus).HasDefaultValue("confirmed");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(sysdatetime())");

            entity.HasOne(d => d.Guest).WithMany(p => p.Bookings)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_booking_guest");
        });

        modelBuilder.Entity<BookingRoom>(entity =>
        {
            entity.HasKey(e => e.BookingRoomId).HasName("PK__booking___083C323CF8FCDD25");

            entity.Property(e => e.NumberOfGuests).HasDefaultValue(1);

            entity.HasOne(d => d.Booking).WithMany(p => p.BookingRooms).HasConstraintName("FK_booking_room_booking");

            entity.HasOne(d => d.Room).WithMany(p => p.BookingRooms)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_booking_room_room");
        });

        modelBuilder.Entity<ExpenseCharge>(entity =>
        {
            entity.HasKey(e => e.ChargeId).HasName("PK__expense___F3F52EBC8523040C");

            entity.Property(e => e.IncurredAt).HasDefaultValueSql("(sysdatetime())");

            entity.HasOne(d => d.RoomOrder).WithMany(p => p.ExpenseCharges).HasConstraintName("FK_expense_charge_room_order");

            entity.HasOne(d => d.Stay).WithMany(p => p.ExpenseCharges)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_expense_charge_stay");
        });

        modelBuilder.Entity<Guest>(entity =>
        {
            entity.HasKey(e => e.GuestId).HasName("PK__guest__19778E35E72EB441");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(sysdatetime())");
        });

        modelBuilder.Entity<Hotel>(entity =>
        {
            entity.HasKey(e => e.HotelId).HasName("PK__hotel__45FE7E26B6AB96C7");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<HotelExpense>(entity =>
        {
            entity.HasKey(e => e.ExpenseId).HasName("PK__hotel_ex__404B6A6B6241CB8E");

            entity.Property(e => e.ExpenseDate).HasDefaultValueSql("(sysdatetime())");

            entity.HasOne(d => d.Hotel).WithMany(p => p.HotelExpenses)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_hotel_expense_hotel");
        });

        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.HasKey(e => e.InvoiceId).HasName("PK__invoice__F58DFD498E7AFB15");

            entity.Property(e => e.GeneratedAt).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.InvoiceStatus).HasDefaultValue("unpaid");

            entity.HasOne(d => d.Booking).WithOne(p => p.Invoice)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_invoice_booking");
        });

        modelBuilder.Entity<InvoiceItem>(entity =>
        {
            entity.HasKey(e => e.InvoiceItemId).HasName("PK__invoice___84ECDEE94302E4E3");

            entity.Property(e => e.Quantity).HasDefaultValue(1);

            entity.HasOne(d => d.Invoice).WithMany(p => p.InvoiceItems).HasConstraintName("FK_invoice_item_invoice");
        });

        modelBuilder.Entity<MenuCategory>(entity =>
        {
            entity.HasKey(e => e.MenuCategoryId).HasName("PK__menu_cat__E3FCE267B00BEA9A");

            entity.HasOne(d => d.Hotel).WithMany(p => p.MenuCategories)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_menu_category_hotel");
        });

        modelBuilder.Entity<MenuItem>(entity =>
        {
            entity.HasKey(e => e.MenuItemId).HasName("PK__menu_ite__973431D5E3BF0586");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.IsAvailable).HasDefaultValue(true);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(sysdatetime())");

            entity.HasOne(d => d.MenuCategory).WithMany(p => p.MenuItems)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_menu_item_category");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__payment__ED1FC9EAE8DCF960");

            entity.Property(e => e.PaidAt).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.PaymentStatus).HasDefaultValue("pending");

            entity.HasOne(d => d.Booking).WithMany(p => p.Payments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_payment_booking");

            entity.HasOne(d => d.Invoice).WithMany(p => p.Payments).HasConstraintName("FK_payment_invoice");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__role__760965CC2BE7F92A");
        });

        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasKey(e => e.RoomId).HasName("PK__room__19675A8A4B774315");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Status).HasDefaultValue("available");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(sysdatetime())");

            entity.HasOne(d => d.Category).WithMany(p => p.Rooms)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_room_category");

            entity.HasOne(d => d.Hotel).WithMany(p => p.Rooms)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_room_hotel");
        });

        modelBuilder.Entity<RoomCategory>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__room_cat__D54EE9B48EAB7E99");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.MaxOccupancy).HasDefaultValue(1);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(sysdatetime())");

            entity.HasOne(d => d.Hotel).WithMany(p => p.RoomCategories)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_room_category_hotel");
        });

        modelBuilder.Entity<RoomMedium>(entity =>
        {
            entity.HasKey(e => e.MediaId).HasName("PK__room_med__D0A840F42C338844");

            entity.Property(e => e.DisplayOrder).HasDefaultValue(0);
            entity.Property(e => e.UploadedAt).HasDefaultValueSql("(sysdatetime())");

            entity.HasOne(d => d.Room).WithMany(p => p.RoomMedia).HasConstraintName("FK_room_media_room");
        });

        modelBuilder.Entity<RoomOrder>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__room_ord__4659622966EB661B");

            entity.Property(e => e.OrderStatus).HasDefaultValue("pending");
            entity.Property(e => e.OrderedAt).HasDefaultValueSql("(sysdatetime())");

            entity.HasOne(d => d.Room).WithMany(p => p.RoomOrders)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_room_order_room");

            entity.HasOne(d => d.Stay).WithMany(p => p.RoomOrders)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_room_order_stay");
        });

        modelBuilder.Entity<RoomOrderItem>(entity =>
        {
            entity.HasKey(e => e.OrderItemId).HasName("PK__room_ord__3764B6BC38D39CEE");

            entity.Property(e => e.Quantity).HasDefaultValue(1);

            entity.HasOne(d => d.MenuItem).WithMany(p => p.RoomOrderItems)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_room_order_item_menu");

            entity.HasOne(d => d.Order).WithMany(p => p.RoomOrderItems).HasConstraintName("FK_room_order_item_order");
        });

        modelBuilder.Entity<StaffUser>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__staff_us__B9BE370F5FAF11F2");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(sysdatetime())");

            entity.HasOne(d => d.Hotel).WithMany(p => p.StaffUsers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_staff_user_hotel");

            entity.HasOne(d => d.Role).WithMany(p => p.StaffUsers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_staff_user_role");
        });

        modelBuilder.Entity<Stay>(entity =>
        {
            entity.HasKey(e => e.StayId).HasName("PK__stay__C2B9B01F6D8F79C1");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.StayStatus).HasDefaultValue("active");

            entity.HasOne(d => d.Booking).WithOne(p => p.Stay)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_stay_booking");
        });

        modelBuilder.Entity<WebsiteSyncLog>(entity =>
        {
            entity.HasKey(e => e.SyncId).HasName("PK__website___54E41ED0AF405D67");

            entity.Property(e => e.SyncStatus).HasDefaultValue("success");
            entity.Property(e => e.SyncedAt).HasDefaultValueSql("(sysdatetime())");

            entity.HasOne(d => d.Hotel).WithMany(p => p.WebsiteSyncLogs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_website_sync_log_hotel");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
