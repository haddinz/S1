# Component Architecture (C4 Level 3)

> **Part 2 of 4** — split from [`ARCHITECTURE.md`](ARCHITECTURE.md).
> **Status:** AS-IS component structure (verified against the code).
> **Scope:** `Support.Auth.Id` + hosted Notification module, and the legacy `Support.Platform` / `Command` subsystem.

This document shows how the layers and modules of the .NET application and its internal services are structured, including presentation, application/CQRS, domain, infrastructure, cross-cutting services, and background processing.

**Related:** [Database Model](01-DATABASE-MODEL.md) · [Data Flow & Security](03-DATA-FLOW-SECURITY.md) · [Infrastructure & Deployment](04-INFRASTRUCTURE-DEPLOYMENT.md)

---

## 2.1 C4 Level 1 — System Context

```mermaid
flowchart TB
  USER["End User / Frontend (BaseUrl)"]
  ADMIN["Platform Admin"]
  AUTH["Support.Auth.Id<br/>(Identity & Access + Notification host)"]
  CMD["Command API"]
  PLAT["Support.Platform API"]
  SMTP["SMTP Provider"]
  DB[("PostgreSQL")]
  DB2[("SQL Server (legacy)")]
  MQ["RabbitMQ"]
  USER --> AUTH
  ADMIN --> AUTH
  ADMIN --> CMD
  ADMIN --> PLAT
  AUTH --> SMTP
  AUTH --> DB
  PLAT --> MQ
  MQ --> CMD
  PLAT --> DB2
  CMD --> PLAT
```

## 2.2 C4 Level 3 — `Support.Auth.Id` (+ hosted Notification module)

```mermaid
flowchart TB
  subgraph EXT["External Actors"]
    CLIENT["Web / Mobile Client"]
    DASH["Operator (Hangfire Dashboard)"]
  end

  subgraph AUTH["Support.Auth.Id (net10.0, ASP.NET Core)"]
    direction TB
    subgraph PRES["Presentation Layer"]
      EM["ExceptionMiddleware<br/>maps domain exceptions -> HTTP"]
      AC["AuthController<br/>/api/a/Auth"]
      UC["UserController<br/>/api/a/User"]
      RC["RoleController<br/>/api/a/Role"]
    end

    subgraph APP["Application Layer (CQRS)"]
      HA["Auth Handlers<br/>Login, Logout, Register, RefreshToken,<br/>ChangePassword, VerifyEmail, ResendVerifyEmail,<br/>ForgotPassword, ResetPassword"]
      HU["User Handlers<br/>GetActiveUser, GetUsers, UpdateUser"]
      HR["Role Handlers"]
      IF["ICommandHandler&lt;T&gt; / ICommandQueryHandler&lt;T,R&gt;"]
    end

    subgraph DOM["Domain Layer"]
      ENT["Entities: User, Role, RefreshToken"]
      VO["Value Objects: EmailVerification,<br/>PasswordReset, JwtSettings"]
      EXC["Domain Exceptions"]
    end

    subgraph INFRA["Infrastructure Layer"]
      REPO["AuthRepositories : IAuthRepositories<br/>+ ExecuteInTransactionAsync"]
      CTX["AppDbContext (EF Core / Npgsql)"]
      CFG["Entity Configurations"]
      SEED["SeedData"]
    end

    subgraph SEC["Cross-cutting Services"]
      HASH["IAppHasher (BCrypt + SHA-256)"]
      TOK["ITokenGenerator (JWT)"]
      STOK["ISecureTokenGenerator&lt;T&gt;"]
      PPV["IPasswordPolicyValidator"]
    end

    subgraph NOTIF["Notification Module (Support.Notification.Id)"]
      EQ["IEmailQueueServiceClient<br/>EmailQueueService"]
      EBS["IEmailBackgroundServices<br/>EmailBackgroundServices [Queue: email]"]
      NC["INotificationClient<br/>EmailSenderServices (MailKit)"]
      SMTPOPT["SmtpOptions (validated)"]
    end

    subgraph JOBS["Background Processing"]
      HFS["Hangfire Server<br/>queues: email, cleanup, default"]
      OBW["OutboxWorkerService<br/>(AS-IS: disabled)"]
    end
  end

  subgraph DATA["Data Stores"]
    PG[("PostgreSQL supportdb<br/>identity + hangfire")]
  end

  SMTP["SMTP Provider"]
  OUTBOX[("OutboxMessages")]

  CLIENT -->|HTTPS/JSON| EM
  EM --> AC & UC & RC
  AC --> HA
  UC --> HU
  RC --> HR
  HA & HU & HR --> IF
  HA & HU & HR --> REPO
  HA --> TOK & HASH & STOK & PPV
  HA --> EQ
  HA & HU & HR --> ENT
  ENT --> VO & EXC
  REPO --> CTX
  CTX --> CFG
  CTX --> PG
  CTX --> OUTBOX
  EQ -->|BackgroundJob.Enqueue| HFS
  HFS --> EBS --> NC --> SMTP
  EBS --> SMTPOPT
  OBW --> OUTBOX
  OBW --> NC
  DASH -->|/auth/hangfire BasicAuth| HFS
  EXC -->|ErrorResponse JSON| CLIENT
```

