
CREATE OR REPLACE FUNCTION public.create_order(
    p_customer_name TEXT,
    p_order_date TIMESTAMPTZ,
    p_created_by TEXT
) RETURNS INT AS $$
DECLARE
    new_order_id INT;
BEGIN
    INSERT INTO "Order" (
        "CustomerName",
        "OrderDate",
        "TotalAmount",
        "CreatedBy",
        "CreatedDate",
        "IsActive",
        "IsDeleted"
    ) VALUES (
        p_customer_name,
        p_order_date,
        0, 
        p_created_by,
        CURRENT_TIMESTAMP,
        TRUE,
        FALSE
    )
    RETURNING "Id" INTO new_order_id;

    RETURN new_order_id;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION public.update_order(
    p_order_id INT,
    p_customer_name TEXT DEFAULT NULL,
    p_order_date TIMESTAMPTZ DEFAULT NULL,
    p_updated_by TEXT DEFAULT NULL
) RETURNS TABLE (
    error_code INT,
    message TEXT
)
LANGUAGE plpgsql
AS $$
BEGIN
    UPDATE "Order"
    SET 
        "CustomerName" = COALESCE(p_customer_name, "CustomerName"),
        "OrderDate" = COALESCE(p_order_date, "OrderDate"),
        "UpdatedBy" = COALESCE(p_updated_by, "UpdatedBy"),
        "UpdatedDate" = CURRENT_TIMESTAMP
    WHERE "Id" = p_order_id;

    IF FOUND THEN
        RETURN QUERY SELECT 0, 'Order updated successfully';
    ELSE
        RETURN QUERY SELECT 1, 'Order not found';
    END IF;

EXCEPTION WHEN OTHERS THEN
    RETURN QUERY SELECT 1, SQLERRM;
END;
$$;

CREATE OR REPLACE FUNCTION public.delete_order(
    p_order_id INT
) RETURNS TABLE (
    error_code INT,
    message TEXT
)
LANGUAGE plpgsql
AS $$
BEGIN
   UPDATE "Order" SET "IsDeleted" = TRUE WHERE "Id" = p_order_id;

    IF FOUND THEN
        RETURN QUERY SELECT 0, 'Order deleted successfully';
    ELSE
        RETURN QUERY SELECT 1, 'Order not found';
    END IF;

EXCEPTION WHEN OTHERS THEN
    RETURN QUERY SELECT 1, SQLERRM;
END;
$$;
CREATE OR REPLACE FUNCTION add_order_item(
    p_order_id INT,
    p_item_id INT,
    p_product_name TEXT,
    p_quantity INT,
    p_unit_price NUMERIC
)
RETURNS INT AS $$
DECLARE
    new_order_item_id INT;
BEGIN
    INSERT INTO "OrderItem" ("OrderId", "ItemId", "ProductName", "Quantity", "UnitPrice")
    VALUES (p_order_id, p_item_id, p_product_name, p_quantity, p_unit_price)
    RETURNING "Id" INTO new_order_item_id;

    -- Update total amount in the parent order
    UPDATE "Order"
    SET "TotalAmount" = (
        SELECT COALESCE(SUM("Quantity" * "UnitPrice"), 0)
        FROM "OrderItem"
        WHERE "OrderId" = p_order_id AND "IsActive" = TRUE AND "IsDeleted" = FALSE
    )
    WHERE "Id" = p_order_id;

    RETURN new_order_item_id;
END;
$$ LANGUAGE plpgsql;


CREATE OR REPLACE FUNCTION public.update_order_item(
    p_id INT,
    p_product_name TEXT,
    p_quantity INT,
    p_unit_price NUMERIC,
    p_updated_by TEXT
)
RETURNS TABLE (
    error_code INT,
    message TEXT
)
LANGUAGE plpgsql
AS $$
BEGIN
    UPDATE "OrderItem"
    SET
        "ProductName" = COALESCE(NULLIF(p_product_name, ''), "ProductName"),
        "Quantity" = COALESCE(p_quantity, "Quantity"),
        "UnitPrice" = COALESCE(p_unit_price, "UnitPrice"),
        "UpdatedBy" = p_updated_by,
        "UpdatedDate" = CURRENT_TIMESTAMP
    WHERE "Id" = p_id AND "IsDeleted" = FALSE;

    IF FOUND THEN
        RETURN QUERY SELECT 0, 'Order item updated successfully';
    ELSE
        RETURN QUERY SELECT 1, 'Order item not found';
    END IF;

EXCEPTION WHEN OTHERS THEN
    RETURN QUERY SELECT 1, SQLERRM;
