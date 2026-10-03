-- ==============================================================================
-- Guest House Booking System - Billing & Invoice Module Database Schema
-- Target RDBMS: Microsoft SQL Server (SSMS Compatible)
-- Note: Converted from original MySQL dialect to SQL Server standards:
--   - Uses INT IDENTITY(1,1) instead of AUTO_INCREMENT
--   - Uses NVARCHAR / VARCHAR instead of ENUM and backticks
--   - Uses CHECK constraints for enum-like domain integrity
--   - Uses DECIMAL(10,2) for exact monetary precision
--   - Uses DATETIME2 with SYSDATETIME() default
--   - Fully qualified SQL Server Foreign Key constraints
-- ==============================================================================

USE [GuestHouse];
GO

-- 1. Table: expense_charge
-- Stores customer-incurred incidentals and extra charges during their stay
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'expense_charge')
BEGIN
    CREATE TABLE [dbo].[expense_charge] (
        [charge_id]      INT IDENTITY(1,1)   NOT NULL,
        [stay_id]        INT                 NOT NULL,
        [charge_type]    NVARCHAR(30)        NOT NULL,
        [description]    NVARCHAR(255)       NULL,
        [room_order_id]  INT                 NULL,
        [amount]         DECIMAL(10,2)       NOT NULL,
        [incurred_at]    DATETIME2           NULL CONSTRAINT [DF_expense_charge_incurred_at] DEFAULT (SYSDATETIME()),

        CONSTRAINT [PK_expense_charge] PRIMARY KEY CLUSTERED ([charge_id] ASC),
        CONSTRAINT [FK_expense_charge_stay] FOREIGN KEY ([stay_id]) REFERENCES [dbo].[stay] ([stay_id]),
        CONSTRAINT [FK_expense_charge_room_order] FOREIGN KEY ([room_order_id]) REFERENCES [dbo].[room_order] ([order_id]),
        CONSTRAINT [CK_expense_charge_charge_type] CHECK ([charge_type] IN ('room_charge', 'room_order', 'minibar', 'laundry', 'damage', 'extra_bed', 'service_fee', 'other')),
        CONSTRAINT [CK_expense_charge_amount] CHECK ([amount] >= 0.00)
    );

    CREATE NONCLUSTERED INDEX [IX_expense_charge_stay_id] ON [dbo].[expense_charge] ([stay_id] ASC);
    CREATE NONCLUSTERED INDEX [IX_expense_charge_room_order_id] ON [dbo].[expense_charge] ([room_order_id] ASC);
END
GO

-- 2. Table: invoice
-- Master bill for a booking aggregating room charges and incidental extra charges
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'invoice')
BEGIN
    CREATE TABLE [dbo].[invoice] (
        [invoice_id]          INT IDENTITY(1,1)   NOT NULL,
        [booking_id]          INT                 NOT NULL,
        [invoice_number]      NVARCHAR(50)        NOT NULL,
        [room_charge_total]   DECIMAL(10,2)       NOT NULL,
        [extra_charge_total]  DECIMAL(10,2)       NOT NULL,
        [tax_amount]          DECIMAL(10,2)       NOT NULL CONSTRAINT [DF_invoice_tax_amount] DEFAULT (0.00),
        [discount_amount]     DECIMAL(10,2)       NOT NULL CONSTRAINT [DF_invoice_discount_amount] DEFAULT (0.00),
        [grand_total]         DECIMAL(10,2)       NOT NULL,
        [paid_amount]         DECIMAL(10,2)       NOT NULL CONSTRAINT [DF_invoice_paid_amount] DEFAULT (0.00),
        [due_amount]          DECIMAL(10,2)       NOT NULL,
        [invoice_status]      NVARCHAR(20)        NOT NULL CONSTRAINT [DF_invoice_invoice_status] DEFAULT ('unpaid'),
        [generated_at]        DATETIME2           NULL CONSTRAINT [DF_invoice_generated_at] DEFAULT (SYSDATETIME()),

        CONSTRAINT [PK_invoice] PRIMARY KEY CLUSTERED ([invoice_id] ASC),
        CONSTRAINT [FK_invoice_booking] FOREIGN KEY ([booking_id]) REFERENCES [dbo].[Bookings] ([BookingId]),
        CONSTRAINT [UQ_invoice_booking] UNIQUE ([booking_id]),
        CONSTRAINT [UQ_invoice_number] UNIQUE ([invoice_number]),
        CONSTRAINT [CK_invoice_status] CHECK ([invoice_status] IN ('unpaid', 'partially_paid', 'paid', 'cancelled')),
        CONSTRAINT [CK_invoice_room_charge_total] CHECK ([room_charge_total] >= 0.00),
        CONSTRAINT [CK_invoice_extra_charge_total] CHECK ([extra_charge_total] >= 0.00),
        CONSTRAINT [CK_invoice_tax_amount] CHECK ([tax_amount] >= 0.00),
        CONSTRAINT [CK_invoice_discount_amount] CHECK ([discount_amount] >= 0.00),
        CONSTRAINT [CK_invoice_grand_total] CHECK ([grand_total] >= 0.00),
        CONSTRAINT [CK_invoice_paid_amount] CHECK ([paid_amount] >= 0.00),
        CONSTRAINT [CK_invoice_due_amount] CHECK ([due_amount] >= 0.00)
    );
END
GO

-- 3. Table: invoice_item
-- Itemized line entries for an invoice (room charges, minibar, service fees, laundry, etc.)
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'invoice_item')
BEGIN
    CREATE TABLE [dbo].[invoice_item] (
        [invoice_item_id]  INT IDENTITY(1,1)   NOT NULL,
        [invoice_id]       INT                 NOT NULL,
        [item_type]        NVARCHAR(30)        NOT NULL,
        [description]      NVARCHAR(255)       NOT NULL,
        [quantity]         INT                 NOT NULL CONSTRAINT [DF_invoice_item_quantity] DEFAULT (1),
        [unit_price]       DECIMAL(10,2)       NOT NULL,
        [amount]           DECIMAL(10,2)       NOT NULL,

        CONSTRAINT [PK_invoice_item] PRIMARY KEY CLUSTERED ([invoice_item_id] ASC),
        CONSTRAINT [FK_invoice_item_invoice] FOREIGN KEY ([invoice_id]) REFERENCES [dbo].[invoice] ([invoice_id]) ON DELETE CASCADE,
        CONSTRAINT [CK_invoice_item_item_type] CHECK ([item_type] IN ('room', 'room_order', 'laundry', 'minibar', 'damage', 'extra_bed', 'service', 'other')),
        CONSTRAINT [CK_invoice_item_quantity] CHECK ([quantity] > 0),
        CONSTRAINT [CK_invoice_item_unit_price] CHECK ([unit_price] >= 0.00),
        CONSTRAINT [CK_invoice_item_amount] CHECK ([amount] >= 0.00)
    );

    CREATE NONCLUSTERED INDEX [IX_invoice_item_invoice_id] ON [dbo].[invoice_item] ([invoice_id] ASC);
END
GO
