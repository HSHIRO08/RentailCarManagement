-- =============================================
-- Enable UUID Extension
-- =============================================
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

BEGIN;


CREATE TABLE IF NOT EXISTS public."AspNetRoleClaims"
(
    "Id" serial NOT NULL,
    "RoleId" uuid NOT NULL,
    "ClaimType" text COLLATE pg_catalog."default",
    "ClaimValue" text COLLATE pg_catalog."default",
    CONSTRAINT "AspNetRoleClaims_pkey" PRIMARY KEY ("Id")
);

CREATE TABLE IF NOT EXISTS public."AspNetRoles"
(
    "Id" uuid NOT NULL DEFAULT uuid_generate_v4(),
    "Name" character varying(256) COLLATE pg_catalog."default",
    "NormalizedName" character varying(256) COLLATE pg_catalog."default",
    "ConcurrencyStamp" text COLLATE pg_catalog."default",
    "Description" text COLLATE pg_catalog."default",
    "CreatedAt" timestamp with time zone DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT "AspNetRoles_pkey" PRIMARY KEY ("Id"),
    CONSTRAINT "AspNetRoles_NormalizedName_key" UNIQUE ("NormalizedName")
);

CREATE TABLE IF NOT EXISTS public."AspNetUserClaims"
(
    "Id" serial NOT NULL,
    "UserId" uuid NOT NULL,
    "ClaimType" text COLLATE pg_catalog."default",
    "ClaimValue" text COLLATE pg_catalog."default",
    CONSTRAINT "AspNetUserClaims_pkey" PRIMARY KEY ("Id")
);

CREATE TABLE IF NOT EXISTS public."AspNetUserLogins"
(
    "LoginProvider" text COLLATE pg_catalog."default" NOT NULL,
    "ProviderKey" text COLLATE pg_catalog."default" NOT NULL,
    "ProviderDisplayName" text COLLATE pg_catalog."default",
    "UserId" uuid NOT NULL,
    CONSTRAINT "AspNetUserLogins_pkey" PRIMARY KEY ("LoginProvider", "ProviderKey")
);

CREATE TABLE IF NOT EXISTS public."AspNetUserRoles"
(
    "UserId" uuid NOT NULL,
    "RoleId" uuid NOT NULL,
    CONSTRAINT "AspNetUserRoles_pkey" PRIMARY KEY ("UserId", "RoleId")
);

CREATE TABLE IF NOT EXISTS public."AspNetUserTokens"
(
    "UserId" uuid NOT NULL,
    "LoginProvider" text COLLATE pg_catalog."default" NOT NULL,
    "Name" text COLLATE pg_catalog."default" NOT NULL,
    "Value" text COLLATE pg_catalog."default",
    CONSTRAINT "AspNetUserTokens_pkey" PRIMARY KEY ("UserId", "LoginProvider", "Name")
);

CREATE TABLE IF NOT EXISTS public."AspNetUsers"
(
    "Id" uuid NOT NULL DEFAULT uuid_generate_v4(),
    "UserName" character varying(256) COLLATE pg_catalog."default",
    "NormalizedUserName" character varying(256) COLLATE pg_catalog."default",
    "Email" character varying(256) COLLATE pg_catalog."default",
    "NormalizedEmail" character varying(256) COLLATE pg_catalog."default",
    "EmailConfirmed" boolean NOT NULL DEFAULT true,
    "PasswordHash" text COLLATE pg_catalog."default",
    "SecurityStamp" text COLLATE pg_catalog."default",
    "ConcurrencyStamp" text COLLATE pg_catalog."default",
    "PhoneNumber" text COLLATE pg_catalog."default",
    "PhoneNumberConfirmed" boolean NOT NULL DEFAULT false,
    "TwoFactorEnabled" boolean NOT NULL DEFAULT false,
    "LockoutEnd" timestamp with time zone,
    "LockoutEnabled" boolean NOT NULL DEFAULT false,
    "AccessFailedCount" integer NOT NULL DEFAULT 0,
    "FullName" character varying(100) COLLATE pg_catalog."default" NOT NULL,
    "Status" character varying(20) COLLATE pg_catalog."default" NOT NULL DEFAULT 'Active'::character varying,
    "CreatedAt" timestamp with time zone DEFAULT CURRENT_TIMESTAMP,
    "UpdatedAt" timestamp with time zone,
    CONSTRAINT "AspNetUsers_pkey" PRIMARY KEY ("Id")
);

