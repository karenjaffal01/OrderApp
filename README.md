🏗️ Microservices Project with Clean Architecture, JWT, and Ocelot API Gateway

This project demonstrates a full-fledged microservices architecture using .NET 8, PostgreSQL, and Ocelot API Gateway, built with Clean Architecture principles.

It includes multiple services (OrderService, InventoryService, LoginService, StockService) and features JWT authentication with refresh tokens, transactional operations, structured logging, rate limiting, caching, and Dockerized deployments.

The project is designed to be secure, scalable, and maintainable, making it a solid foundation for real-world enterprise systems.

📑 Table of Contents

Technologies

Project Overview

Project Structure

Database Design

Order Schema

Inventory Schema

Authentication & Authorization

Services

OrderService

InventoryService

API Gateway

Logging

Rate Limiting & Caching

Testing & Documentation

Setup Instructions

Usage

Base Response Format

License

⚙️ Technologies

Backend: .NET 8, C#

Database: PostgreSQL

ORM & Micro-ORM: EF Core + Dapper

API Gateway: Ocelot

Authentication: JWT (Access + Refresh Tokens)

Logging: Serilog (with correlation IDs)

Caching & Rate Limiting: In-memory + Ocelot policies

Documentation: Swagger + Postman Collections

Containerization: Docker (services run in isolated containers)

🚀 Project Overview

Goal: Build a microservices-based system with secure APIs, modular services, and centralized gateway management.

Core Services:

OrderService: Handles customer orders and order items.

InventoryService: Manages stock and product items.

LoginService: Provides JWT-based authentication and refresh token handling.

StockService: Maintains stock updates for items.

Gateway: Ocelot API Gateway routes requests, applies policies, and enforces security.

Clean Architecture: Layers are separated into Domain, Application, Infrastructure, and API.

Database: Normalized relational schemas with transactional consistency.

📂 Project Structure
Business/        # Business logic, services, use cases
Persistence/     # EF Core DbContext, Dapper Repositories, Unit of Work
Domain/          # Entities, Enums, DTOs
ApiGateway/      # Ocelot configuration, JWT forwarding, rate limiting
Order.Api/       # Controllers for Order service (orders + order items)
Item.Api/        # Controllers for Item service
Stock.Api/       # Controllers for Stock service
Login.Api/       # Controllers for Login/authentication

🔑 Authentication & Authorization

JWT Authentication: Access + Refresh tokens

Claims-based Authorization: Role & permission validation

Login Table: Securely stores credentials & roles

Security Features:

Refresh token workflow

Expired token handling

Role-based endpoint restrictions

🌐 Ocelot API Gateway Features

Routes requests to microservices (Order, Inventory, Login, Stock)

Forwards JWT tokens for secure communication

Enforces per-route rate limiting

Provides caching for repeated queries

Optionally blocks direct API access, forcing all traffic through the gateway

📊 Logging

Serilog structured logging

Logs include:

Correlation IDs for request tracing

Request & response payloads

Error details with stack traces

Outputs to console & file sinks

⏱️ Rate Limiting & Caching

Rate Limiting: Prevents abuse by limiting requests per user or service

Caching: Improves performance by caching frequently accessed routes

🧪 Testing & Documentation

Unit Tests:

Covers business logic and service behaviors

Ensures API endpoints return expected responses (200, 400, etc.)

Integration Tests (via Postman):

Postman collections provided for testing workflows

Predefined requests for login, orders, inventory, and stock APIs

Swagger UI: Available per service for exploring endpoints

🐳 Dockerized Deployment

Each service is containerized into its own Docker container, making it easy to:

Spin up isolated environments

Scale services independently

Run the whole system using docker-compose

⚡ Setup Instructions

Clone the repository

Configure PostgreSQL database connection strings in appsettings.json

Run migrations for each service

Start services locally or build & run with Docker:

docker-compose up --build


Access APIs through the Ocelot Gateway

▶️ Usage

Authentication:

Login using /login endpoint to obtain JWT tokens

Use the token to authorize requests via Swagger or Postman

Order Operations:

Create, update, delete, and fetch customer orders

Inventory Operations:

Add new items, update stock, and fetch available items

📦 Base Response Format

All APIs return responses in a consistent format:

{
  "success": true,
  "message": "Operation completed successfully",
  "data": { }
}
