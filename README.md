
# S1 — .NET Microservices & Distributed Systems

Support Pro-Document is a microservices-driven platform designed to streamline secure academic grading and professional documentation management. While the final business goal is to enable educators and administrators to seamlessly input, track, and process grade documents, current development focuses on the core infrastructure. The repository is presently establishing the foundational Authentication and Notification subsystems using an event-driven .NET architecture. Once this secure distributed communication fabric is complete, the core Support Pro-Document business features will be plugged directly into the system.

## 🎯 Purpose

This project is used to learn and experiment with:

-   Building REST APIs with ASP.NET Core
    
-   Database access using Entity Framework Core
    
-   Working with MSSQL and PostgreSQL
    
-   Object mapping between DTOs and Entities
    
-   Communication between services using gRPC
    
-   Asynchronous communication using RabbitMQ
    
-   Background message consumers
    
-   Containerizing services with Docker
    
-   Deploying and communicating between services using Kubernetes
    
-   Managing application configuration
    
-   Understanding dependency injection and service lifetimes
    
-   Applying backend coding and architecture best practices
    

----------

# 🛠️ Technology Stack

Technology

Purpose

Link

**C#**

Main programming language

[C#](https://learn.microsoft.com/en-us/dotnet/csharp/)

**.NET**

Runtime and development platform

[.NET](https://dotnet.microsoft.com/)

**ASP.NET Core**

Building REST APIs and backend services

[ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/)

**Entity Framework Core**

ORM for database access and migrations

[EF Core](https://learn.microsoft.com/en-us/ef/core/)

**Microsoft SQL Server**

Relational database

[SQL Server](https://www.microsoft.com/en-us/sql-server)

**PostgreSQL**

Alternative relational database

[PostgreSQL](https://www.postgresql.org/)

**AutoMapper**

Mapping DTOs ↔ Entities

[AutoMapper](https://automapper.org/)

**RabbitMQ**

Message broker for asynchronous communication

[RabbitMQ](https://www.rabbitmq.com/)

**AMQP**

Messaging protocol used by RabbitMQ

[AMQP](https://www.rabbitmq.com/tutorials/amqp-concepts)

**gRPC**

Synchronous RPC communication between services

[gRPC](https://grpc.io/)

**Protocol Buffers**

Contract/schema used by gRPC

[Protobuf](https://protobuf.dev/)

**Docker**

Containerizing applications and dependencies

[Docker](https://www.docker.com/)

**Kubernetes**

Container orchestration and service management

[Kubernetes](https://kubernetes.io/)

**Swagger / OpenAPI**

API documentation and testing

[Swagger](https://swagger.io/)

**.NET User Secrets**

Local development secret management

[.NET User Secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets)

----------

# 🧩 Tools & Their Purposes

## 1. ASP.NET Core

ASP.NET Core is the main framework used to build the backend services.

It is responsible for:

-   HTTP APIs
    
-   Controllers
    
-   Dependency Injection
    
-   Middleware
    
-   Configuration
    
-   Authentication/authorization
    
-   Service hosting
    

Example:

```text
Client
   │
   ▼
ASP.NET Core API
   │
   ├── Application Logic
   ├── Repository
   └── Database

```

Official documentation:

[https://learn.microsoft.com/en-us/aspnet/core/](https://learn.microsoft.com/en-us/aspnet/core/)

----------

## 2. Entity Framework Core

Entity Framework Core is used as the ORM between the application and relational databases.

Instead of manually writing SQL for every operation:

```text
C# Entity
    ↓
EF Core
    ↓
SQL
    ↓
Database

```

It is also used for:

-   Database migrations
    
-   Relationships
    
-   LINQ queries
    
-   Change tracking
    
-   Database configuration
    

Official documentation:

[https://learn.microsoft.com/en-us/ef/core/](https://learn.microsoft.com/en-us/ef/core/)

----------

## 3. Microsoft SQL Server

SQL Server is used as one of the relational database options.

It is useful for learning:

-   Relational data
    
-   Foreign keys
    
-   Transactions
    
-   Indexes
    
-   SQL queries
    
-   EF Core integration
    

Official website:

[https://www.microsoft.com/en-us/sql-server](https://www.microsoft.com/en-us/sql-server)

----------

## 4. PostgreSQL

PostgreSQL provides another relational database option.

The project can use the PostgreSQL EF Core provider:

```text
Application
     │
     ▼
Entity Framework Core
     │
     ▼
Npgsql
     │
     ▼
PostgreSQL

```

Official website:

[https://www.postgresql.org/](https://www.postgresql.org/)

Npgsql:

[https://www.npgsql.org/](https://www.npgsql.org/)

----------

# 🔄 5. AutoMapper

AutoMapper is used to convert objects between different application models.

A common flow is:

```text
Request DTO
     ↓
Entity
     ↓
Database
     ↓
Entity
     ↓
Response DTO

```

For example:

```text
CreatePlatformDto
        ↓
     Platform
        ↓
PlatformReadDto

```

The important concept is that mapping normally follows:

```text
Request / Create / Published
              ↓
            Entity
              ↓
       Read / Response

```

AutoMapper is particularly useful when DTOs and database entities have different structures.

Official website:

[https://automapper.org/](https://automapper.org/)

----------

# 📨 6. RabbitMQ

RabbitMQ is the **message broker** used for asynchronous communication.

Instead of one service directly calling another service:

```text
Service A
   │
   │ HTTP / gRPC
   ▼
Service B

```

RabbitMQ allows:

```text
Service A
   │
   ▼
RabbitMQ
   │
   ▼
Service B

```

This allows services to communicate asynchronously.

Typical use cases:

-   Publishing events
    
-   Background processing
    
-   Decoupling services
    
-   Notifications
    
-   Event-driven communication
    

Example:

```text
Command Service
      │
      │ Publish Event
      ▼
   RabbitMQ
      │
      │ Consume
      ▼
Notification Service

```

Official website:

[https://www.rabbitmq.com/](https://www.rabbitmq.com/)

----------

# 📡 7. AMQP

AMQP stands for:

> Advanced Message Queuing Protocol

RabbitMQ supports AMQP.

The advantage of using a standard messaging protocol is that the application is not necessarily tied to one particular implementation.

Conceptually:

```text
Application
     ↓
   AMQP
     ↓
 RabbitMQ

```

If another message broker supports the same protocol and the application is designed properly, migration can become easier.

Official documentation:

[https://www.rabbitmq.com/tutorials/amqp-concepts](https://www.rabbitmq.com/tutorials/amqp-concepts)

----------

# ⚡ 8. gRPC

gRPC is used for **synchronous service-to-service communication**.

Instead of:

```text
Service A
   ↓
HTTP REST
   ↓
Service B

```

we can have:

```text
Service A
   ↓
 gRPC
   ↓
Service B

```

gRPC is especially useful when one service needs an immediate response from another service.

Example:

```text
Command Service
      │
      │ GetPlatform()
      ▼
Platform Service
      │
      ▼
Platform Data

```

The communication contract is defined using Protocol Buffers (`.proto`).

Official website:

[https://grpc.io/](https://grpc.io/)

----------

# 📜 9. Protocol Buffers

Protocol Buffers, or Protobuf, define the contract used by gRPC.

Example:

```proto
service Platform {
    rpc GetPlatform(GetPlatformRequest)
        returns (PlatformResponse);
}

```

From this contract, gRPC tooling generates the required client/server classes.

Conceptually:

```text
.proto
  │
  ▼
gRPC Tools
  │
  ├── Server Base
  ├── Client
  └── Message Classes

```

Official website:

[https://protobuf.dev/](https://protobuf.dev/)

----------

# 🐳 10. Docker

Docker is used to package applications and their dependencies into containers.

Instead of requiring every developer to install everything locally:

```text
Developer Machine
 ├── .NET
 ├── SQL Server
 ├── RabbitMQ
 └── Other dependencies

```

containers can provide isolated environments:

```text
Docker
 ├── API Container
 ├── Database Container
 └── RabbitMQ Container

```

This makes development and deployment environments more consistent.

Official website:

[https://www.docker.com/](https://www.docker.com/)

----------

# ☸️ 11. Kubernetes

Kubernetes is used to orchestrate containers.

Docker answers:

> "How do I package and run this application?"

Kubernetes answers:

> "How do I manage many containers/services?"

Example:

```text
                    Kubernetes
                        │
        ┌───────────────┼───────────────┐
        ▼               ▼               ▼
 Command Service   Platform Service   RabbitMQ
        │               │
        ▼               ▼
       Pod             Pod

```

Kubernetes can manage:

-   Pods
    
-   Deployments
    
-   Services
    
-   Networking
    
-   Replicas
    
-   Configuration
    
-   Service discovery
    
-   Rolling updates
    

Official website:

[https://kubernetes.io/](https://kubernetes.io/)

----------

# 🌐 Kubernetes Service Discovery

When services run inside Kubernetes, they should generally not communicate using:

```text
http://localhost:5277

```

because `localhost` refers to the current container/pod.

Instead, Kubernetes Services provide internal DNS names.

Example:

```text
http://command-clusterip-srv:8080

```

Conceptually:

```text
Platform Service
       │
       │ HTTP/gRPC
       ▼
command-clusterip-srv
       │
       ▼
Command Pod

```

This allows Kubernetes to handle service discovery.

----------

# ⚙️ 12. IConfiguration

`.NET IConfiguration` provides access to application configuration.

For example:

```json
{
  "CommandService": "http://localhost:6000"
}

```

The application can retrieve it with:

```csharp
_configuration["CommandService"];

```

This allows configuration to be changed without hard-coding values into the application.

Typical configuration sources include:

-   `appsettings.json`
    
-   `appsettings.Development.json`
    
-   Environment variables
    
-   User Secrets
    
-   Kubernetes configuration
    

Official documentation:

[https://learn.microsoft.com/en-us/dotnet/core/extensions/configuration](https://learn.microsoft.com/en-us/dotnet/core/extensions/configuration)

----------

# 🔐 13. .NET User Secrets

User Secrets are used during local development to keep sensitive configuration outside the source code.

For example:

```text
SMTP username
SMTP password
Database credentials
API keys

```

Instead of putting credentials inside:

```text
appsettings.json

```

use:

```bash
dotnet user-secrets set "Smtp:Password" "your-password"

```

This repository uses User Secrets for SMTP configuration during local development.

Official documentation:

[https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets)

----------

# 📬 14. BackgroundService

`BackgroundService` is useful for running long-running background processes inside a .NET application.

One example is consuming messages from RabbitMQ:

```text
Application starts
      │
      ▼
BackgroundService
      │
      ▼
RabbitMQ Consumer
      │
      ▼
Process Message

```

This is useful for:

-   Message consumers
    
-   Background jobs
    
-   Event processing
    
-   Periodic tasks
    

Official documentation:

[https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.hosting.backgroundservice](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.hosting.backgroundservice)

----------

# 💉 15. Dependency Injection

.NET has built-in Dependency Injection.

Common service lifetimes:

### Scoped

```csharp
AddScoped<T>()

```

A new instance is created for each scope, normally each HTTP request.

Useful for:

-   Repositories
    
-   Request-level services
    
-   EF Core DbContext
    

### Singleton

```csharp
AddSingleton<T>()

```

One instance is shared throughout the application's lifetime.

Useful for resources such as:

-   Long-lived connections
    
-   Message broker connections
    
-   Expensive shared objects
    

### Transient

```csharp
AddTransient<T>()

```

A new instance is created every time it is requested.

Official documentation:

[https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection](https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection)

----------

# 🗄️ Database Relationships

The project also demonstrates relationships between entities using Entity Framework Core.

For example:

```text
Platform
   │
   │ 1
   │
   └───────────*
              Command

```

One `Platform` can have many `Command` records.

Conceptually:

```csharp
Platform
    └── ICollection<Command>

Command
    ├── PlatformId
    └── Platform

```

EF Core can configure this relationship using `HasMany`, `WithOne`, and `HasForeignKey`.

----------

# 📦 DTO Pattern

DTOs are used to control the data entering and leaving the API.

Typical structure:

```text
HTTP Request
     │
     ▼
Request DTO
     │
     ▼
Entity
     │
     ▼
Database
     │
     ▼
Entity
     │
     ▼
Response DTO
     │
     ▼
HTTP Response

```

This prevents database entities from becoming the direct contract of the API.

For a parent-child resource, for example:

```http
POST /api/platform/{platformId}/commands

```

the parent ID can be represented in the URL while the request body contains the data required to create the child resource.

----------

# 🏗️ Project Structure

The repository contains several areas related to the distributed application:

```text
S1/
├── Command/
│   └── Command Service
│
├── Support/
│   └── Support / authentication-related services
│
├── K8S/
│   └── Kubernetes manifests
│
├── README.mdx
└── .gitignore

```

The exact implementation can evolve as the project develops.

----------

# 🔗 Communication Strategy

The project demonstrates two different communication models.

## Synchronous — gRPC

Use when a service needs an immediate response.

```text
Service A
   │
   │ gRPC Request
   ▼
Service B
   │
   │ Response
   ▼
Service A

```

Good for:

-   Querying another service
    
-   Immediate validation
    
-   Request/response operations
    

----------

## Asynchronous — RabbitMQ

Use when the sender does not need an immediate response.

```text
Service A
   │
   │ Publish Event
   ▼
RabbitMQ
   │
   │ Consume
   ▼
Service B

```

Good for:

-   Events
    
-   Notifications
    
-   Background processing
    
-   Decoupling services
    

----------

# 🚀 Development Flow

The intended development/deployment flow is approximately:

```text
Write Code
    │
    ▼
dotnet run
    │
    ▼
Test API
    │
    ▼
Docker Build
    │
    ▼
Docker Push
    │
    ▼
Kubernetes Deployment
    │
    ▼
Kubernetes Rollout
    │
    ▼
Check Pods

```

For configuration changes:

```bash
kubectl apply -f <deployment>.yaml

```

For application/code changes:

```bash
kubectl rollout restart deployment <deployment-name>

```

Check running workloads:

```bash
kubectl get pods

```

----------

# 🧠 Main Concepts Learned

This repository is primarily a learning environment for understanding:

```text
                ┌──────────────┐
                │   REST API   │
                └──────┬───────┘
                       │
                 ┌─────▼─────┐
                 │  .NET API │
                 └─────┬─────┘
                       │
             ┌─────────┴─────────┐
             │                   │
             ▼                   ▼
        Entity Framework       gRPC
             │                   │
             ▼                   ▼
          Database          Other Service
                                  
             │
             ▼
          RabbitMQ
             │
             ▼
      Background Service
             
             │
             ▼
           Docker
             │
             ▼
        Kubernetes

```

The main goal is to understand **how individual backend technologies work together to form a distributed system**.

----------

# 📚 Official Documentation

-   [.NET](https://dotnet.microsoft.com/)
    
-   [ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/)
    
-   [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/)
    
-   [C#](https://learn.microsoft.com/en-us/dotnet/csharp/)
    
-   [SQL Server](https://www.microsoft.com/en-us/sql-server)
    
-   [PostgreSQL](https://www.postgresql.org/)
    
-   [Npgsql](https://www.npgsql.org/)
    
-   [AutoMapper](https://automapper.org/)
    
-   [RabbitMQ](https://www.rabbitmq.com/)
    
-   [AMQP Concepts](https://www.rabbitmq.com/tutorials/amqp-concepts)
    
-   [gRPC](https://grpc.io/)
    
-   [Protocol Buffers](https://protobuf.dev/)
    
-   [Docker](https://www.docker.com/)
    
-   [Kubernetes](https://kubernetes.io/)
    
-   [Swagger / OpenAPI](https://swagger.io/)
    
-   [.NET User Secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets)
    
-   [.NET Dependency Injection](https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection)
    

----------

# 📌 Repository

Original repository:

[https://github.com/haddinz/S1/tree/development](https://github.com/haddinz/S1/tree/development)


# Configuration & Setup Guide

Dokumentasi ini menjelaskan konfigurasi yang dibutuhkan oleh modul **Notification** (SMTP) untuk keperluan pengiriman email.

---

## 1. Environment Variables / AppSettings Definition

Berikut adalah daftar variabel konfigurasi SMTP yang dibaca oleh aplikasi:

| Key | Tipe Data | Wajib | Deskripsi | Contoh Nilai |
| :--- | :--- | :--- | :--- | :--- |
| `Smtp:Host` | `string` | Ya | Alamat server SMTP | `smtp.gmail.com` |
| `Smtp:Port` | `int` | Ya | Port server SMTP | `587` (TLS) / `465` (SSL) |
| `Smtp:Username` | `string` | Ya | Username / Email autentikasi | `support@example.com` |
| `Smtp:Password` | `string` | Ya | Password / App Password SMTP | `your-app-password` |
| `Smtp:SenderName` | `string` | Ya | Nama pengirim yang muncul di email | `Support` |
| `Smtp:SenderEmail` | `string` | Ya | Email pengirim | `no-replaysupport@example.com` |
| `Smtp:EnableSsl` | `boolean` | Tidak | Mengaktifkan enkripsi SSL/TLS | `true` |

---

## 2. Local Development Setup (.NET User Secrets)

Untuk pengembangan lokal, **dilarang menyimpan kredensial asli di dalam file `appsettings.json`**. Gunakan fitur **.NET User Secrets** pada project entry point (`Support.Auth.Id`).

Jalankan perintah berikut di terminal (di root folder project `Support.Auth.Id`):

```bash
# 1. Inisialisasi User Secrets
dotnet user-secrets init --project Support.Auth.Id

# 2. Set konfigurasi SMTP
dotnet user-secrets set "Smtp:Host" "smtp.gmail.com"
dotnet user-secrets set "Smtp:Port" "587"
dotnet user-secrets set "Smtp:Username" "support@example.com"
dotnet user-secrets set "Smtp:Password" "password"
dotnet user-secrets set "Smtp:SenderName" "Support"
dotnet user-secrets set "Smtp:SenderEmail" "no-replaysupport@example.com"
dotnet user-secrets set "Smtp:EnableSsl" "true"

# 2. Set konfigurasi SMTP for Mailpit testing
dot3et user-secrets list
Smtp:Username = 
Smtp:SenderName = Support
Smtp:SenderEmail = no-reply@support.local
Smtp:Port = 1025
Smtp:Password = 
Smtp:Host = localhost
Smtp:EnableSsl = false