CREATE TABLE IF NOT EXISTS public.car_categories
(
    category_id uuid NOT NULL DEFAULT uuid_generate_v4(),
    name character varying(100) COLLATE pg_catalog."default" NOT NULL,
    description text COLLATE pg_catalog."default",
    is_active boolean DEFAULT true,
    created_at timestamp with time zone DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT car_categories_pkey PRIMARY KEY (category_id)
);

CREATE TABLE IF NOT EXISTS public.cars
(
    car_id uuid NOT NULL DEFAULT uuid_generate_v4(),
    supplier_id uuid NOT NULL,
    category_id uuid NOT NULL,
    license_plate character varying(20) COLLATE pg_catalog."default" NOT NULL,
    brand character varying(50) COLLATE pg_catalog."default" NOT NULL,
    model character varying(50) COLLATE pg_catalog."default" NOT NULL,
    year integer,
    seats integer,
    fuel_type character varying(50) COLLATE pg_catalog."default",
    transmission character varying(50) COLLATE pg_catalog."default",
    price_per_day numeric(15, 2) NOT NULL,
    price_per_hour numeric(15, 2),
    status character varying(20) COLLATE pg_catalog."default" DEFAULT 'Available'::character varying,
    is_approved boolean DEFAULT false,
    created_at timestamp with time zone DEFAULT CURRENT_TIMESTAMP,
    updated_at timestamp with time zone,
    car_image_url text,
    CONSTRAINT cars_pkey PRIMARY KEY (car_id),
    CONSTRAINT cars_license_plate_key UNIQUE (license_plate)
);

CREATE TABLE IF NOT EXISTS public.chat_sessions
(
    chat_session_id uuid NOT NULL DEFAULT uuid_generate_v4(),
    user_id uuid NOT NULL,
    subject character varying(200) COLLATE pg_catalog."default",
    is_active boolean DEFAULT true,
    started_at timestamp with time zone DEFAULT CURRENT_TIMESTAMP,
    ended_at timestamp with time zone,
    CONSTRAINT chat_sessions_pkey PRIMARY KEY (chat_session_id)
);

CREATE TABLE IF NOT EXISTS public.commissions
(
    commission_id uuid NOT NULL DEFAULT uuid_generate_v4(),
    rental_id uuid NOT NULL,
    supplier_id uuid NOT NULL,
    commission_rate numeric(5, 4) DEFAULT 0.15,
    system_amount numeric(15, 2) NOT NULL,
    supplier_amount numeric(15, 2) NOT NULL,
    created_at timestamp with time zone DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT commissions_pkey PRIMARY KEY (commission_id),
    CONSTRAINT commissions_rental_id_key UNIQUE (rental_id)
);

CREATE TABLE IF NOT EXISTS public.complaints
(
    complaint_id uuid NOT NULL DEFAULT uuid_generate_v4(),
    rental_id uuid NOT NULL,
    user_id uuid NOT NULL,
    title character varying(200) COLLATE pg_catalog."default",
    description text COLLATE pg_catalog."default",
    resolution text COLLATE pg_catalog."default",
    status character varying(50) COLLATE pg_catalog."default" DEFAULT 'Pending'::character varying,
    created_at timestamp with time zone DEFAULT CURRENT_TIMESTAMP,
    resolved_at timestamp with time zone,
    CONSTRAINT complaints_pkey PRIMARY KEY (complaint_id)
);

CREATE TABLE IF NOT EXISTS public.customers
(
    customer_id uuid NOT NULL DEFAULT uuid_generate_v4(),
    user_id uuid NOT NULL,
    driver_license_number character varying(50) COLLATE pg_catalog."default",
    license_expiry_date date,
    address text COLLATE pg_catalog."default",
    created_at timestamp with time zone DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT customers_pkey PRIMARY KEY (customer_id),
    CONSTRAINT customers_user_id_key UNIQUE (user_id)
);

