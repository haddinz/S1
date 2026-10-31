## Stoppper


# Checklist Implementasi — PBAC & Audit Trail

> Berdasarkan dokumen konsep PBAC + Audit Trail.
> Scope: `Support.Auth.Id`, `Support.Auth.Client`, `Support.UI`, `Support.Audit` (library baru).
> **Command & Support.Platform tidak disentuh.**

---

## FASE 0 — Rapikan Fondasi (1–2 hari)

- [x] Fix double-serialize di `ExceptionMiddleware` — hapus `JsonSerializer.Serialize` sebelum `WriteAsJsonAsync`
- [x] Bonus: tambah `UnauthorizedException` ke daftar `is not` (401 masuk `LogWarning`, bukan `LogError`)
- [ ] Rapikan `RoleController` — ganti `RoleManager<IdentityRole>` / `UserManager<User>` (Identity) → pakai `IAuthRepositories` + entity `Role`/`User` custom
- [ ] Verifikasi `RoleController` — role baru masuk tabel `Roles` custom & kebaca `TokenGenerator`
- [ ] `AddAuthClient` untuk Command/Platform — *(SKIP: Command & Support.Platform tidak disentuh)*

---

## FASE 1 — PBAC Core (3–5 hari)

### Entity & Data
- [ ] Entity `Permission` — `Code` (unique), `Resource`, `Action`, `Description`
- [ ] Entity `Policy` — `Name`, `Effect` (Allow/Deny), `Priority`, `ConditionJson?`, `IsActive`
- [ ] Entity `PolicyPermission` — `PolicyId`, `PermissionId`
- [ ] Entity `RolePermission` — `RoleId`, `PermissionId`
- [ ] Entity `RolePolicy` — `RoleId`, `PolicyId` *(opsional, fase 2)*
- [ ] Entity `UserPolicy` — `UserId`, `PolicyId`, `Effect` *(direct override, opsional)*
- [ ] Seed permission dari role existing (Admin, User) → `RolePermission`

### Token
- [ ] Tambah claim `perm` di `TokenGenerator` — role claim tetap ada untuk backward compat
- [ ] Tambah claim `pver` — versi permission; naikkan saat permission user berubah

### Authorization
- [ ] `HasPermissionAttribute` — `[HasPermission(Permissions.Platform.Create)]` → policy `perm:platform.create`
- [ ] `PermissionPolicyProvider` — `IAuthorizationPolicyProvider`, generate policy dari prefix `perm:`
- [ ] `PermissionAuthorizationHandler` — handle `PermissionRequirement`, baca claim `perm`
- [ ] Registrasi di `Program.cs` — `PermissionPolicyProvider` + `PermissionAuthorizationHandler`

### Contract (`Support.Auth.Client`)
- [ ] `Permissions.cs` — konstanta kode permission
- [ ] `AuthClientExtensions.cs` — `AddAuthClient(configuration)`

### Migrasi Endpoint
- [ ] Ganti `[Authorize(Policy="Admin")]` → `[HasPermission(...)]` satu per satu

---

## FASE 2 — Policy + Kondisi (3–4 hari)

### Services
- [ ] `PolicyEvaluator` — evaluasi `ConditionJson` (ownership/tenant)
- [ ] `PermissionCache` — cache permission untuk performa
- [ ] Resource-based authorization — `AuthorizeAsync(User, resource, permission)` untuk kondisi (owner, tenant, status)

### Features & API
- [ ] CRUD Permission — `Features/Handler/Permissions/`
- [ ] CRUD Policy + kondisi — `Features/Handler/Policies/`
- [ ] `GetEffectivePermissionsQuery` — `Features/Handler/Auth/GetEffectivePermissionsQuery.cs`
- [ ] Endpoint `GET /api/a/auth/me/permissions` — untuk UI menyembunyikan menu

### UI
- [ ] Gating menu di UI — pakai endpoint `me/permissions`

### Cleanup
- [ ] Hapus policy `"Admin"`/`"User"` — setelah semua endpoint pindah ke `[HasPermission]`

---

