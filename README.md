\# Microservices Project with Clean Architecture, JWT, and Ocelot Gateway



This project demonstrates a \*\*full-fledged microservices architecture\*\* using \*\*.NET 6\*\*, \*\*PostgreSQL\*\*, and \*\*Ocelot API Gateway\*\*, following \*\*Clean Architecture principles\*\*. It implements two main services (`OrderService` and `InventoryService`) with full CRUD operations, JWT authentication (including refresh tokens), transactional database operations, rate limiting, caching, structured logging, and normalized schemas.



---



\## Table of Contents



\- \[Technologies](#technologies)

\- \[Project Overview](#project-overview)

\- \[Project Structure](#project-structure)

\- \[Database Design](#database-design)

&nbsp; - \[Order Schema](#order-schema)

&nbsp; - \[Inventory Schema](#inventory-schema)

\- \[Authentication \& Authorization](#authentication--authorization)

\- \[Services](#services)

&nbsp; - \[OrderService](#orderservice)

&nbsp; - \[InventoryService](#inventoryservice)

\- \[API Gateway](#api-gateway)

\- \[Logging](#logging)

\- \[Rate Limiting \& Caching](#rate-limiting--caching)

\- \[Documentation](#documentation)

\- \[Advanced Features](#advanced-features)

\- \[Setup Instructions](#setup-instructions)

\- \[Usage](#usage)

\- \[Base Response Format](#base-response-format)

\- \[License](#license)



---



\## Technologies



\- \*\*Backend:\*\* .NET 8, C#

\- \*\*Database:\*\* PostgreSQL

\- \*\*ORM \& Micro-ORM:\*\* EF Core + Dapper

\- \*\*API Gateway:\*\* Ocelot

\- \*\*Authentication:\*\* JWT (Access + Refresh Tokens)

\- \*\*Logging:\*\* Serilog (structured logs with correlation IDs)

\- \*\*Caching \& Rate Limiting:\*\* In-memory + gateway-level policies

\- \*\*Documentation:\*\* Swagger + Postman



---



\## Project Overview



\- \*\*Goal:\*\* Build a microservices system with secure, scalable, and maintainable architecture.

\- \*\*Services:\*\*  

&nbsp; 1. \*\*OrderService:\*\* Manages customer orders.  

&nbsp; 2. \*\*InventoryService:\*\* Manages stock and items.

\- \*\*Gateway:\*\* Centralized API gateway using Ocelot to route requests, forward JWT, and enforce rate limits and caching.

\- \*\*Clean Architecture:\*\* Separate concerns into `API`, `Business`, `Persistence`, and `Domain` layers.

\- \*\*Transactions:\*\* All CRUD operations across multiple tables are transactional to maintain database consistency.

\- \*\*Database Normalization:\*\* All schemas are normalized to avoid redundancy and enforce integrity.



---



\## Project Structure





&nbsp;Business # Business logic, services, use cases

&nbsp;Persistence # EF Core DbContext, Dapper Repositories, Unit of Work

&nbsp;Domain # Entities, Enums, DTO

&nbsp;ApiGateway # Ocelot gateway configuration, JWT forwarding, rate limiting, caching

&nbsp;Item.api # Controllers for item service

&nbsp;Order.api # Controllers for order service that contains order items operations

&nbsp;Login.api # Controllers for login service with JWT authentication 

&nbsp;Stock.api # Controllers for stock service



\## Authentication \& Authorization



\- \*\*JWT Authentication:\*\* Access + Refresh tokens

\- \*\*Claims-based Authorization:\*\* Define roles and permissions

\- \*\*Login Table:\*\* Stores user credentials and roles

\- \*\*Security Features:\*\*

&nbsp; - Refresh token workflow

&nbsp; - Token expiration handling

&nbsp; - Role and claims validation on all endpoints



\## Ocelot Gateway Features:

&nbsp; - Routes requests to OrderService and InventoryService

&nbsp; - Forwards JWT tokens

&nbsp; - Rate limiting per route

&nbsp; - Caching for repeated queries

&nbsp; - Optional: blocks direct API access for added security



\## Logging:

  - Serilog structured logging

&nbsp;  Logs include:

&nbsp;   -Correlation IDs for request tracing

&nbsp;   -Request and response bodies

&nbsp;   -Errors with stack traces

&nbsp;   -Output to console + files

