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
order, order\_items, login
Ensure all base tables have the default columns:
created\_by
created\_date
updated\_by
updated\_date
is\_active
is\_deleted
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
Use basic validation attributes (e.g., \[Required], \[StringLength]).







For Week 2: Authentication and InventoryService

Goals

Implement JWT authentication, claims.

Add InventoryService with its own schema inventory.

Secure APIs with JWT and claims.

Steps

JWT Setup

Configure token issuance and validation.

Use login table for authentication.

Define claims and roles.

Secure APIs

Apply \[Authorize] and claims policies to OrderService endpoints.

InventoryService Setup

Create inventory schema with tables (items, stock + default audit columns).

Write CRUD stored procedures.

Week 3 : Service Communication and Ocelot Gateway
Goals
· Integrate APIs with Ocelot Gateway.
· Secure routing with JWT.
Steps
1. Setup Gateway Project
o Create ApiGateway project.
o Install Ocelot NuGet.
o Configure ocelot.json with /orders, /inventory routes.
o Forward JWT tokens.
2. Service Config
o Update service URLs and ports.
3. Security
o Secure Gateway with JWT validation.
o Block direct access to APIs (if needed).
4. Testing
o Validate routing and auth via Swagger/Postman.
 
Week 4: Advanced Features and Polish
Goals
· Finalize logging, documentation, and testing.
· Polish and optimize the system.
Steps
1. Enhanced Logging
o Structured logs with correlation IDs, request/response bodies.
2. Documentation
o Complete Swagger setup.
o Create Postman collection.
4 / 5
3. Optional Advanced
o Health checks, readiness probes.
o Rate limiting policies.
 

Setup solution with API, Business, Persistence, Domain.

Configure EF Core + Dapper.

Implement /api/inventory CRUD endpoints with JWT protection.

