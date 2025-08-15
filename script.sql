DROP FUNCTION IF EXISTS add_order_item(integer, text, integer, numeric);
DROP FUNCTION IF EXISTS get_order_with_items(integer);

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
    DELETE FROM "Order"
    WHERE "Id" = p_order_id;

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
    p_product_name TEXT,
    p_quantity INT,
    p_unit_price NUMERIC
)
RETURNS void AS $$
BEGIN

    INSERT INTO "OrderItem" ("OrderId", "ProductName", "Quantity", "UnitPrice")
    VALUES (p_order_id, p_product_name, p_quantity, p_unit_price);


    UPDATE "Order"
    SET "TotalAmount" = (
        SELECT COALESCE(SUM("Quantity" * "UnitPrice"), 0)
        FROM "OrderItem"
        WHERE "OrderId" = p_order_id AND "IsActive" = TRUE AND "IsDeleted" = FALSE
    )
    WHERE "Id" = p_order_id;
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
    DELETE FROM "OrderItem"
    WHERE "Id" = p_id;

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
    DELETE FROM "OrderItem" WHERE "OrderId" = p_order_id;
    DELETE FROM "Order" WHERE "Id" = p_order_id;

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

CREATE OR REPLACE FUNCTION create_user(
    p_username TEXT,
    p_password TEXT
)
RETURNS TABLE (
    error_code INT,
    message TEXT
) AS $$
BEGIN
    IF EXISTS (SELECT 1 FROM "Login" WHERE "Username" = p_username) THEN
        RETURN QUERY SELECT 1, 'Username already exists';
        RETURN;
    END IF;

    INSERT INTO "Login" ("Username", "Password")
    VALUES (p_username, p_password);

    RETURN QUERY SELECT 0, 'User created successfully';
END;
$$ LANGUAGE plpgsql;


CREATE OR REPLACE FUNCTION get_user(
    p_username TEXT,
    p_password TEXT
)
RETURNS TABLE (
    error_code INT,
    message TEXT
) AS $$
BEGIN
    IF EXISTS (
        SELECT 1 FROM "Login"
        WHERE "Username" = p_username AND "Password" = p_password
    ) THEN
        RETURN QUERY SELECT 0, 'Login successful';
    ELSE
        RETURN QUERY SELECT 1, 'Invalid username or password';
    END IF;
END;
$$ LANGUAGE plpgsql;