CREATE TABLE IF NOT EXISTS public.payments
(
    payment_id uuid NOT NULL DEFAULT uuid_generate_v4(),
    rental_id uuid NOT NULL,
    amount numeric(15, 2) NOT NULL,
    payment_method character varying(50) COLLATE pg_catalog."default",
    status character varying(20) COLLATE pg_catalog."default" DEFAULT 'Pending'::character varying,
    paid_at timestamp with time zone,
    created_at timestamp with time zone DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT payments_pkey PRIMARY KEY (payment_id),
    CONSTRAINT payments_rental_id_key UNIQUE (rental_id)
);

CREATE TABLE IF NOT EXISTS public.rentals
(
    rental_id uuid NOT NULL DEFAULT uuid_generate_v4(),
    car_id uuid NOT NULL,
    customer_id uuid NOT NULL,
    start_date timestamp with time zone NOT NULL,
    end_date timestamp with time zone NOT NULL,
    total_amount numeric(15, 2) NOT NULL,
    status character varying(20) COLLATE pg_catalog."default" DEFAULT 'Pending'::character varying,
    created_at timestamp with time zone DEFAULT CURRENT_TIMESTAMP,
    updated_at timestamp with time zone,
    CONSTRAINT rentals_pkey PRIMARY KEY (rental_id)
);

CREATE TABLE IF NOT EXISTS public.reviews
(
    review_id uuid NOT NULL DEFAULT uuid_generate_v4(),
    rental_id uuid NOT NULL,
    customer_id uuid NOT NULL,
    rating integer,
    comment text COLLATE pg_catalog."default",
    is_approved boolean DEFAULT true,
    created_at timestamp with time zone DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT reviews_pkey PRIMARY KEY (review_id),
    CONSTRAINT reviews_rental_id_key UNIQUE (rental_id)
);

CREATE TABLE IF NOT EXISTS public.suppliers
(
    supplier_id uuid NOT NULL DEFAULT uuid_generate_v4(),
    user_id uuid NOT NULL,
    company_name character varying(200) COLLATE pg_catalog."default",
    tax_code character varying(50) COLLATE pg_catalog."default",
    address text COLLATE pg_catalog."default",
    bank_account character varying(50) COLLATE pg_catalog."default",
    bank_name character varying(100) COLLATE pg_catalog."default",
    is_verified boolean DEFAULT false,
    created_at timestamp with time zone DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT suppliers_pkey PRIMARY KEY (supplier_id),
    CONSTRAINT suppliers_user_id_key UNIQUE (user_id)
);

ALTER TABLE IF EXISTS public."AspNetRoleClaims"
    ADD CONSTRAINT "AspNetRoleClaims_RoleId_fkey" FOREIGN KEY ("RoleId")
    REFERENCES public."AspNetRoles" ("Id") MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE CASCADE;
CREATE INDEX IF NOT EXISTS "IX_AspNetRoleClaims_RoleId"
    ON public."AspNetRoleClaims"("RoleId");


ALTER TABLE IF EXISTS public."AspNetUserClaims"
    ADD CONSTRAINT "AspNetUserClaims_UserId_fkey" FOREIGN KEY ("UserId")
    REFERENCES public."AspNetUsers" ("Id") MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE CASCADE;
CREATE INDEX IF NOT EXISTS "IX_AspNetUserClaims_UserId"
    ON public."AspNetUserClaims"("UserId");


ALTER TABLE IF EXISTS public."AspNetUserLogins"
    ADD CONSTRAINT "AspNetUserLogins_UserId_fkey" FOREIGN KEY ("UserId")
    REFERENCES public."AspNetUsers" ("Id") MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE CASCADE;
CREATE INDEX IF NOT EXISTS "IX_AspNetUserLogins_UserId"
    ON public."AspNetUserLogins"("UserId");


ALTER TABLE IF EXISTS public."AspNetUserRoles"
    ADD CONSTRAINT "AspNetUserRoles_RoleId_fkey" FOREIGN KEY ("RoleId")
    REFERENCES public."AspNetRoles" ("Id") MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE CASCADE;
CREATE INDEX IF NOT EXISTS "IX_AspNetUserRoles_RoleId"
    ON public."AspNetUserRoles"("RoleId");


ALTER TABLE IF EXISTS public."AspNetUserRoles"
    ADD CONSTRAINT "AspNetUserRoles_UserId_fkey" FOREIGN KEY ("UserId")
    REFERENCES public."AspNetUsers" ("Id") MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE CASCADE;