**Component responsibilities**

| Component | Responsibility | Key files |
|---|---|---|
| `ExceptionMiddleware` | Global error handling; typed exception → HTTP (404/409/401/400/500) + `ErrorResponse` with TraceId | `src/Middleware/ExceptionMiddleware.cs` |
| Controllers | HTTP boundary, model validation, dispatch to handlers; no business logic | `src/Controller/*.cs` |
| CQRS Handlers | Orchestration, transaction boundary (`ExecuteInTransactionAsync`) | `src/Features/Handler/**` |
| Domain entities | Invariants, factories, state transitions | `src/Domain/Entity/*.cs` |
| Value objects | Immutable sub-values, owned by `User` | `src/Domain/ValueObject/*.cs` |
| `AuthRepositories` | EF Core data access + transaction | `src/Repositories/*` |
| `AppDbContext` | EF model, query filters, configurations | `src/Repositories/Data/*` |
| `TokenGenerator` | JWT access token + refresh token creation | `src/Services/Security/TokenGenerator.cs` |
| `AppHasher` | BCrypt (password) + SHA-256 (token hash) | `src/Services/Security/PasswordHasher.cs` |
| `EmailQueueService` | Enqueue Hangfire email job | `Support.Notification.Id/src/Services/EmailQueueService.cs` |
| `EmailBackgroundServices` | Hangfire job `[Queue("email")]` → MailKit | `.../BackgroundServices/EmailBackgroundServices.cs` |
| `EmailSenderServices` | SMTP send (`INotificationClient`) | `.../Services/EmailSenderServices.cs` |
| `Hangfire Server` | Executes queued jobs; dashboard at `/auth/hangfire` | `Program.cs` |
| `OutboxWorkerService` | Poll outbox, deliver via `INotificationClient` | `src/Services/BackgroundServices/OutboxWorkerService.cs` |

## 2.3 C4 Level 3 — Legacy subsystem (`Support.Platform` + `Command`)

```mermaid
flowchart TB
  subgraph PLAT["Support.Platform (net9.0)"]
    PC["PlatformsController<br/>/api/Platforms"]
    PR["IPlatformRepo / PlatformRepo"]
    PDB[("AppDBContext<br/>SQL Server / InMemory")]
    MB["IMessageBusClient / MessageBusClient"]
    MRI["RabbitMqInitializer (HostedService)"]
    GRPCS["GrpcPlatformService (gRPC server)"]
    HTTPC["ICommandDataCLient / HttpCommandDataClient"]
    AM1["AutoMapper"]
  end
  subgraph CMD["Command (net9.0)"]
    CC["PlatformController, CommandsController<br/>/api/c/..."]
    CR["ICommandsRepo / CommandsRepo"]
    CDB[("AppDbContext<br/>InMemory")]
    MBS["MessageBusSubscriber (HostedService)"]
    EP["IEventProcessor / EventProcessort"]
    GRPCC["PlatformDataClient (gRPC client)"]
    AM2["AutoMapper"]
  end
  MQ["RabbitMQ (fanout exchange 'trigger')"]
  CLIENT["Client"]

  CLIENT --> PC & CC
  PC --> PR --> PDB
  PC --> MB --> MQ
  MRI --> MQ
  GRPCC -->|GetAllPlatforms| GRPCS
  PC -. disabled .-> HTTPC --> CC
  MQ --> MBS --> EP --> CR --> CDB
  CC --> CR
```