## FASE 3 — Library Audit (4–6 hari)

### Entity & DB
- [ ] Entity `AuditLog` — Id, OccurredAtUtc, TraceId, CorrelationId, ServiceName, UserId, UserEmail, RolesSnapshot, IpAddress, UserAgent, HttpMethod, Path, Action, ResourceType, ResourceId, Result, StatusCode, DurationMs, ChangesJson, MetadataJson, ErrorMessage
- [ ] Index `AuditLog` — `(OccurredAtUtc)`, `(UserId, OccurredAtUtc)`, `(ResourceType, ResourceId)`, `(TraceId)`, GIN `MetadataJson`

### Capture (3 Lapis)
- [ ] `AuditSaveChangesInterceptor` — diff entity old→new, ditulis **di transaksi yang sama**
- [ ] `AuditRequestMiddleware` — capture semua HTTP request → `Channel<AuditEvent>` → background writer (batch insert)
- [ ] `IAuditLogger` — untuk security event & business event eksplisit
- [ ] Background writer — `Channel` + `BackgroundService`, bounded, fallback ke file/`ILogger` kalau DB down

### Aturan
- [ ] Masking PII/secret — `PasswordHash`, `RefreshToken`, `Token`, `SecurityStamp`, connection string → `***`; daftar field via konfigurasi
- [ ] Body capture opt-in — `[AuditBody]` + limit ukuran
- [ ] Append-only — tidak ada API update/delete; user DB aplikasi hanya INSERT + SELECT
- [ ] Audit of audit — akses halaman audit dicatat (`audit.read`)

### Query API
- [ ] `GET /api/a/audit` — filter userId, action, resourceType, from, to, result, page, size
- [ ] `GET /api/a/audit/{id}`
- [ ] `GET /api/a/audit/export?format=csv`
- [ ] Permission `audit.read` — lindungi query API
- [ ] Role baru `Auditor` — untuk baca audit, bukan Admin blanket

### Terapkan di Auth
- [ ] Registrasi `AddAudit(...)` di `Support.Auth.Id`
- [ ] Migration tabel `AuditLog`

---

## FASE 4 — Terapkan ke Command & Platform (3–5 hari)

- [ ] *(SKIP — Command & Support.Platform tidak disentuh)*

---

## FASE 5 — Opsional

- [ ] Partisi bulanan Postgres untuk `AuditLog`
- [ ] Job Hangfire — arsip/hapus audit (mis. 24 bulan, sesuai UU PDP)
- [ ] Hash chain (tamper-evidence) — `PrevHash` + `RowHash` SHA-256
- [ ] Alert — 403 / brute force
- [ ] Dashboard audit

---

## Keputusan yang Belum Dijawab

- [ ] **Q1. Granularitas permission** — per resource-action (`platform.create`) atau per endpoint (`platform.command.create`)?
- [ ] **Q2. ABAC** (owner, tenant, departemen, jam kerja) — fase 1 atau fase 2?
- [ ] **Q3. Token** — `perm` di JWT + `pver` (Opsi A), atau full introspection ke Auth (Opsi B)?
- [ ] **Q4. Audit storage** — per-service DB dulu (rekomendasi), atau langsung central service via RabbitMQ?
- [ ] **Q5. Cakupan data** — rekam request/response body, atau cukup metadata + diff entity?
- [ ] **Q6. Retensi audit** — 12/24 bulan? Perlu tamper-evidence (hash chain)?
- [ ] **Q7. Role Auditor baru** untuk baca audit — setuju?

---

## Ringkasan Status

| Fase | Jumlah Task | Status |
|---|---|---|
| Fase 0 | 5 item (1 di-skip) | 2 ✅ selesai, 2 ⬜ sisa, 1 ❌ skip |
| Fase 1 | 16 item | ⬜ belum |
| Fase 2 | 9 item | ⬜ belum |
| Fase 3 | 15 item | ⬜ belum |
| Fase 4 | — | ❌ skip |
| Fase 5 | 5 item | ⬜ opsional |
| Keputusan | 7 item | ⬜ belum dijawab |