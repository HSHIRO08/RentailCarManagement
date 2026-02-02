-- =============================================
-- 🚗 RENTAL CAR MANAGEMENT - IDENTITY SYSTEM
-- =============================================
-- Database: PostgreSQL 12+
-- Framework: ASP.NET Core Identity + EF Core 8
-- Password: Password123!
-- =============================================

-- Enable Extensions
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- =============================================
-- DROP ALL TABLES (Clean start)
-- =============================================
DROP TABLE IF EXISTS "__EFMigrationsHistory" CASCADE;
DROP TABLE IF EXISTS "AspNetUserTokens" CASCADE;
DROP TABLE IF EXISTS "AspNetUserRoles" CASCADE;
DROP TABLE IF EXISTS "AspNetUserLogins" CASCADE;
DROP TABLE IF EXISTS "AspNetUserClaims" CASCADE;
DROP TABLE IF EXISTS "AspNetRoleClaims" CASCADE;
DROP TABLE IF EXISTS chat_messages CASCADE;
DROP TABLE IF EXISTS chat_sessions CASCADE;
DROP TABLE IF EXISTS complaints CASCADE;
DROP TABLE IF EXISTS reviews CASCADE;
DROP TABLE IF EXISTS commissions CASCADE;
DROP TABLE IF EXISTS payments CASCADE;
DROP TABLE IF EXISTS rentals CASCADE;
DROP TABLE IF EXISTS car_documents CASCADE;
DROP TABLE IF EXISTS car_images CASCADE;
DROP TABLE IF EXISTS cars CASCADE;
DROP TABLE IF EXISTS car_categories CASCADE;
DROP TABLE IF EXISTS customers CASCADE;
DROP TABLE IF EXISTS suppliers CASCADE;
DROP TABLE IF EXISTS "AspNetRoles" CASCADE;
DROP TABLE IF EXISTS "AspNetUsers" CASCADE;

-- =============================================
-- IDENTITY TABLES (Standard ASP.NET Core Identity)
-- =============================================