ALTER TABLE IF EXISTS public."AspNetUserTokens"
    ADD CONSTRAINT "AspNetUserTokens_UserId_fkey" FOREIGN KEY ("UserId")
    REFERENCES public."AspNetUsers" ("Id") MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE CASCADE;


ALTER TABLE IF EXISTS public.cars
    ADD CONSTRAINT cars_category_id_fkey FOREIGN KEY (category_id)
    REFERENCES public.car_categories (category_id) MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE NO ACTION;
CREATE INDEX IF NOT EXISTS idx_cars_category
    ON public.cars(category_id);


ALTER TABLE IF EXISTS public.cars
    ADD CONSTRAINT cars_supplier_id_fkey FOREIGN KEY (supplier_id)
    REFERENCES public.suppliers (supplier_id) MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE NO ACTION;
CREATE INDEX IF NOT EXISTS idx_cars_supplier
    ON public.cars(supplier_id);


ALTER TABLE IF EXISTS public.chat_sessions
    ADD CONSTRAINT chat_sessions_user_id_fkey FOREIGN KEY (user_id)
    REFERENCES public."AspNetUsers" ("Id") MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE NO ACTION;


ALTER TABLE IF EXISTS public.commissions
    ADD CONSTRAINT commissions_rental_id_fkey FOREIGN KEY (rental_id)
    REFERENCES public.rentals (rental_id) MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE NO ACTION;
CREATE INDEX IF NOT EXISTS commissions_rental_id_key
    ON public.commissions(rental_id);


ALTER TABLE IF EXISTS public.commissions
    ADD CONSTRAINT commissions_supplier_id_fkey FOREIGN KEY (supplier_id)
    REFERENCES public.suppliers (supplier_id) MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE NO ACTION;


ALTER TABLE IF EXISTS public.complaints
    ADD CONSTRAINT complaints_rental_id_fkey FOREIGN KEY (rental_id)
    REFERENCES public.rentals (rental_id) MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE NO ACTION;


ALTER TABLE IF EXISTS public.complaints
    ADD CONSTRAINT complaints_user_id_fkey FOREIGN KEY (user_id)
    REFERENCES public."AspNetUsers" ("Id") MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE NO ACTION;


ALTER TABLE IF EXISTS public.customers
    ADD CONSTRAINT customers_user_id_fkey FOREIGN KEY (user_id)
    REFERENCES public."AspNetUsers" ("Id") MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE CASCADE;
CREATE INDEX IF NOT EXISTS customers_user_id_key
    ON public.customers(user_id);


ALTER TABLE IF EXISTS public.payments
    ADD CONSTRAINT payments_rental_id_fkey FOREIGN KEY (rental_id)
    REFERENCES public.rentals (rental_id) MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE NO ACTION;
CREATE INDEX IF NOT EXISTS payments_rental_id_key
    ON public.payments(rental_id);


ALTER TABLE IF EXISTS public.rentals
    ADD CONSTRAINT rentals_car_id_fkey FOREIGN KEY (car_id)
    REFERENCES public.cars (car_id) MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE NO ACTION;
CREATE INDEX IF NOT EXISTS idx_rentals_car
    ON public.rentals(car_id);


ALTER TABLE IF EXISTS public.rentals
    ADD CONSTRAINT rentals_customer_id_fkey FOREIGN KEY (customer_id)
    REFERENCES public.customers (customer_id) MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE NO ACTION;
CREATE INDEX IF NOT EXISTS idx_rentals_customer
    ON public.rentals(customer_id);


ALTER TABLE IF EXISTS public.reviews
    ADD CONSTRAINT reviews_customer_id_fkey FOREIGN KEY (customer_id)
    REFERENCES public.customers (customer_id) MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE NO ACTION;


ALTER TABLE IF EXISTS public.reviews
    ADD CONSTRAINT reviews_rental_id_fkey FOREIGN KEY (rental_id)
    REFERENCES public.rentals (rental_id) MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE NO ACTION;
CREATE INDEX IF NOT EXISTS reviews_rental_id_key
    ON public.reviews(rental_id);


ALTER TABLE IF EXISTS public.suppliers
    ADD CONSTRAINT suppliers_user_id_fkey FOREIGN KEY (user_id)
    REFERENCES public."AspNetUsers" ("Id") MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE CASCADE;
CREATE INDEX IF NOT EXISTS suppliers_user_id_key
    ON public.suppliers(user_id);

END;