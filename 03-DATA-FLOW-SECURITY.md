# Data Flow & Security Sequence Diagrams

> **Part 3 of 4** — split from [`ARCHITECTURE.md`](ARCHITECTURE.md).
> **Status:** AS-IS flows (verified against the code); TO-BE additions are marked as such.
> **Scope:** login and token issuance, authorization checks, password reset, token rotation, registration/verification, and cross-service messaging.

This document illustrates the operational and security flows from request to response, including JWT handling, RBAC authorization, and background email delivery.

**Related:** [Database Model](01-DATABASE-MODEL.md) · [Component Architecture](02-COMPONENT-ARCHITECTURE.md) · [Infrastructure & Deployment](04-INFRASTRUCTURE-DEPLOYMENT.md)

---

## 3.1 Login + token issuance (AS-IS logic, TO-BE adds the `permissions` claim)

```mermaid
sequenceDiagram
  autonumber
  participant C as Client
  participant AC as AuthController
  participant LH as LoginCommandHandler
  participant R as AuthRepositories
  participant U as User (Domain)
  participant H as AppHasher
  participant T as TokenGenerator
  participant DB as PostgreSQL

  C->>AC: POST /api/a/Auth/login {email, password}
  AC->>AC: ModelState validation
  AC->>LH: HandleAsync(LoginCommand)
  LH->>R: GetUserByEmailAsync(email)
  R->>DB: SELECT user + roles
  DB-->>R: user or null
  R-->>LH: user
  LH->>U: ValidateLogin() (active? locked out?)
  alt invalid / inactive / locked
    U-->>LH: throw BusinessValidationException
    LH-->>AC: exception
  else credentials
    LH->>H: Verify(password, PasswordHash)
    alt wrong password
      LH->>U: IncrementAccessFailed()
      LH->>R: SaveChangesAsync()
      LH-->>AC: throw UnauthorizedException
    else correct
      LH->>U: RecordSuccessfulLogin()
      LH->>T: GenerateAccessToken(user)
      T-->>LH: JWT (sub, email, role, [permissions])
      LH->>T: GeneratorRefreshToken(ip)
      LH->>U: AddRefreshToken(refreshToken)
      LH->>R: SaveChangesAsync()
      LH-->>AC: AuthResponse(accessToken, refreshToken)
    end
  end
  AC-->>C: 200 OK / 400 / 401 (via ExceptionMiddleware)
```

## 3.2 Authorization check on a protected endpoint (RBAC + permission)

```mermaid
sequenceDiagram
  autonumber
  participant C as Client
  participant MW as ASP.NET Core Pipeline
  participant JWT as JwtBearer Authentication
  participant AUTHZ as Authorization + PermissionHandler
  participant UC as UserController
  participant H as Handler

  C->>MW: GET /api/a/User (Authorization: Bearer <JWT>)
  MW->>JWT: Validate signature, issuer, audience, lifetime (ClockSkew=0)
  alt invalid/expired
    JWT-->>C: 401 Unauthorized
  else valid
    JWT->>JWT: Build ClaimsPrincipal (sub, email, role, permissions)
    JWT->>AUTHZ: Evaluate [Authorize(Policy = "Admin")] / [HasPermission("identity.user.read")]
    alt policy/permission not satisfied
      AUTHZ-->>C: 403 Forbidden
    else satisfied
      AUTHZ->>UC: proceed
      UC->>H: HandleAsync(query, CancellationToken)
      H-->>UC: result
      UC-->>C: 200 OK
    end
  end
```

## 3.3 Forgot password (transactional side-effect via Hangfire)