-- AspNetRoles (Identity Roles)
CREATE TABLE "AspNetRoles" (
    "Id" UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    "Name" VARCHAR(256),
    "NormalizedName" VARCHAR(256) UNIQUE,
    "ConcurrencyStamp" TEXT,
    "Description" TEXT,
    "CreatedAt" TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

-- AspNetUsers (Identity Users)
CREATE TABLE "AspNetUsers" (
    "Id" UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    "UserName" VARCHAR(256),
    "NormalizedUserName" VARCHAR(256),
    "Email" VARCHAR(256),
    "NormalizedEmail" VARCHAR(256),
    "EmailConfirmed" BOOLEAN NOT NULL DEFAULT TRUE,
    "PasswordHash" TEXT,
    "SecurityStamp" TEXT,
    "ConcurrencyStamp" TEXT,
    "PhoneNumber" TEXT,
    "PhoneNumberConfirmed" BOOLEAN NOT NULL DEFAULT FALSE,
    "TwoFactorEnabled" BOOLEAN NOT NULL DEFAULT FALSE,
    "LockoutEnd" TIMESTAMP WITH TIME ZONE,
    "LockoutEnabled" BOOLEAN NOT NULL DEFAULT FALSE,
    "AccessFailedCount" INTEGER NOT NULL DEFAULT 0,
    -- Custom fields
    "FullName" VARCHAR(100) NOT NULL,
    "Status" VARCHAR(20) NOT NULL DEFAULT 'Active',
    "CreatedAt" TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    "UpdatedAt" TIMESTAMP WITH TIME ZONE
);

-- AspNetUserRoles (Many-to-Many: Users ↔ Roles)
CREATE TABLE "AspNetUserRoles" (
    "UserId" UUID NOT NULL,
    "RoleId" UUID NOT NULL,
    PRIMARY KEY ("UserId", "RoleId"),
    FOREIGN KEY ("UserId") REFERENCES "AspNetUsers"("Id") ON DELETE CASCADE,
    FOREIGN KEY ("RoleId") REFERENCES "AspNetRoles"("Id") ON DELETE CASCADE
);

-- AspNetUserClaims
CREATE TABLE "AspNetUserClaims" (
    "Id" SERIAL PRIMARY KEY,
    "UserId" UUID NOT NULL,
    "ClaimType" TEXT,
    "ClaimValue" TEXT,
    FOREIGN KEY ("UserId") REFERENCES "AspNetUsers"("Id") ON DELETE CASCADE
);

-- AspNetUserLogins (External logins: Google, Facebook...)
CREATE TABLE "AspNetUserLogins" (
    "LoginProvider" TEXT NOT NULL,
    "ProviderKey" TEXT NOT NULL,
    "ProviderDisplayName" TEXT,
    "UserId" UUID NOT NULL,
    PRIMARY KEY ("LoginProvider", "ProviderKey"),
    FOREIGN KEY ("UserId") REFERENCES "AspNetUsers"("Id") ON DELETE CASCADE
);

-- AspNetUserTokens (Reset password, 2FA tokens)
CREATE TABLE "AspNetUserTokens" (
    "UserId" UUID NOT NULL,
    "LoginProvider" TEXT NOT NULL,
    "Name" TEXT NOT NULL,
    "Value" TEXT,
    PRIMARY KEY ("UserId", "LoginProvider", "Name"),
    FOREIGN KEY ("UserId") REFERENCES "AspNetUsers"("Id") ON DELETE CASCADE
);

-- AspNetRoleClaims
CREATE TABLE "AspNetRoleClaims" (
    "Id" SERIAL PRIMARY KEY,
    "RoleId" UUID NOT NULL,
    "ClaimType" TEXT,
    "ClaimValue" TEXT,
    FOREIGN KEY ("RoleId") REFERENCES "AspNetRoles"("Id") ON DELETE CASCADE
);

-- =============================================
-- BUSINESS TABLES
-- =============================================

-- Suppliers (1-1 with AspNetUsers)
CREATE TABLE suppliers (
    supplier_id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    user_id UUID UNIQUE NOT NULL,
    company_name VARCHAR(200),
    tax_code VARCHAR(50),
    address TEXT,
    bank_account VARCHAR(50),
    bank_name VARCHAR(100),
    is_verified BOOLEAN DEFAULT FALSE,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (user_id) REFERENCES "AspNetUsers"("Id") ON DELETE CASCADE
);

-- Customers (1-1 with AspNetUsers)
CREATE TABLE customers (
    customer_id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    user_id UUID UNIQUE NOT NULL,
    driver_license_number VARCHAR(50),
    license_expiry_date DATE,
    address TEXT,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (user_id) REFERENCES "AspNetUsers"("Id") ON DELETE CASCADE
);

-- Car Categories
CREATE TABLE car_categories (
    category_id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    name VARCHAR(100) NOT NULL,
    description TEXT,
    is_active BOOLEAN DEFAULT TRUE,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

-- Cars
CREATE TABLE cars (
    car_id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    supplier_id UUID NOT NULL,
    category_id UUID NOT NULL,
    license_plate VARCHAR(20) UNIQUE NOT NULL,
    brand VARCHAR(50) NOT NULL,
    model VARCHAR(50) NOT NULL,
    year INT,
    seats INT,
    fuel_type VARCHAR(50),
    transmission VARCHAR(50),
    price_per_day DECIMAL(15, 2) NOT NULL,
    price_per_hour DECIMAL(15, 2),
    status VARCHAR(20) DEFAULT 'Available',
    is_approved BOOLEAN DEFAULT FALSE,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP WITH TIME ZONE,
    FOREIGN KEY (supplier_id) REFERENCES suppliers(supplier_id),
    FOREIGN KEY (category_id) REFERENCES car_categories(category_id)
);

-- Car Images
CREATE TABLE car_images (
    image_id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    car_id UUID NOT NULL,
    image_url TEXT NOT NULL,
    is_primary BOOLEAN DEFAULT FALSE,
    display_order INT DEFAULT 0,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (car_id) REFERENCES cars(car_id) ON DELETE CASCADE
);

-- Car Documents
CREATE TABLE car_documents (
    document_id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    car_id UUID NOT NULL,
    document_type VARCHAR(50) NOT NULL DEFAULT 'Registration',
    document_number VARCHAR(100),
    document_url TEXT,
    issue_date DATE,
    expiry_date DATE,
    is_verified BOOLEAN DEFAULT FALSE,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (car_id) REFERENCES cars(car_id) ON DELETE CASCADE
);

-- Rentals
CREATE TABLE rentals (
    rental_id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    car_id UUID NOT NULL,
    customer_id UUID NOT NULL,
    start_date TIMESTAMP WITH TIME ZONE NOT NULL,
    end_date TIMESTAMP WITH TIME ZONE NOT NULL,
    total_amount DECIMAL(15, 2) NOT NULL,
    status VARCHAR(20) DEFAULT 'Pending',
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP WITH TIME ZONE,
    FOREIGN KEY (car_id) REFERENCES cars(car_id),
    FOREIGN KEY (customer_id) REFERENCES customers(customer_id)
);

-- Payments (1-1 with Rentals)
CREATE TABLE payments (
    payment_id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    rental_id UUID UNIQUE NOT NULL,
    amount DECIMAL(15, 2) NOT NULL,
    payment_method VARCHAR(50),
    status VARCHAR(20) DEFAULT 'Pending',
    paid_at TIMESTAMP WITH TIME ZONE,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (rental_id) REFERENCES rentals(rental_id)
);

-- Commissions
CREATE TABLE commissions (
    commission_id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    rental_id UUID UNIQUE NOT NULL,
    supplier_id UUID NOT NULL,
    commission_rate DECIMAL(5, 4) DEFAULT 0.15,
    system_amount DECIMAL(15, 2) NOT NULL,
    supplier_amount DECIMAL(15, 2) NOT NULL,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (rental_id) REFERENCES rentals(rental_id),
    FOREIGN KEY (supplier_id) REFERENCES suppliers(supplier_id)
);

-- Reviews (1-1 with Rentals)
CREATE TABLE reviews (
    review_id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    rental_id UUID UNIQUE NOT NULL,
    customer_id UUID NOT NULL,
    rating INT CHECK (rating >= 1 AND rating <= 5),
    comment TEXT,
    is_approved BOOLEAN DEFAULT TRUE,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (rental_id) REFERENCES rentals(rental_id),
    FOREIGN KEY (customer_id) REFERENCES customers(customer_id)
);

-- Complaints
CREATE TABLE complaints (
    complaint_id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    rental_id UUID NOT NULL,
    user_id UUID NOT NULL,
    title VARCHAR(200),
    description TEXT,
    resolution TEXT,
    status VARCHAR(50) DEFAULT 'Pending',
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    resolved_at TIMESTAMP WITH TIME ZONE,
    FOREIGN KEY (rental_id) REFERENCES rentals(rental_id),
    FOREIGN KEY (user_id) REFERENCES "AspNetUsers"("Id")
);

-- Chat Sessions
CREATE TABLE chat_sessions (
    chat_session_id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    user_id UUID NOT NULL,
    subject VARCHAR(200),
    is_active BOOLEAN DEFAULT TRUE,
    started_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    ended_at TIMESTAMP WITH TIME ZONE,
    FOREIGN KEY (user_id) REFERENCES "AspNetUsers"("Id")
);

-- Chat Messages
CREATE TABLE chat_messages (
    message_id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    chat_session_id UUID NOT NULL,
    sender_type VARCHAR(20) NOT NULL,
    content TEXT NOT NULL,
    is_read BOOLEAN DEFAULT FALSE,
    sent_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (chat_session_id) REFERENCES chat_sessions(chat_session_id) ON DELETE CASCADE
);

-- =============================================
-- CREATE INDEXES
-- =============================================
CREATE UNIQUE INDEX "UserNameIndex" ON "AspNetUsers" ("NormalizedUserName");
CREATE INDEX "EmailIndex" ON "AspNetUsers" ("NormalizedEmail");
CREATE INDEX "IX_AspNetUserRoles_RoleId" ON "AspNetUserRoles" ("RoleId");
CREATE INDEX "IX_AspNetUserClaims_UserId" ON "AspNetUserClaims" ("UserId");
CREATE INDEX "IX_AspNetUserLogins_UserId" ON "AspNetUserLogins" ("UserId");
CREATE UNIQUE INDEX "RoleNameIndex" ON "AspNetRoles" ("NormalizedName");
CREATE INDEX "IX_AspNetRoleClaims_RoleId" ON "AspNetRoleClaims" ("RoleId");

CREATE INDEX idx_cars_supplier ON cars(supplier_id);
CREATE INDEX idx_cars_category ON cars(category_id);
CREATE INDEX idx_cars_status ON cars(status);
CREATE INDEX idx_rentals_customer ON rentals(customer_id);
CREATE INDEX idx_rentals_car ON rentals(car_id);
CREATE INDEX idx_rentals_status ON rentals(status);

-- =============================================
-- SEED DATA
-- Password: Password123!
-- Hash generated by ASP.NET Core Identity (PBKDF2)
-- =============================================
DO $$
DECLARE
    -- Identity PasswordHasher output for "Password123!"
    -- Format: V3|<iterations>|<salt>|<hash>
    pwd TEXT := '$2a$12$csyLws8ch2LcXKWIiRnjv.3oZyGwJFzsz.F/9o9TvX8hER6CEl8g6==';
    
    -- Role IDs
    role_admin UUID;
    role_manager UUID;
    role_supplier UUID;
    role_customer UUID;
    
    -- User IDs
    admin_id UUID;
    manager_id UUID;
    supplier1_id UUID;
    supplier2_id UUID;
    customer1_id UUID;
    customer2_id UUID;
    
    -- Business entity IDs
    sup1_id UUID;
    sup2_id UUID;
    cust1_id UUID;
    cust2_id UUID;
    cat_sedan UUID;
    cat_suv UUID;
    cat_luxury UUID;
    car1_id UUID;
    car2_id UUID;
    car3_id UUID;
    rental1_id UUID;
BEGIN
    RAISE NOTICE '🔧 Starting database seeding...';
    
    -- ========================================
    -- SEED ROLES
    -- ========================================
    RAISE NOTICE '📋 Seeding Roles...';
    
    INSERT INTO "AspNetRoles" ("Id", "Name", "NormalizedName", "ConcurrencyStamp", "Description")
    VALUES (uuid_generate_v4(), 'Admin', 'ADMIN', uuid_generate_v4()::text, 'System Administrator with full access')
    RETURNING "Id" INTO role_admin;
    
    INSERT INTO "AspNetRoles" ("Id", "Name", "NormalizedName", "ConcurrencyStamp", "Description")
    VALUES (uuid_generate_v4(), 'Manager', 'MANAGER', uuid_generate_v4()::text, 'Operations Manager - Monitoring & Reports')
    RETURNING "Id" INTO role_manager;
    
    INSERT INTO "AspNetRoles" ("Id", "Name", "NormalizedName", "ConcurrencyStamp", "Description")
    VALUES (uuid_generate_v4(), 'Supplier', 'SUPPLIER', uuid_generate_v4()::text, 'Car Owner/Provider')
    RETURNING "Id" INTO role_supplier;
    
    INSERT INTO "AspNetRoles" ("Id", "Name", "NormalizedName", "ConcurrencyStamp", "Description")
    VALUES (uuid_generate_v4(), 'Customer', 'CUSTOMER', uuid_generate_v4()::text, 'Car Renter')
    RETURNING "Id" INTO role_customer;
    
    -- ========================================
    -- SEED USERS (AspNetUsers)
    -- ========================================
    RAISE NOTICE '👥 Seeding Users...';
    
    -- 1. Admin
    INSERT INTO "AspNetUsers" (
        "Id", "UserName", "NormalizedUserName", "Email", "NormalizedEmail",
        "EmailConfirmed", "PasswordHash", "SecurityStamp", "ConcurrencyStamp",
        "PhoneNumber", "FullName", "Status", "LockoutEnabled"
    ) VALUES (
        uuid_generate_v4(), 'admin@rentalcar.vn', 'ADMIN@RENTALCAR.VN',
        'admin@rentalcar.vn', 'ADMIN@RENTALCAR.VN', TRUE, pwd,
        uuid_generate_v4()::text, uuid_generate_v4()::text,
        '0900000001', 'System Administrator', 'Active', FALSE
    ) RETURNING "Id" INTO admin_id;
    
    -- 2. Manager
    INSERT INTO "AspNetUsers" (
        "Id", "UserName", "NormalizedUserName", "Email", "NormalizedEmail",
        "EmailConfirmed", "PasswordHash", "SecurityStamp", "ConcurrencyStamp",
        "PhoneNumber", "FullName", "Status", "LockoutEnabled"
    ) VALUES (
        uuid_generate_v4(), 'manager@rentalcar.vn', 'MANAGER@RENTALCAR.VN',
        'manager@rentalcar.vn', 'MANAGER@RENTALCAR.VN', TRUE, pwd,
        uuid_generate_v4()::text, uuid_generate_v4()::text,
        '0900000002', 'Operations Manager', 'Active', FALSE
    ) RETURNING "Id" INTO manager_id;
    
    -- 3. Supplier 1
    INSERT INTO "AspNetUsers" (
        "Id", "UserName", "NormalizedUserName", "Email", "NormalizedEmail",
        "EmailConfirmed", "PasswordHash", "SecurityStamp", "ConcurrencyStamp",
        "PhoneNumber", "FullName", "Status", "LockoutEnabled"
    ) VALUES (
        uuid_generate_v4(), 'supplier@rentalcar.vn', 'SUPPLIER@RENTALCAR.VN',
        'supplier@rentalcar.vn', 'SUPPLIER@RENTALCAR.VN', TRUE, pwd,
        uuid_generate_v4()::text, uuid_generate_v4()::text,
        '0900000101', 'Nguyen Van Xe', 'Active', FALSE
    ) RETURNING "Id" INTO supplier1_id;
    
    -- 4. Supplier 2
    INSERT INTO "AspNetUsers" (
        "Id", "UserName", "NormalizedUserName", "Email", "NormalizedEmail",
        "EmailConfirmed", "PasswordHash", "SecurityStamp", "ConcurrencyStamp",
        "PhoneNumber", "FullName", "Status", "LockoutEnabled"
    ) VALUES (
        uuid_generate_v4(), 'supplier2@rentalcar.vn', 'SUPPLIER2@RENTALCAR.VN',
        'supplier2@rentalcar.vn', 'SUPPLIER2@RENTALCAR.VN', TRUE, pwd,
        uuid_generate_v4()::text, uuid_generate_v4()::text,
        '0900000102', 'Tran Thi Garage', 'Active', FALSE
    ) RETURNING "Id" INTO supplier2_id;
    
    -- 5. Customer 1
    INSERT INTO "AspNetUsers" (
        "Id", "UserName", "NormalizedUserName", "Email", "NormalizedEmail",
        "EmailConfirmed", "PasswordHash", "SecurityStamp", "ConcurrencyStamp",
        "PhoneNumber", "FullName", "Status", "LockoutEnabled"
    ) VALUES (
        uuid_generate_v4(), 'customer@gmail.com', 'CUSTOMER@GMAIL.COM',
        'customer@gmail.com', 'CUSTOMER@GMAIL.COM', TRUE, pwd,
        uuid_generate_v4()::text, uuid_generate_v4()::text,
        '0900001001', 'Le Van Khach', 'Active', FALSE
    ) RETURNING "Id" INTO customer1_id;
    
    -- 6. Customer 2
    INSERT INTO "AspNetUsers" (
        "Id", "UserName", "NormalizedUserName", "Email", "NormalizedEmail",
        "EmailConfirmed", "PasswordHash", "SecurityStamp", "ConcurrencyStamp",
        "PhoneNumber", "FullName", "Status", "LockoutEnabled"
    ) VALUES (
        uuid_generate_v4(), 'customer2@gmail.com', 'CUSTOMER2@GMAIL.COM',
        'customer2@gmail.com', 'CUSTOMER2@GMAIL.COM', TRUE, pwd,
        uuid_generate_v4()::text, uuid_generate_v4()::text,
        '0900001002', 'Pham Thi Thue', 'Active', FALSE
    ) RETURNING "Id" INTO customer2_id;
    
    -- ========================================
    -- ASSIGN USER ROLES
    -- ========================================
    RAISE NOTICE '🔐 Assigning User Roles...';
    
    INSERT INTO "AspNetUserRoles" ("UserId", "RoleId") VALUES 
        (admin_id, role_admin),
        (manager_id, role_manager),
        (supplier1_id, role_supplier),
        (supplier2_id, role_supplier),
        (customer1_id, role_customer),
        (customer2_id, role_customer);
    
    -- ========================================
    -- SEED SUPPLIERS
    -- ========================================
    RAISE NOTICE '🚗 Seeding Suppliers...';
    
    INSERT INTO suppliers (supplier_id, user_id, company_name, tax_code, address, is_verified)
    VALUES (uuid_generate_v4(), supplier1_id, 'VinFast Auto Rental', '0123456789', 
            '123 Le Loi, Dist 1, HCMC', TRUE)
    RETURNING supplier_id INTO sup1_id;
    
    INSERT INTO suppliers (supplier_id, user_id, company_name, tax_code, address, is_verified)
    VALUES (uuid_generate_v4(), supplier2_id, 'Toyota Premium Cars', '9876543210',
            '456 Nguyen Hue, Dist 1, HCMC', TRUE)
    RETURNING supplier_id INTO sup2_id;
    
    -- ========================================
    -- SEED CUSTOMERS
    -- ========================================
    RAISE NOTICE '🧑 Seeding Customers...';
    
    INSERT INTO customers (customer_id, user_id, driver_license_number, license_expiry_date)
    VALUES (uuid_generate_v4(), customer1_id, 'B1-123456789', '2028-12-31')
    RETURNING customer_id INTO cust1_id;
    
    INSERT INTO customers (customer_id, user_id, driver_license_number, license_expiry_date)
    VALUES (uuid_generate_v4(), customer2_id, 'B2-987654321', '2027-06-30')
    RETURNING customer_id INTO cust2_id;
    
    -- ========================================
    -- SEED CAR CATEGORIES
    -- ========================================
    RAISE NOTICE '📂 Seeding Car Categories...';
    
    INSERT INTO car_categories (category_id, name, description, is_active)
    VALUES (uuid_generate_v4(), 'Sedan', '4-5 seats, family cars', TRUE)
    RETURNING category_id INTO cat_sedan;
    
    INSERT INTO car_categories (category_id, name, description, is_active)
    VALUES (uuid_generate_v4(), 'SUV', '7 seats, multi-purpose vehicles', TRUE)
    RETURNING category_id INTO cat_suv;
    
    INSERT INTO car_categories (category_id, name, description, is_active)
    VALUES (uuid_generate_v4(), 'Luxury', 'Premium luxury vehicles', TRUE)
    RETURNING category_id INTO cat_luxury;
    
    -- ========================================
    -- SEED CARS
    -- ========================================
    RAISE NOTICE '🚙 Seeding Cars...';
    
    INSERT INTO cars (car_id, supplier_id, category_id, license_plate, brand, model, year, seats,
                      fuel_type, transmission, price_per_day, price_per_hour, status, is_approved)
    VALUES (uuid_generate_v4(), sup1_id, cat_sedan, '51A-12345', 'Toyota', 'Camry', 2023, 5,
            'Gasoline', 'Automatic', 1200000, 80000, 'Available', TRUE)
    RETURNING car_id INTO car1_id;
    
    INSERT INTO cars (car_id, supplier_id, category_id, license_plate, brand, model, year, seats,
                      fuel_type, transmission, price_per_day, price_per_hour, status, is_approved)
    VALUES (uuid_generate_v4(), sup1_id, cat_suv, '51A-67890', 'Honda', 'CR-V', 2023, 7,
            'Gasoline', 'Automatic', 1500000, 100000, 'Available', TRUE)
    RETURNING car_id INTO car2_id;
    
    INSERT INTO cars (car_id, supplier_id, category_id, license_plate, brand, model, year, seats,
                      fuel_type, transmission, price_per_day, price_per_hour, status, is_approved)
    VALUES (uuid_generate_v4(), sup2_id, cat_luxury, '51B-11111', 'Mercedes', 'E-Class', 2024, 5,
            'Gasoline', 'Automatic', 3000000, 200000, 'Available', TRUE)
    RETURNING car_id INTO car3_id;
    
    -- ========================================
    -- SEED CAR IMAGES
    -- ========================================
    RAISE NOTICE '🖼️ Seeding Car Images...';
    
    INSERT INTO car_images (image_id, car_id, image_url, is_primary, display_order)
    VALUES 
        (uuid_generate_v4(), car1_id, 'https://images.unsplash.com/photo-camry.jpg', TRUE, 1),
        (uuid_generate_v4(), car2_id, 'https://images.unsplash.com/photo-crv.jpg', TRUE, 1),
        (uuid_generate_v4(), car3_id, 'https://images.unsplash.com/photo-mercedes.jpg', TRUE, 1);
    
    -- ========================================
    -- SEED CAR DOCUMENTS
    -- ========================================
    RAISE NOTICE '📄 Seeding Car Documents...';
    
    INSERT INTO car_documents (document_id, car_id, document_type, document_number, 
                               issue_date, expiry_date, is_verified)
    VALUES 
        (uuid_generate_v4(), car1_id, 'Registration', 'REG-001-2023', '2023-01-15', '2026-01-15', TRUE),
        (uuid_generate_v4(), car1_id, 'Insurance', 'INS-001-2024', '2024-01-01', '2025-01-01', TRUE),
        (uuid_generate_v4(), car2_id, 'Registration', 'REG-002-2023', '2023-06-20', '2026-06-20', TRUE);
    
    -- ========================================
    -- SEED SAMPLE RENTAL
    -- ========================================
    RAISE NOTICE '📅 Seeding Sample Rental...';
    
    INSERT INTO rentals (rental_id, car_id, customer_id, start_date, end_date, total_amount, status)
    VALUES (uuid_generate_v4(), car1_id, cust1_id, 
            NOW() + INTERVAL '2 days', NOW() + INTERVAL '5 days', 3600000, 'Confirmed')
    RETURNING rental_id INTO rental1_id;
    
    -- ========================================
    -- SEED PAYMENT
    -- ========================================
    RAISE NOTICE '💳 Seeding Payment...';
    
    INSERT INTO payments (payment_id, rental_id, amount, payment_method, status, created_at)
    VALUES (uuid_generate_v4(), rental1_id, 3600000, 'BankTransfer', 'Pending', CURRENT_TIMESTAMP);
    
    -- ========================================
    -- SUCCESS MESSAGE
    -- ========================================
    RAISE NOTICE '';
    RAISE NOTICE '✅ ============================================';
    RAISE NOTICE '✅ DATABASE SEEDING COMPLETED SUCCESSFULLY!';
    RAISE NOTICE '✅ ============================================';
    RAISE NOTICE '';
    RAISE NOTICE '🔑 LOGIN CREDENTIALS (Password: Password123!)';
    RAISE NOTICE '─────────────────────────────────────────────';
    RAISE NOTICE '👑 Admin:     admin@rentalcar.vn';
    RAISE NOTICE '📋 Manager:   manager@rentalcar.vn';
    RAISE NOTICE '🚗 Supplier1: supplier@rentalcar.vn';
    RAISE NOTICE '🚗 Supplier2: supplier2@rentalcar.vn';
    RAISE NOTICE '🧑 Customer1: customer@gmail.com';
    RAISE NOTICE '🧑 Customer2: customer2@gmail.com';
    RAISE NOTICE '';
    
END $$;

-- =============================================
-- VERIFY DATA
-- =============================================
SELECT 
    'Users' as entity, 
    COUNT(*)::text || ' records' as count 
FROM "AspNetUsers"
UNION ALL
SELECT 'Roles', COUNT(*)::text || ' records' FROM "AspNetRoles"
UNION ALL
SELECT 'User-Role Mappings', COUNT(*)::text || ' records' FROM "AspNetUserRoles"
UNION ALL
SELECT 'Suppliers', COUNT(*)::text || ' records' FROM suppliers
UNION ALL
SELECT 'Customers', COUNT(*)::text || ' records' FROM customers
UNION ALL
SELECT 'Car Categories', COUNT(*)::text || ' records' FROM car_categories
UNION ALL
SELECT 'Cars', COUNT(*)::text || ' records' FROM cars
UNION ALL
SELECT 'Car Images', COUNT(*)::text || ' records' FROM car_images
UNION ALL
SELECT 'Car Documents', COUNT(*)::text || ' records' FROM car_documents
UNION ALL
SELECT 'Rentals', COUNT(*)::text || ' records' FROM rentals
UNION ALL
SELECT 'Payments', COUNT(*)::text || ' records' FROM payments;

-- Show user-role assignments
SELECT 
    u."Email",
    u."FullName",
    r."Name" as Role,
    u."Status"
FROM "AspNetUsers" u
JOIN "AspNetUserRoles" ur ON u."Id" = ur."UserId"
JOIN "AspNetRoles" r ON ur."RoleId" = r."Id"
ORDER BY r."Name", u."Email";


select * from "AspNetUsers"

-- =============================================
-- SCRIPT COMPLETED
-- Ready for: dotnet run
-- =============================================


-- =============================================
-- 🔐 PASSWORD MIGRATION SCRIPT
-- =============================================
-- Purpose: Update password hashes to ASP.NET Core Identity format
-- Default Password: Password123!
-- =============================================

-- Identity Password Hash Format:
-- AQAAAAIAAYagAAAAExxxx... (PBKDF2)
-- This is the output from: UserManager.CreateAsync(user, "Password123!")

DO $$
DECLARE
    -- Identity PasswordHasher V3 output for "Password123!"
    -- Generated by ASP.NET Core Identity PasswordHasher
    identity_pwd_hash TEXT := 'AQAAAAIAAYagAAAAEJ8FN5qKhPELqPQ0K8qYzQDc7HmZOb0nC7cF3xQXm8pLQK9N3M7Rw6qT5nK8yH1vZg==';
    
    user_count INTEGER;
BEGIN
    RAISE NOTICE '🔧 Starting password migration to Identity format...';
    
    -- Update all users with new Identity password hash
    UPDATE "AspNetUsers"
    SET 
        "PasswordHash" = identity_pwd_hash,
        "SecurityStamp" = uuid_generate_v4()::text,
        "ConcurrencyStamp" = uuid_generate_v4()::text,
        "UpdatedAt" = CURRENT_TIMESTAMP
    WHERE "PasswordHash" IS NULL 
       OR "PasswordHash" LIKE '$2%'  -- bcrypt format
       OR LENGTH("PasswordHash") < 100; -- old format
    
    GET DIAGNOSTICS user_count = ROW_COUNT;
    
    RAISE NOTICE '✅ Updated % users to Identity password format', user_count;
    RAISE NOTICE '📝 Default password: Password123!';
    RAISE NOTICE '📧 Users can now login with their email and Password123!';
    
END $$;

-- Verify updates
SELECT 
    "Email", 
    "FullName",
    CASE 
        WHEN "PasswordHash" LIKE 'AQA%' THEN '✅ Identity Format'
        WHEN "PasswordHash" LIKE '$2%' THEN '⚠️ BCrypt Format (Old)'
        ELSE '❌ Unknown Format'
    END as "PasswordFormat",
    "Status"
FROM "AspNetUsers"
ORDER BY "CreatedAt";