END;
$$;
CREATE OR REPLACE FUNCTION public.delete_order_item(
    p_id INT
)
RETURNS TABLE (
    error_code INT,
    message TEXT
)
LANGUAGE plpgsql
AS $$
BEGIN
    UPDATE "OrderItem" SET "IsDeleted" = TRUE WHERE "Id" = p_order_id;

    IF FOUND THEN
        RETURN QUERY SELECT 0, 'Order item deleted successfully';
    ELSE
        RETURN QUERY SELECT 1, 'Order item not found';
    END IF;

EXCEPTION WHEN OTHERS THEN
    RETURN QUERY SELECT 1, SQLERRM;
END;
$$;
CREATE OR REPLACE FUNCTION delete_order_with_items(p_order_id INT)
RETURNS TABLE (
    error_code INT,
    message TEXT
)
LANGUAGE plpgsql
AS $$
BEGIN
    UPDATE "OrderItem"
	SET "IsDeleted" = TRUE
	WHERE "OrderId" = p_order_id;
	
	UPDATE "Order"
	SET "IsDeleted" = TRUE
	WHERE "Id" = p_order_id;

    IF FOUND THEN
        RETURN QUERY SELECT 0, 'Order and related order items deleted successfully.';
    ELSE
        RETURN QUERY SELECT 1, 'Order not found.';
    END IF;

EXCEPTION WHEN OTHERS THEN
    RETURN QUERY SELECT 1, SQLERRM;
END;
$$;
CREATE OR REPLACE FUNCTION get_order_with_items(p_order_id INT)
RETURNS TABLE (
    Id INT,
    CustomerName TEXT,
    OrderDate TIMESTAMPTZ,
    TotalAmount NUMERIC,
    CreatedBy TEXT,
    CreatedDate TIMESTAMPTZ,
    UpdatedBy TEXT,
    UpdatedDate TIMESTAMPTZ,
    IsActive BOOLEAN,
    IsDeleted BOOLEAN,
    ItemId INT,
    ProductName TEXT,
    Quantity INT,
    UnitPrice NUMERIC,
    ItemCreatedBy TEXT,
    ItemCreatedDate TIMESTAMPTZ,
    ItemUpdatedBy TEXT,
    ItemUpdatedDate TIMESTAMPTZ,
    ItemIsActive BOOLEAN,
    ItemIsDeleted BOOLEAN
)
LANGUAGE sql
AS $$
    SELECT
        o."Id" AS "Id",
        o."CustomerName" AS "CustomerName",
        o."OrderDate" AS "OrderDate",
        o."TotalAmount" AS "TotalAmount",
        o."CreatedBy" AS "CreatedBy",
        o."CreatedDate" AS "CreatedDate",
        o."UpdatedBy" AS "UpdatedBy",
        o."UpdatedDate" AS "UpdatedDate",
        o."IsActive" AS "IsActive",
        o."IsDeleted" AS "IsDeleted",
        oi."Id" AS "ItemId",
        oi."ProductName" AS "ProductName",
        oi."Quantity" AS "Quantity",
        oi."UnitPrice" AS "UnitPrice",
        oi."CreatedBy" AS "ItemCreatedBy",
        oi."CreatedDate" AS "ItemCreatedDate",
        oi."UpdatedBy" AS "ItemUpdatedBy",
        oi."UpdatedDate" AS "ItemUpdatedDate",
        oi."IsActive" AS "ItemIsActive",
        oi."IsDeleted" AS "ItemIsDeleted"
    FROM "Order" o
    LEFT JOIN "OrderItem" oi 
        ON oi."OrderId" = o."Id" 
       AND oi."IsDeleted" = FALSE 
       AND oi."IsActive" = TRUE
    WHERE o."Id" = p_order_id 
      AND o."IsDeleted" = FALSE 
      AND o."IsActive" = TRUE;
$$;

