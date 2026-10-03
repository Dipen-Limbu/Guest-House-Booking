USE [GuestHouse];
GO

-- =====================================================================
-- 1. HOTEL
-- =====================================================================
IF NOT EXISTS (SELECT 1 FROM [hotel] WHERE [hotel_id] = 1)
BEGIN
    SET IDENTITY_INSERT [hotel] ON;
    INSERT INTO [hotel] ([hotel_id], [name], [address], [phone], [email], [website_url], [created_at])
    VALUES (1, 'Kalika Hotel & Lodge', 'Itahari-9, Buspark', '9800000000',
            'kalika.itahari@gmail.com', 'https://kalikahotel.com', GETDATE());
    SET IDENTITY_INSERT [hotel] OFF;
END
GO

-- =====================================================================
-- 2. ROLES  (Manager added because the Room APIs allow Admin,Manager)
-- =====================================================================
IF NOT EXISTS (SELECT 1 FROM [role] WHERE [role_name] = 'Admin')
    INSERT INTO [role] ([role_name], [description]) VALUES ('Admin', 'Full administrative access');
IF NOT EXISTS (SELECT 1 FROM [role] WHERE [role_name] = 'Manager')
    INSERT INTO [role] ([role_name], [description]) VALUES ('Manager', 'Hotel manager');
IF NOT EXISTS (SELECT 1 FROM [role] WHERE [role_name] = 'Staff')
    INSERT INTO [role] ([role_name], [description]) VALUES ('Staff', 'General staff member');
IF NOT EXISTS (SELECT 1 FROM [role] WHERE [role_name] = 'Guest')
    INSERT INTO [role] ([role_name], [description]) VALUES ('Guest', 'Hotel guest account');
GO

-- =====================================================================
-- 3. USERS  (no variables, each insert is self-contained)
--    admin@gmail.com   / Admin@123
--    manager@gmail.com / Manager@123
--    staff@gmail.com   / Staff@123
--    guest@gmail.com   / Guest@123
-- =====================================================================
IF NOT EXISTS (SELECT 1 FROM [staff_user] WHERE [username] = 'admin@gmail.com')
    INSERT INTO [staff_user] ([hotel_id], [role_id], [full_name], [username], [password_hash], [is_active], [created_at], [updated_at])
    SELECT 1, r.[role_id], 'Test Admin', 'admin@gmail.com',
           'oByNQe0IdymBbpdqYtl1bg==:kC46RnwB5IwQf9OWzKopvow57XXUWpYRuuK+n7dStkg=:100000', 1, GETDATE(), GETDATE()
    FROM [role] r WHERE r.[role_name] = 'Admin';

IF NOT EXISTS (SELECT 1 FROM [staff_user] WHERE [username] = 'manager@gmail.com')
    INSERT INTO [staff_user] ([hotel_id], [role_id], [full_name], [username], [password_hash], [is_active], [created_at], [updated_at])
    SELECT 1, r.[role_id], 'Test Manager', 'manager@gmail.com',
           'AQIDBAUGBwgJCgsMDQ4PEA==:A541fqUn7EvROJzB9t1ip4kcjz3MAFbhgspYqjHziec=:100000', 1, GETDATE(), GETDATE()
    FROM [role] r WHERE r.[role_name] = 'Manager';

IF NOT EXISTS (SELECT 1 FROM [staff_user] WHERE [username] = 'staff@gmail.com')
    INSERT INTO [staff_user] ([hotel_id], [role_id], [full_name], [username], [password_hash], [is_active], [created_at], [updated_at])
    SELECT 1, r.[role_id], 'Test Staff', 'staff@gmail.com',
           'fKs7TRPZfk/Gwp2aM4jt0g==:odOVUHX48bqV8DNSqGyzf3hCk94tq5FfzPvenxo62Pk=:100000', 1, GETDATE(), GETDATE()
    FROM [role] r WHERE r.[role_name] = 'Staff';

IF NOT EXISTS (SELECT 1 FROM [staff_user] WHERE [username] = 'guest@gmail.com')
    INSERT INTO [staff_user] ([hotel_id], [role_id], [full_name], [username], [password_hash], [is_active], [created_at], [updated_at])
    SELECT 1, r.[role_id], 'Test Guest', 'guest@gmail.com',
           '5YXwG49Cy1Ic8OiAjls/Jw==:vs2joFGURRc6XOD1qKbsF/nWa7RmqrZ2vXPXide73Mk=:100000', 1, GETDATE(), GETDATE()
    FROM [role] r WHERE r.[role_name] = 'Guest';
