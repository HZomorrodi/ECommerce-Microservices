# 🛒 E-Commerce Microservices

<p align="center">

![.NET](https://img.shields.io/badge/.NET-8-purple)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET_Core-8-blue)
![Microservices](https://img.shields.io/badge/Architecture-Microservices-success)
![Docker](https://img.shields.io/badge/Docker-Containerized-blue)
![RabbitMQ](https://img.shields.io/badge/RabbitMQ-Event_Driven-orange)
![Redis](https://img.shields.io/badge/Redis-Distributed_Cache-red)
![Ocelot](https://img.shields.io/badge/Ocelot-API_Gateway-green)

</p>

A production-style **ASP.NET Core 8 Microservices** solution demonstrating distributed system design using **Ocelot API Gateway**, **RabbitMQ**, **Redis**, **Docker Compose**, and multiple databases.

---

# 🚀 Architecture

<img width="1536" height="1024" alt="ChatGPT Image Jul 30, 2026, 02_45_54 PM" src="https://github.com/user-attachments/assets/145dfeef-04c8-4cc9-9d25-8575324321a1" />

---

# 📦 Services

| Service | Database | Description |
|----------|----------|-------------|
| Orders Service | MongoDB | Order management |
| Products Service | MySQL | Product catalog |
| Users Service | SQL Server | Authentication & User Management |
| API Gateway | - | Ocelot API Gateway |

---

# 🏗 Solution Structure

```text
ECommerce-Microservices
│
├── api-gateway
├── orders-service
├── products-service
├── users-service
├── docker
│   ├── MySQL
│   └── PostgreSQL
│
├── docker-compose.yml
└── README.md
```

---

# ✨ Features

- ASP.NET Core 8
- Microservices Architecture
- Ocelot API Gateway
- RabbitMQ Event-Driven Communication
- Redis Distributed Cache
- Docker Compose
- MongoDB
- MySQL
- SQL Server
- Repository Pattern
- Dependency Injection
- FluentValidation
- AutoMapper
- Dapper
- RESTful APIs

---

# 🖥 Screenshots


## Products Service

<img width="1901" height="796" alt="Screenshot 2026-07-30 150512" src="https://github.com/user-attachments/assets/47d2d28f-181b-429d-b9b9-57e38f472cb2" />

## Users Service

<img width="1900" height="648" alt="Screenshot 2026-07-30 150214" src="https://github.com/user-attachments/assets/5ee51542-9f05-4c19-a803-28128091ccf7" />


---

# ⚙️ Getting Started

```bash
git clone https://github.com/HZomorrodi/ECommerce-Microservices.git

cd ECommerce-Microservices

docker compose up --build
```

---

# 🛠 Tech Stack

- ASP.NET Core 8
- Ocelot
- RabbitMQ
- Redis
- Docker
- MongoDB
- MySQL
- SQL Server
- Dapper
- FluentValidation
- AutoMapper

---

# 👨‍💻 Author

**Hossein Zomorrodi**

GitHub:
https://github.com/HZomorrodi
