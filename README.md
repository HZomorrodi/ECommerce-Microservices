# 🛒 E-Commerce Microservices

```{=html}
<p align="center">
```
![.NET](https://img.shields.io/badge/.NET-8-512BD4?style=for-the-badge&logo=dotnet)
![ASP.NET
Core](https://img.shields.io/badge/ASP.NET_Core-Microservices-5C2D91?style=for-the-badge)
![Docker](https://img.shields.io/badge/Docker-2496ED?style=for-the-badge&logo=docker)
![RabbitMQ](https://img.shields.io/badge/RabbitMQ-FF6600?style=for-the-badge&logo=rabbitmq)
![Redis](https://img.shields.io/badge/Redis-DC382D?style=for-the-badge&logo=redis)
![Ocelot](https://img.shields.io/badge/Ocelot-API_Gateway-4CAF50?style=for-the-badge)

```{=html}
</p>
```
A production-style **ASP.NET Core Microservices** solution demonstrating
distributed system design, asynchronous messaging, API Gateway,
distributed caching, resiliency patterns, Docker containerization, and
polyglot persistence.

------------------------------------------------------------------------

# ✨ Highlights

-   ✅ ASP.NET Core 8
-   ✅ Microservices Architecture
-   ✅ Ocelot API Gateway
-   ✅ RabbitMQ Event-Driven Communication
-   ✅ Redis Distributed Cache
-   ✅ JWT Authentication
-   ✅ Docker & Docker Compose
-   ✅ SQL Server, MySQL & MongoDB
-   ✅ Polly Retry & Circuit Breaker
-   ✅ Dapper & Entity Framework Core
-   ✅ FluentValidation & AutoMapper

------------------------------------------------------------------------

# 🏛 Architecture

``` text
                        Client
                           │
                           ▼
                 Ocelot API Gateway
          ┌────────────┼────────────┐
          ▼            ▼            ▼
   Users Service  Products Service  Orders Service
          │            │            │
          └────────────┼────────────┘
                       ▼
                   RabbitMQ
             Product Created/Updated/Deleted
                       │
                       ▼
                  Orders Sync

Redis → Distributed Cache

Databases
Users    → SQL Server
Products → MySQL
Orders   → MongoDB
```

# 📁 Repository Structure

``` text
ECommerce-Microservices
│
├── users-service
├── products-service
├── orders-service
├── api-gateway
├── docker
└── docker-compose.yaml
```

# 🚀 Services

## 👤 Users Service

-   JWT Authentication
-   User Registration & Login
-   Dapper
-   SQL Server
-   FluentValidation
-   Clean Architecture

## 📦 Products Service

-   Product CRUD
-   Minimal API
-   EF Core + MySQL
-   RabbitMQ Publisher
-   AutoMapper
-   Repository Pattern

## 🛍 Orders Service

-   Order Processing
-   MongoDB
-   RabbitMQ Consumer
-   HttpClient
-   Polly Retry
-   Circuit Breaker

## 🌐 API Gateway

-   Ocelot Routing
-   Reverse Proxy
-   Single Entry Point

# 📨 Event-Driven Communication

RabbitMQ events:

-   Product Created
-   Product Updated
-   Product Deleted

Orders Service subscribes to product events to keep its local product
data synchronized.

# 🐳 Containerized Environment

Run everything with:

``` bash
docker compose up --build
```

Containers include:

-   API Gateway
-   Users Service
-   Products Service
-   Orders Service
-   RabbitMQ
-   Redis
-   SQL Server
-   MySQL
-   MongoDB

# 💻 Technologies

  Category     Stack
  ------------ ----------------------------
  Backend      ASP.NET Core 8, C#
  APIs         REST, Minimal API
  ORM          EF Core, Dapper
  Messaging    RabbitMQ
  Cache        Redis
  Gateway      Ocelot
  Databases    SQL Server, MySQL, MongoDB
  Resiliency   Polly
  Validation   FluentValidation
  Mapping      AutoMapper
  Containers   Docker, Docker Compose

# 📈 Planned Improvements

-   Azure DevOps Pipelines
-   AKS Deployment
-   Health Checks
-   Centralized Logging
-   Distributed Tracing
-   API Versioning

# 👨‍💻 Author

**Hossein Zomorrodi**

GitHub: https://github.com/HZomorrodi