GO

-- =====================================================================
-- 4. ROOM CATEGORIES (pricing tiers)
-- =====================================================================
INSERT INTO [room_category] ([hotel_id], [category_name], [base_price], [max_occupancy], [description])
SELECT 1, v.n, v.p, v.o, v.d
FROM (VALUES
    ('Standard', 1500.00, 2, 'Comfortable room with a double bed and attached bathroom.'),
    ('Deluxe',   2500.00, 2, 'Spacious room with AC, TV and a balcony.'),
    ('Family',   3500.00, 4, 'Large room with two beds, ideal for families.'),
    ('Suite',    5000.00, 3, 'Premium suite with a sitting area and king bed.')
) AS v(n, p, o, d)
WHERE NOT EXISTS (
    SELECT 1 FROM [room_category] c WHERE c.[hotel_id] = 1 AND c.[category_name] = v.n
);
GO

-- =====================================================================
-- 5. ROOMS
-- =====================================================================
INSERT INTO [room] ([hotel_id], [category_id], [room_number], [floor_number], [status], [description])
SELECT 1, c.[category_id], v.num, v.fl, v.st, v.d
FROM (VALUES
    ('101', 'Standard', 1, 'available',   'Ground-side room near the lobby.'),
    ('102', 'Standard', 1, 'available',   'Quiet room facing the garden.'),
    ('103', 'Standard', 1, 'cleaning',    'Recently vacated, being cleaned.'),
    ('201', 'Deluxe',   2, 'available',   'Deluxe room with street view.'),
    ('202', 'Deluxe',   2, 'occupied',    'Deluxe room, currently occupied.'),
    ('203', 'Family',   2, 'available',   'Family room with two double beds.'),
    ('301', 'Suite',    3, 'available',   'Top-floor suite with city view.'),
    ('302', 'Suite',    3, 'maintenance', 'Air conditioner under repair.')
) AS v(num, cat, fl, st, d)
JOIN [room_category] c ON c.[hotel_id] = 1 AND c.[category_name] = v.cat
WHERE NOT EXISTS (
    SELECT 1 FROM [room] r WHERE r.[hotel_id] = 1 AND r.[room_number] = v.num
);
GO

-- =====================================================================
-- 6. ROOM MEDIA (placeholder URLs)
-- =====================================================================
INSERT INTO [room_media] ([room_id], [media_type], [file_url], [caption], [display_order])
SELECT r.[room_id], v.t, v.u, v.cap, v.ord
FROM (VALUES
    ('101', 'image', 'https://example.com/rooms/101-front.jpg',  'Front view',     1),
    ('101', 'image', 'https://example.com/rooms/101-bath.jpg',   'Bathroom',       2),
    ('201', 'image', 'https://example.com/rooms/201-main.jpg',   'Main view',      1),
    ('201', 'video', 'https://example.com/rooms/201-tour.mp4',   'Room tour',      2),
    ('301', 'image', 'https://example.com/rooms/301-suite.jpg',  'Suite bedroom',  1)
) AS v(num, t, u, cap, ord)
JOIN [room] r ON r.[hotel_id] = 1 AND r.[room_number] = v.num
WHERE NOT EXISTS (
    SELECT 1 FROM [room_media] m WHERE m.[room_id] = r.[room_id] AND m.[file_url] = v.u
);
GO

-- =====================================================================
-- 7. QUICK CHECK
-- =====================================================================
SELECT c.category_name, c.base_price, COUNT(r.room_id) AS rooms
FROM room_category c LEFT JOIN room r ON r.category_id = c.category_id
WHERE c.hotel_id = 1 GROUP BY c.category_name, c.base_price ORDER BY c.base_price;

SELECT r.room_number, c.category_name, r.status,
       (SELECT COUNT(*) FROM room_media m WHERE m.room_id = r.room_id) AS media_count
FROM room r JOIN room_category c ON c.category_id = r.category_id
WHERE r.hotel_id = 1 ORDER BY r.room_number;
GO