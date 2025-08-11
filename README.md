For Week 1, we need to implement the following:
 
Setup the project structure.
Build the first API service (OrderService)
Create schema order with tables and stored procedures.
Configure logging, base response, Swagger.
Implement CRUD APIs with Dapper and basic validation.

Steps:

Environment Setup

Install .NET 6 SDK, PostgreSQL, pgAdmin.
Initialize Git repository.
Install Visual Studio or VS Code.

Project Structure

Create a solution with projects:
API
Business
Persistence
Domain

Database Setup

Create order schema with tables:
order, order_items, login
Ensure all base tables have the default columns:
created_by
created_date
updated_by
updated_date
is_active
is_deleted
Write stored procedures (CRUD) returning { errorCode, data }.

EF Core + Dapper Setup

Configure DbContext and Dapper for stored procedures.
Implement Repository and Unit of Work.
Logging
Setup Serilog (console and files) with correlation IDs.
Base Response Structure
{
  "message": "Operation successful",
  "data": {},
  "errorCode": 0
}
Swagger
Enable and configure annotations.
CRUD API Endpoints
Implement /api/orders CRUD with Dapper.
Use basic validation attributes (e.g., [Required], [StringLength]).