CREATE OR REPLACE FUNCTION get_all_orders()
RETURNS TABLE (
    Id INT,
    CustomerName TEXT,
    OrderDate TIMESTAMPTZ,
    TotalAmount NUMERIC,
    CreatedBy TEXT,
    CreatedDate TIMESTAMPTZ,
    UpdatedBy TEXT,
    UpdatedDate TIMESTAMPTZ,
    IsActive BOOLEAN,
    IsDeleted BOOLEAN,
    ItemId INT,
    ProductName TEXT,
    Quantity INT,
    UnitPrice NUMERIC,
    ItemCreatedBy TEXT,
    ItemCreatedDate TIMESTAMPTZ,
    ItemUpdatedBy TEXT,
    ItemUpdatedDate TIMESTAMPTZ,
    ItemIsActive BOOLEAN,
    ItemIsDeleted BOOLEAN
)
LANGUAGE sql
AS $$
    SELECT
        o."Id",
        o."CustomerName",
        o."OrderDate",
        o."TotalAmount",
        o."CreatedBy",
        o."CreatedDate",
        o."UpdatedBy",
        o."UpdatedDate",
        o."IsActive",
        o."IsDeleted",
        oi."Id" AS "ItemId",
        oi."ProductName",
        oi."Quantity",
        oi."UnitPrice",
        oi."CreatedBy" AS "ItemCreatedBy",
        oi."CreatedDate" AS "ItemCreatedDate",
        oi."UpdatedBy" AS "ItemUpdatedBy",
        oi."UpdatedDate" AS "ItemUpdatedDate",
        oi."IsActive" AS "ItemIsActive",
        oi."IsDeleted" AS "ItemIsDeleted"
    FROM "Order" o
    LEFT JOIN "OrderItem" oi 
        ON oi."OrderId" = o."Id" 
       AND oi."IsDeleted" = FALSE 
       AND oi."IsActive" = TRUE
    WHERE o."IsDeleted" = FALSE 
      AND o."IsActive" = TRUE;
$$;

CREATE TYPE user_role AS ENUM ('Admin', 'User');

CREATE TABLE public."Login" (
    "Id" SERIAL PRIMARY KEY,
    "Username" TEXT NOT NULL UNIQUE,
    "Password" TEXT NOT NULL,
    "CreatedDate" TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "IsActive" BOOLEAN NOT NULL DEFAULT TRUE,
    "IsDeleted" BOOLEAN NOT NULL DEFAULT FALSE,
    "Role" user_role NOT NULL DEFAULT 'User' 
);

CREATE OR REPLACE FUNCTION public.create_user(
    p_username TEXT,
    p_password TEXT
) RETURNS INT AS $$
DECLARE
    new_user_id INT;
BEGIN
    INSERT INTO public."Login" (
        "Username",
        "Password",
        "CreatedDate",
        "IsActive",
        "IsDeleted"
    ) VALUES (
        p_username,
        p_password,
        CURRENT_TIMESTAMP,
        TRUE,
        FALSE
    )
    RETURNING "Id" INTO new_user_id;

    RETURN new_user_id;
