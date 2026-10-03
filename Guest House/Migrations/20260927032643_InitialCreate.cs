using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Guest_House.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Guests",
                columns: table => new
                {
                    GuestId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IdProofType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IdProofNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(sysdatetime())"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(sysdatetime())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__guest__19778E35E72EB441", x => x.GuestId);
                });

            migrationBuilder.CreateTable(
                name: "hotel",
                columns: table => new
                {
                    hotel_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: false),
                    address = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    phone = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    email = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: true),
                    website_url = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__hotel__45FE7E26B6AB96C7", x => x.hotel_id);
                });

            migrationBuilder.CreateTable(
                name: "role",
                columns: table => new
                {
                    role_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    role_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__role__760965CC2BE7F92A", x => x.role_id);
                });

            migrationBuilder.CreateTable(
                name: "Bookings",
                columns: table => new
                {
                    BookingId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GuestId = table.Column<int>(type: "int", nullable: false),
                    BookingReference = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BookingSource = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CheckInDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpectedCheckout = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BookingStatus = table.Column<string>(type: "nvarchar(max)", nullable: true, defaultValue: "confirmed"),
                    SpecialRequest = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(sysdatetime())"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(sysdatetime())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__booking__5DE3A5B1A1852AEF", x => x.BookingId);
                    table.ForeignKey(
                        name: "FK_booking_guest",
                        column: x => x.GuestId,
                        principalTable: "Guests",
                        principalColumn: "GuestId");
                });

            migrationBuilder.CreateTable(
                name: "hotel_expense",
                columns: table => new
                {
                    expense_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    hotel_id = table.Column<int>(type: "int", nullable: false),
                    expense_category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    amount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    payment_method = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    expense_date = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(sysdatetime())"),
                    receipt_url = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__hotel_ex__404B6A6B6241CB8E", x => x.expense_id);
                    table.ForeignKey(
                        name: "FK_hotel_expense_hotel",
                        column: x => x.hotel_id,
                        principalTable: "hotel",
                        principalColumn: "hotel_id");
                });

            migrationBuilder.CreateTable(
                name: "menu_category",
                columns: table => new
                {
                    menu_category_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    hotel_id = table.Column<int>(type: "int", nullable: false),
                    category_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__menu_cat__E3FCE267B00BEA9A", x => x.menu_category_id);
                    table.ForeignKey(
                        name: "FK_menu_category_hotel",
                        column: x => x.hotel_id,
                        principalTable: "hotel",
                        principalColumn: "hotel_id");
                });

            migrationBuilder.CreateTable(
                name: "room_category",
                columns: table => new
                {
                    category_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    hotel_id = table.Column<int>(type: "int", nullable: false),
                    category_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    base_price = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    max_occupancy = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(sysdatetime())"),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(sysdatetime())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__room_cat__D54EE9B48EAB7E99", x => x.category_id);
                    table.ForeignKey(
                        name: "FK_room_category_hotel",
                        column: x => x.hotel_id,
                        principalTable: "hotel",
                        principalColumn: "hotel_id");
                });

            migrationBuilder.CreateTable(
                name: "website_sync_log",
                columns: table => new
                {
                    sync_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    hotel_id = table.Column<int>(type: "int", nullable: false),
                    entity_type = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    entity_id = table.Column<int>(type: "int", nullable: false),
                    sync_status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "success"),
                    error_message = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    synced_at = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(sysdatetime())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__website___54E41ED0AF405D67", x => x.sync_id);
                    table.ForeignKey(
                        name: "FK_website_sync_log_hotel",
                        column: x => x.hotel_id,
                        principalTable: "hotel",
                        principalColumn: "hotel_id");
                });

            migrationBuilder.CreateTable(
                name: "staff_user",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    hotel_id = table.Column<int>(type: "int", nullable: false),
                    role_id = table.Column<int>(type: "int", nullable: false),
                    full_name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    password_hash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    is_active = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(sysdatetime())"),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(sysdatetime())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__staff_us__B9BE370F5FAF11F2", x => x.user_id);
                    table.ForeignKey(
                        name: "FK_staff_user_hotel",
                        column: x => x.hotel_id,
                        principalTable: "hotel",
                        principalColumn: "hotel_id");
                    table.ForeignKey(
                        name: "FK_staff_user_role",
                        column: x => x.role_id,
                        principalTable: "role",
                        principalColumn: "role_id");
                });

            migrationBuilder.CreateTable(
                name: "invoice",
                columns: table => new
                {
                    invoice_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    booking_id = table.Column<int>(type: "int", nullable: false),
                    invoice_number = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    room_charge_total = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    extra_charge_total = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    tax_amount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    discount_amount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    grand_total = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    paid_amount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    due_amount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    invoice_status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "unpaid"),
                    generated_at = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(sysdatetime())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__invoice__F58DFD498E7AFB15", x => x.invoice_id);
                    table.ForeignKey(
                        name: "FK_invoice_booking",
                        column: x => x.booking_id,
                        principalTable: "Bookings",
                        principalColumn: "BookingId");
                });

            migrationBuilder.CreateTable(
                name: "stay",
                columns: table => new
                {
                    stay_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    booking_id = table.Column<int>(type: "int", nullable: false),
                    actual_checkin = table.Column<DateTime>(type: "datetime2", nullable: true),
                    actual_checkout = table.Column<DateTime>(type: "datetime2", nullable: true),
                    stay_status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "active"),
                    notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(sysdatetime())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__stay__C2B9B01F6D8F79C1", x => x.stay_id);
                    table.ForeignKey(
                        name: "FK_stay_booking",
                        column: x => x.booking_id,
                        principalTable: "Bookings",
                        principalColumn: "BookingId");
                });

            migrationBuilder.CreateTable(
                name: "menu_item",
                columns: table => new
                {
                    menu_item_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    menu_category_id = table.Column<int>(type: "int", nullable: false),
                    item_name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    price = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    image_url = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    is_available = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(sysdatetime())"),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(sysdatetime())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__menu_ite__973431D5E3BF0586", x => x.menu_item_id);
                    table.ForeignKey(
                        name: "FK_menu_item_category",
                        column: x => x.menu_category_id,
                        principalTable: "menu_category",
                        principalColumn: "menu_category_id");
                });

            migrationBuilder.CreateTable(
                name: "room",
                columns: table => new
                {
                    room_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    hotel_id = table.Column<int>(type: "int", nullable: false),
                    category_id = table.Column<int>(type: "int", nullable: false),
                    room_number = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    floor_number = table.Column<int>(type: "int", nullable: true),
                    status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "available"),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(sysdatetime())"),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(sysdatetime())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__room__19675A8A4B774315", x => x.room_id);
                    table.ForeignKey(
                        name: "FK_room_category",
                        column: x => x.category_id,
                        principalTable: "room_category",
                        principalColumn: "category_id");
                    table.ForeignKey(
                        name: "FK_room_hotel",
                        column: x => x.hotel_id,
                        principalTable: "hotel",
                        principalColumn: "hotel_id");
                });

            migrationBuilder.CreateTable(
                name: "invoice_item",
                columns: table => new
                {
                    invoice_item_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    invoice_id = table.Column<int>(type: "int", nullable: false),
                    item_type = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    quantity = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    unit_price = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    amount = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__invoice___84ECDEE94302E4E3", x => x.invoice_item_id);
                    table.ForeignKey(
                        name: "FK_invoice_item_invoice",
                        column: x => x.invoice_id,
                        principalTable: "invoice",
                        principalColumn: "invoice_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "payment",
                columns: table => new
                {
                    payment_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    booking_id = table.Column<int>(type: "int", nullable: false),
                    invoice_id = table.Column<int>(type: "int", nullable: true),
                    amount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    payment_method = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    payment_type = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    payment_status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "pending"),
                    transaction_ref = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    paid_at = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(sysdatetime())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__payment__ED1FC9EAE8DCF960", x => x.payment_id);
                    table.ForeignKey(
                        name: "FK_payment_booking",
                        column: x => x.booking_id,
                        principalTable: "Bookings",
                        principalColumn: "BookingId");
                    table.ForeignKey(
                        name: "FK_payment_invoice",
                        column: x => x.invoice_id,
                        principalTable: "invoice",
                        principalColumn: "invoice_id");
                });

            migrationBuilder.CreateTable(
                name: "BookingRooms",
                columns: table => new
                {
                    BookingRoomId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookingId = table.Column<int>(type: "int", nullable: false),
                    RoomId = table.Column<int>(type: "int", nullable: false),
                    RoomPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NumberOfGuests = table.Column<int>(type: "int", nullable: true, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__booking___083C323CF8FCDD25", x => x.BookingRoomId);
                    table.ForeignKey(
                        name: "FK_booking_room_booking",
                        column: x => x.BookingId,
                        principalTable: "Bookings",
                        principalColumn: "BookingId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_booking_room_room",
                        column: x => x.RoomId,
                        principalTable: "room",
                        principalColumn: "room_id");
                });

            migrationBuilder.CreateTable(
                name: "room_media",
                columns: table => new
                {
                    media_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    room_id = table.Column<int>(type: "int", nullable: false),
                    media_type = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    file_url = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    caption = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    display_order = table.Column<int>(type: "int", nullable: true, defaultValue: 0),
                    uploaded_at = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(sysdatetime())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__room_med__D0A840F42C338844", x => x.media_id);
                    table.ForeignKey(
                        name: "FK_room_media_room",
                        column: x => x.room_id,
                        principalTable: "room",
                        principalColumn: "room_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "room_order",
                columns: table => new
                {
                    order_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    stay_id = table.Column<int>(type: "int", nullable: false),
                    room_id = table.Column<int>(type: "int", nullable: false),
                    order_number = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    order_status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "pending"),
                    notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ordered_at = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(sysdatetime())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__room_ord__4659622966EB661B", x => x.order_id);
                    table.ForeignKey(
                        name: "FK_room_order_room",
                        column: x => x.room_id,
                        principalTable: "room",
                        principalColumn: "room_id");
                    table.ForeignKey(
                        name: "FK_room_order_stay",
                        column: x => x.stay_id,
                        principalTable: "stay",
                        principalColumn: "stay_id");
                });

            migrationBuilder.CreateTable(
                name: "expense_charge",
                columns: table => new
                {
                    charge_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    stay_id = table.Column<int>(type: "int", nullable: false),
                    charge_type = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    room_order_id = table.Column<int>(type: "int", nullable: true),
                    amount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    incurred_at = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(sysdatetime())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__expense___F3F52EBC8523040C", x => x.charge_id);
                    table.ForeignKey(
                        name: "FK_expense_charge_room_order",
                        column: x => x.room_order_id,
                        principalTable: "room_order",
                        principalColumn: "order_id");
                    table.ForeignKey(
                        name: "FK_expense_charge_stay",
                        column: x => x.stay_id,
                        principalTable: "stay",
                        principalColumn: "stay_id");
                });

            migrationBuilder.CreateTable(
                name: "room_order_item",
                columns: table => new
                {
                    order_item_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    order_id = table.Column<int>(type: "int", nullable: false),
                    menu_item_id = table.Column<int>(type: "int", nullable: false),
                    quantity = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    unit_price = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    total_price = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    special_instruction = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__room_ord__3764B6BC38D39CEE", x => x.order_item_id);
                    table.ForeignKey(
                        name: "FK_room_order_item_menu",
                        column: x => x.menu_item_id,
                        principalTable: "menu_item",
                        principalColumn: "menu_item_id");
                    table.ForeignKey(
                        name: "FK_room_order_item_order",
                        column: x => x.order_id,
                        principalTable: "room_order",
                        principalColumn: "order_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BookingRooms_BookingId",
                table: "BookingRooms",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingRooms_RoomId",
                table: "BookingRooms",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_GuestId",
                table: "Bookings",
                column: "GuestId");

            migrationBuilder.CreateIndex(
                name: "IX_expense_charge_room_order_id",
                table: "expense_charge",
                column: "room_order_id");

            migrationBuilder.CreateIndex(
                name: "IX_expense_charge_stay_id",
                table: "expense_charge",
                column: "stay_id");

            migrationBuilder.CreateIndex(
                name: "IX_hotel_expense_hotel_id",
                table: "hotel_expense",
                column: "hotel_id");

            migrationBuilder.CreateIndex(
                name: "UQ_invoice_booking",
                table: "invoice",
                column: "booking_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_invoice_number",
                table: "invoice",
                column: "invoice_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_invoice_item_invoice_id",
                table: "invoice_item",
                column: "invoice_id");

            migrationBuilder.CreateIndex(
                name: "UQ_menu_category_hotel_name",
                table: "menu_category",
                columns: new[] { "hotel_id", "category_name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_menu_item_menu_category_id",
                table: "menu_item",
                column: "menu_category_id");

            migrationBuilder.CreateIndex(
                name: "IX_payment_booking_id",
                table: "payment",
                column: "booking_id");

            migrationBuilder.CreateIndex(
                name: "IX_payment_invoice_id",
                table: "payment",
                column: "invoice_id");

            migrationBuilder.CreateIndex(
                name: "UQ_role_name",
                table: "role",
                column: "role_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_room_category_id",
                table: "room",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "UQ_room_hotel_number",
                table: "room",
                columns: new[] { "hotel_id", "room_number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_room_category_hotel_name",
                table: "room_category",
                columns: new[] { "hotel_id", "category_name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_room_media_room_id",
                table: "room_media",
                column: "room_id");

            migrationBuilder.CreateIndex(
                name: "IX_room_order_room_id",
                table: "room_order",
                column: "room_id");

            migrationBuilder.CreateIndex(
                name: "IX_room_order_stay_id",
                table: "room_order",
                column: "stay_id");

            migrationBuilder.CreateIndex(
                name: "UQ_room_order_number",
                table: "room_order",
                column: "order_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_room_order_item_menu_item_id",
                table: "room_order_item",
                column: "menu_item_id");

            migrationBuilder.CreateIndex(
                name: "IX_room_order_item_order_id",
                table: "room_order_item",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "IX_staff_user_hotel_id",
                table: "staff_user",
                column: "hotel_id");

            migrationBuilder.CreateIndex(
                name: "IX_staff_user_role_id",
                table: "staff_user",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "UQ_staff_user_username",
                table: "staff_user",
                column: "username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_stay_booking",
                table: "stay",
                column: "booking_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_website_sync_log_hotel_id",
                table: "website_sync_log",
                column: "hotel_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookingRooms");

            migrationBuilder.DropTable(
                name: "expense_charge");

            migrationBuilder.DropTable(
                name: "hotel_expense");

            migrationBuilder.DropTable(
                name: "invoice_item");

            migrationBuilder.DropTable(
                name: "payment");

            migrationBuilder.DropTable(
                name: "room_media");

            migrationBuilder.DropTable(
                name: "room_order_item");

            migrationBuilder.DropTable(
                name: "staff_user");

            migrationBuilder.DropTable(
                name: "website_sync_log");

            migrationBuilder.DropTable(
                name: "invoice");

            migrationBuilder.DropTable(
                name: "menu_item");

            migrationBuilder.DropTable(
                name: "room_order");

            migrationBuilder.DropTable(
                name: "role");

            migrationBuilder.DropTable(
                name: "menu_category");

            migrationBuilder.DropTable(
                name: "room");

            migrationBuilder.DropTable(
                name: "stay");

            migrationBuilder.DropTable(
                name: "room_category");

            migrationBuilder.DropTable(
                name: "Bookings");

            migrationBuilder.DropTable(
                name: "hotel");

            migrationBuilder.DropTable(
                name: "Guests");
        }
    }
}