```mermaid
sequenceDiagram
  autonumber
  participant C as Client
  participant AC as AuthController
  participant FH as ForgotPasswordCommandHandler
  participant R as AuthRepositories
  participant DB as PostgreSQL
  participant EQ as IEmailQueueServiceClient
  participant HF as Hangfire Server
  participant EBS as EmailBackgroundServices
  participant NC as INotificationClient
  participant SMTP as SMTP Provider

  C->>AC: POST /api/a/Auth/change-password/forgot-password {email}
  AC->>FH: HandleAsync(ForgotPasswordCommand)
  FH->>R: GetUserByEmailAsync(email)
  R-->>FH: user or null
  alt user null / inactive
    FH-->>AC: return silently (do not reveal existence)
  else user found
    FH->>R: ExecuteInTransactionAsync(work)
    R->>DB: BEGIN
    R->>R: generate token, hash, user.SetPasswordResetToken(...)
    R->>R: render email template (missing template throws -> rollback)
    R->>DB: UPDATE users + COMMIT
    FH->>EQ: Enqueue email (after commit)
    EQ->>HF: BackgroundJob.Enqueue [queue: email]
    HF->>EBS: SendBackgroundEmailAsync(htmlBody)
    EBS->>NC: SendEmailAsync(request)
    NC->>SMTP: MailKit send
  end
  AC-->>C: 200 OK ("email sent if account exists")
```

> Reliability note: the enqueue happens **after** commit. TO-BE strengthens this with the **transactional outbox** (write `OutboxMessage` in the same transaction; a worker publishes to Hangfire), eliminating the dual-write window.

## 3.4 Refresh token rotation

```mermaid
sequenceDiagram
  autonumber
  participant C as Client
  participant AC as AuthController
  participant RH as RefreshTokenCommandHandler
  participant R as AuthRepositories
  participant U as User (Domain)
  participant DB as PostgreSQL

  C->>AC: POST /api/a/Auth/refresh-token {refreshToken}
  AC->>RH: HandleAsync(RefreshTokenCommand, ip)
  RH->>R: GetRefreshTokenAsync(hash(token))
  R->>DB: SELECT refresh token + user
  DB-->>R: token
  alt not found / revoked / expired
    RH-->>AC: throw UnauthorizedException -> 401
  else valid
    RH->>U: GetActiveRefreshToken + Revoke(ip, replacedBy)
    RH->>R: Generate new access + refresh token
    RH->>R: SaveChangesAsync()
    RH-->>AC: AuthResponse(new pair)
  end
  AC-->>C: 200 OK
```

## 3.5 Registration + email verification

```mermaid
sequenceDiagram
  autonumber
  participant C as Client
  participant AC as AuthController
  participant RH as RegisterCommandHandler
  participant R as AuthRepositories
  participant DB as PostgreSQL
  participant HF as Hangfire (email queue)
  participant SMTP as SMTP

  C->>AC: POST /api/a/Auth/register
  AC->>RH: HandleAsync(RegisterCommand)
  RH->>R: IsUserEmailExists / IsUsernameExists
  RH->>R: ExecuteInTransactionAsync
  R->>DB: BEGIN; INSERT user + role; set verification token; COMMIT
  RH->>HF: enqueue verification email
  HF->>SMTP: send
  AC-->>C: 200 OK
  C->>AC: POST /api/a/Auth/verify-email {token}
  AC->>R: GetUserByEmailVerificationTokenAsync(token)
  AC->>DB: UPDATE email verified
  AC-->>C: 200 OK
```

## 3.6 Cross-service flow (Platform → RabbitMQ → Command, and gRPC sync)

```mermaid
sequenceDiagram
  autonumber
  participant U as Client
  participant P as Support.Platform
  participant MQ as RabbitMQ (fanout 'trigger')
  participant Cmd as Command
  participant DB as Command DB

  U->>P: POST /api/Platforms
  P->>P: persist Platform (SQL Server)
  P->>MQ: publish PlatformPublishedDto
  MQ->>Cmd: deliver (MessageBusSubscriber)
  Cmd->>Cmd: EventProcessor switch PlatformPublished
  Cmd->>DB: insert Platform

  Cmd->>P: gRPC GetAllPlatforms (sync)
  P-->>Cmd: platforms
```