END;
$$ LANGUAGE plpgsql;
INSERT INTO "Login" ("Username", "Password", "Role")
VALUES ('admin', 'AQAAAAIAAYagAAAAELNB3mRiMWMzOBD3L2ECUp/KEjPr1ku6r9EMPVc2OA3qM1eSpim4OZ6mTJfX832fYg==
', 'Admin');
CREATE TABLE inventory.items (
    item_id SERIAL PRIMARY KEY,
    item_name TEXT NOT NULL,
    item_description TEXT,
    item_category TEXT,
    created_by TEXT NOT NULL,
    created_date TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_by TEXT,
    updated_date TIMESTAMPTZ,
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    is_deleted BOOLEAN NOT NULL DEFAULT FALSE
);
CREATE SCHEMA IF NOT EXISTS inventory AUTHORIZATION postgres;
CREATE OR REPLACE FUNCTION inventory.create_item(
    p_item_name TEXT,
    p_item_description TEXT,
    p_item_category TEXT
)
RETURNS TABLE(new_item_id INT, err_code INT, msg TEXT)
LANGUAGE plpgsql
AS $$
DECLARE
    inserted_id INT;
BEGIN
    INSERT INTO inventory.items(item_name, item_description, item_category, created_by)
    VALUES (p_item_name, p_item_description, p_item_category, 'Admin')
    RETURNING item_id INTO inserted_id;

    RETURN QUERY SELECT inserted_id AS new_item_id, 0 AS err_code, 'Item created successfully' AS msg;

EXCEPTION WHEN OTHERS THEN
    RETURN QUERY SELECT NULL::INT AS new_item_id, 1 AS err_code, SQLERRM::TEXT AS msg;
END;
$$;


CREATE OR REPLACE FUNCTION inventory.delete_item(
    p_item_id INT
)
RETURNS TABLE(error_code INT, message TEXT)
LANGUAGE plpgsql
AS $$
BEGIN
    UPDATE inventory.items
    SET is_deleted = TRUE
    WHERE item_id = p_item_id
      AND is_deleted = FALSE;

    IF FOUND THEN
        RETURN QUERY SELECT 0, 'Item deleted successfully';
    ELSE
        RETURN QUERY SELECT 1, 'Item not found or already deleted';
    END IF;
END;
$$;


CREATE OR REPLACE FUNCTION inventory.update_item(
    p_item_id INT,
    p_item_name TEXT,
    p_item_description TEXT,
    p_item_category TEXT,
    p_updated_by TEXT
)
RETURNS TABLE(error_code INT, message TEXT)
LANGUAGE plpgsql
AS $$
BEGIN
    UPDATE inventory.items
    SET item_name = p_item_name,
        item_description = p_item_description,
        item_category = p_item_category,
        updated_date = NOW(),
        updated_by = p_updated_by
    WHERE item_id = p_item_id
      AND is_deleted = FALSE;

    IF FOUND THEN
        RETURN QUERY SELECT 0, 'Item updated successfully';
    ELSE
        RETURN QUERY SELECT 1, 'Item not found or deleted';
    END IF;
END;
$$;

CREATE OR REPLACE FUNCTION inventory.get_items()
RETURNS TABLE(
    item_id INT,
    item_name TEXT,
    item_description TEXT,
    item_category TEXT,
    created_date TIMESTAMPTZ,
    created_by TEXT
)
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY
    SELECT i.item_id,
           i.item_name,
           i.item_description,
           i.item_category,
           i.created_date,
           i.created_by
    FROM inventory.items i
    WHERE i.is_deleted = FALSE
    ORDER BY i.created_date DESC;
END;
$$;
CREATE TABLE inventory.stock (
    stock_id SERIAL PRIMARY KEY,
    item_id INT NOT NULL REFERENCES inventory.items(item_id) ON DELETE CASCADE,
    warehouse_location TEXT NOT NULL DEFAULT 'Main Warehouse',
    quantity INT NOT NULL DEFAULT 0,
    created_date TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_date TIMESTAMPTZ,
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    is_deleted BOOLEAN NOT NULL DEFAULT FALSE
);
CREATE OR REPLACE FUNCTION inventory.create_stock(
    p_item_id INT
)
RETURNS TABLE(error_code INT, message TEXT)
LANGUAGE plpgsql
AS $$
BEGIN
    INSERT INTO inventory.stock (item_id, quantity)
    VALUES (p_item_id, 0);

    RETURN QUERY SELECT 0, 'Stock created successfully';
EXCEPTION
    WHEN OTHERS THEN
        RETURN QUERY SELECT 1, SQLERRM;
END;
$$;
CREATE OR REPLACE FUNCTION inventory.update_stock_quantity(
    p_stock_id INT,
    p_quantity INT
)
RETURNS TABLE(error_code INT, message TEXT)
LANGUAGE plpgsql
AS $$
BEGIN
    UPDATE inventory.stock
    SET quantity = p_quantity,
        updated_date = NOW()
    WHERE stock_id = p_stock_id
      AND is_deleted = FALSE;

    IF FOUND THEN
        RETURN QUERY SELECT 0, 'Stock quantity updated successfully';
    ELSE
        RETURN QUERY SELECT 1, 'Stock not found or deleted';
    END IF;
END;
$$;
CREATE OR REPLACE FUNCTION inventory.delete_stock(
    p_stock_id INT
)
RETURNS TABLE(error_code INT, message TEXT)
LANGUAGE plpgsql
AS $$
BEGIN
    UPDATE inventory.stock
    SET is_deleted = TRUE,
        updated_date = NOW()
    WHERE stock_id = p_stock_id
      AND is_deleted = FALSE;

    IF FOUND THEN
        RETURN QUERY SELECT 0, 'Stock deleted successfully';
    ELSE
        RETURN QUERY SELECT 1, 'Stock not found or already deleted';
    END IF;
END;
$$;
CREATE OR REPLACE FUNCTION inventory.get_stock_quantity(
    p_item_id INT
)
RETURNS INT
LANGUAGE sql
AS $$
    SELECT quantity
    FROM inventory.stock
    WHERE item_id = p_item_id
      AND is_deleted = FALSE;
$$;
CREATE OR REPLACE FUNCTION inventory.get_all_stocks()
RETURNS TABLE(
    stock_id INT,
    item_id INT,
    warehouse_location TEXT,
    quantity INT,
    created_date TIMESTAMPTZ,
    updated_date TIMESTAMPTZ,
    is_active BOOLEAN,
    is_deleted BOOLEAN
)
LANGUAGE sql
AS $$
    SELECT stock_id, item_id, warehouse_location, quantity, created_date, updated_date, is_active, is_deleted
    FROM inventory.stock
    WHERE is_deleted = FALSE
    ORDER BY created_date DESC;
$$;
ALTER TABLE public."Login"
  ADD COLUMN IF NOT EXISTS "RefreshToken" text;

ALTER TABLE public."Login"
  ADD COLUMN IF NOT EXISTS "RefreshTokenExpiryTime" timestamp with time zone;