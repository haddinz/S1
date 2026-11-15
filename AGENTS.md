# AGENTS.md — Konteks & Aturan Kerja Project

> **Untuk siapa:** manusia dan AI agent yang bekerja di repository ini.
>
> **Tujuan:** menjaga konsistensi implementasi, mencegah agent mengarang arsitektur, membuat abstraksi/duplikasi yang tidak perlu, dan melakukan perubahan di luar scope.
>
> **Sumber gambaran arsitektur:** [`ARCHITECTURE.md`](ARCHITECTURE.md). Dokumen ini berisi konteks dan aturan kerja agent; bukan pengganti dokumentasi arsitektur.
>
> **Jika ada konflik:** ikuti dokumen ini untuk workflow dan batasan kerja; gunakan `ARCHITECTURE.md` untuk memahami gambaran sistem. Jika keduanya tidak menjawab keputusan penting, periksa implementasi dan dokumentasi terkait, lalu tanyakan kepada user jika masih ada ambiguitas material.

> **Status `ARCHITECTURE.md`:** File tersebut masih ada di repository, tetapi sudah ketinggalan (outdated) dan tidak sedang diupdate. Isinya tidak boleh dijadikan acuan utama untuk memahami kondisi sistem saat ini.
>
> - Jangan menganggap isi `ARCHITECTURE.md` sebagai kondisi aktual.
> - Sumber kebenaran utama adalah kode aktual.
> - Jangan mengarang, melengkapi, atau "memperbaiki" `ARCHITECTURE.md` kecuali user meminta eksplisit.
> - Jika `ARCHITECTURE.md` bertentangan dengan kode, kode yang benar; laporkan ketidaksesuaiannya.
> - Jika ada ambiguitas material, tanyakan kepada user.

---

## 1. Konteks dan fakta utama project

### 1.1 Database

Database utama subsystem modern adalah **PostgreSQL**. `Support.Auth.Id` menggunakan EF Core + Npgsql. Hangfire juga menggunakan PostgreSQL yang sama (`supportdb`). Jangan mengganti provider database atau menambahkan provider alternatif untuk subsystem ini tanpa permintaan dan persetujuan eksplisit.

### 1.2 Pola aplikasi: lightweight CQRS

Pola aplikasi menggunakan pemisahan Command dan Query melalui `ICommandHandler` / `ICommandQueryHandler`. Satu use case memiliki handler sendiri. Controller menjadi HTTP boundary dan meneruskan pekerjaan aplikasi ke handler.

CQRS adalah pola desain utama yang harus menjadi acuan saat menganalisis dan menambahkan use case. Jangan memperlakukan CQRS hanya sebagai konvensi nama class.

### 1.3 Domain: DDD tactical

Project menggunakan sebagian praktik DDD tactical, bukan implementasi DDD penuh. Pola yang digunakan mencakup entity dengan private/protected constructor, static factory, private setter, behavior method, value object yang dipetakan dengan `OwnsOne`, serta repository sebagai abstraksi data access.

Jangan menambahkan pola DDD lanjutan seperti domain event, event bus, atau domain service hanya karena pola tersebut umum digunakan. Analisis kebutuhan dan dampaknya terlebih dahulu.

### 1.4 Arsitektur: modular monolith sebagai fase transisi

Arsitektur saat ini adalah **modular monolith**. Modular monolith merupakan solusi sementara yang sengaja dipertahankan sampai project benar-benar siap dipindahkan ke microservices. Dengan demikian, struktur modular saat ini bukan keputusan bahwa project akan selamanya menjadi monolith.

Arah jangka panjang project adalah berpindah ke microservices ketika waktu, kebutuhan, dan kesiapan memungkinkan. Sampai migrasi tersebut benar-benar dimulai, pengembangan tetap berjalan di struktur modular monolith yang ada. Jangan melakukan big architectural update atau big-bang refactor semata-mata untuk mempercepat transisi; migrasi microservices adalah pekerjaan tersendiri ketika user memutuskan untuk memulainya.

Selama masih menggunakan modular monolith:

- Pertahankan batas antar-module dan arah dependency yang sudah ada.
- Pisahkan kontrak publik (`Client`) dari implementasi (`Id`) agar consumer tidak terikat langsung pada detail implementasi.
- Rancang kontrak dengan mempertimbangkan kemungkinan ekstraksi service di masa depan, tetapi jangan membuat abstraksi atau infrastruktur spekulatif hanya demi kemungkinan tersebut.
- Jangan mengubah struktur besar, memecah project, atau memulai migrasi microservices kecuali memang diminta.

`Support.Auth.Id` adalah implementasi Auth. `Support.Auth.Client` adalah batas kontrak untuk consumer. Arah desainnya adalah perubahan integrasi consumer dilakukan melalui kontrak `Client` sejauh kontraknya memang mencukupi; jangan membuat consumer bergantung langsung pada `Id`.

Karena itu:

- Kontrak publik (DTO request/response dan interface) berada di `Client`.
- `Id` boleh menggunakan tipe kontrak dari `Client`.
- `Client` tidak boleh mereferensikan `Id`, EF Core, atau domain implementation.
- Jangan menaruh implementasi HTTP/SDK di dalam contract library. Implementasi transport untuk komunikasi ant service dibahas ketika ekstraksi service benar-benar dikerjakan.

### 1.5 Kondisi aktual, ide, dan keputusan

- **`ARCHITECTURE.md`:** outdated dan tidak diupdate. Statusnya arsip, bukan current implementation maupun technical decision. Jangan jadikan acuan desain.

Project berkembang secara organik berdasarkan ide dan kebutuhan yang muncul selama pengembangan. Tidak ada roadmap fitur tetap yang harus dikejar oleh agent.

Agent wajib membedakan:

- **Current implementation:** sesuatu yang benar-benar sudah diterapkan dan dapat ditemukan di kode.
- **Technical decision:** keputusan teknis yang sudah dipilih dan harus diikuti.
- **Proposal/idea:** ide atau desain yang sedang dipertimbangkan, belum menjadi kewajiban implementasi.
- **Technical debt:** kondisi yang diketahui kurang ideal, tetapi bukan otomatis task berikutnya.

Jangan menganggap fitur atau pola yang belum ada sebagai target wajib. Jangan mengubah ide menjadi roadmap tanpa arahan user.

---

## 2. Aturan keras

### 2.1 Database dan runtime

- **Dilarang mengakses database tanpa izin eksplisit.** Ini termasuk `dotnet run` jika startup menjalankan `Database.MigrateAsync()`, `dotnet ef database update`, `psql`, query manual, dan integration test yang benar-benar terhubung ke database.
- **Dilarang menjalankan aplikasi hanya untuk memeriksa apakah aplikasi dapat start.** Gunakan `dotnet build` untuk pemeriksaan kompilasi.
- Agent **tidak boleh membuat migration EF Core** (`dotnet ef migrations add`). Perubahan migration dilakukan oleh user. Agent boleh mengubah entity configuration/`DbContext` sesuai scope, lalu melaporkan bahwa migration perlu dibuat oleh user.
- Jangan mengedit `AppDbContextModelSnapshot.cs` secara manual.
- Jangan mengubah schema/tabel Hangfire secara manual.
- Jangan menghapus, mengubah, atau menjalankan operasi database yang bersifat destruktif tanpa izin eksplisit.

### 2.2 Folder yang tidak boleh disentuh

- `Support/` — legacy `Support.Platform` (net9).
- `Command/` — legacy Command service (net9).

Keduanya merupakan pengembangan/environment berbeda. Jangan membaca, mengubah, atau menjadikannya referensi pola kecuali user meminta secara eksplisit.

### 2.3 Scope perubahan

- Hanya sentuh file yang diperlukan oleh task. Tidak ada refactor oportunistik.
- Jangan memindahkan, mengganti nama, atau menghapus file di luar scope tanpa persetujuan.
- Jangan menambah project, package, library, atau layer baru tanpa persetujuan.
- Jangan mengubah `Id` hanya untuk menyesuaikan consumer. Evaluasi kontrak di `Client` terlebih dahulu.
- Jangan melakukan commit/push tanpa diminta.
- Jika menemukan masalah di luar scope, laporkan. Jangan memperbaikinya diam-diam.
- Jangan mengubah keputusan teknis yang sudah ada hanya karena agent lebih menyukai pola lain.

### 2.4 Sebelum menulis kode

- Baca implementasi yang sudah ada di area terkait.
- Cari tipe, method, handler, repository, service, DTO, dan konfigurasi yang mungkin sudah memenuhi kebutuhan.
- **Telusuri inheritance chain sebelum menyimpulkan method, property, atau behavior belum tersedia.** Periksa concrete class, seluruh base class, implemented/inherited interfaces, extension methods, dan tipe yang benar-benar digunakan pada call site. Lihat `SHARED-ABSTRACTIONS.md`.
- Jangan membuat ulang capability yang sudah diwariskan dari base class, dimiliki entity, atau disediakan shared service.
- Jika method tersedia pada concrete class tetapi tidak terlihat melalui interface yang digunakan consumer, identifikasi perbedaan tersebut terlebih dahulu. Jangan langsung membuat implementasi duplikat atau mengubah kontrak tanpa analisis.
- Ikuti pola project yang berlaku; jangan menciptakan pola baru tanpa alasan teknis.
- Identifikasi apakah perubahan termasuk fitur biasa atau perubahan arsitektur.
- Jika requirement memiliki ambiguitas yang memengaruhi behavior, security, data, public contract, atau dependency, tanyakan sebelum mengambil keputusan.
- Jangan bertanya ulang untuk informasi yang sudah tersedia di percakapan atau repository.

### 2.5 Setelah menulis kode

- Jalankan `dotnet build` untuk setiap project yang berubah, selama build tidak memicu akses database atau operasi terlarang.
- Laporkan file yang berubah dan alasan perubahannya.
- Bedakan hasil build, static analysis, unit test, integration test, dan runtime verification. Jangan mengklaim sudah teruji jika hanya melakukan build.
- Jangan mengklaim telah mengubah arsitektur hanya karena menambahkan satu file atau satu class.

---

## 3. Peta project

| Project | Peran | Bergantung pada | Status/keterangan |
|---|---|---|---|
| `Support.Auth.Id` | API identity/access yang di-deploy; host Notification module | `Support.Auth.Client`, `Support.Notification.Client`, `Support.Notification.Id` | Inti implementasi aplikasi |
| `Support.Auth.Client` | Kontrak publik Auth: DTO request/response dan interface | Tidak boleh bergantung pada `Id` | Contract library |
| `Support.Notification.Id` | Module notifikasi (Hangfire queue + MailKit) yang di-host di Auth | `Support.Notification.Client` | Module, bukan service terpisah |
| `Support.Notification.Client` | Kontrak notifikasi (`IEmailQueueServiceClient`, `INotificationClient`, DTO) | Tidak boleh bergantung pada `Notification.Id` | Contract library |
| `Support.UI` | React + Vite scaffold | Integrasi melalui contract/client ketika dikerjakan | Belum terintegrasi |
| `Support/` | Legacy Platform (net9, gRPC + RabbitMQ) | — | **OUT OF SCOPE** |
| `Command/` | Legacy Command (net9, RabbitMQ + gRPC) | — | **OUT OF SCOPE** |

### 3.1 Arah dependency

```text
Support.Notification.Client  <──  Support.Notification.Id  <──  Support.Auth.Id
         ▲                                                            │
         └────────────────────────────────────────────────────────────┤
                                                                      │
Support.Auth.Client         <─────────────────────────────────────────┘

Support.Auth.Id  ──X──>  implementasi HTTP/SDK di dalam Client
```

Aturan:

- `Client` adalah hulu kontrak, bukan tempat implementasi server.
- `Id` adalah implementasi dan boleh mengonsumsi kontrak.
- `Domain` tidak boleh mengetahui `Client`, DTO, ASP.NET, atau EF Core.
- `Features` (application layer) boleh mengetahui `Domain`, `Repositories.Interfaces`, `Services.Interfaces`, dan kontrak `Client` jika diperlukan.
- Jangan membalik arah dependency hanya untuk mempermudah satu use case.

### 3.2 Penambahan Service baru dan Service Definition

Penambahan service/module baru merupakan bagian yang wajar dari fase pengembangan project ini. Agent harus siap membantu membentuk pasangan project `Support.<Service>.Client` dan `Support.<Service>.Id` ketika memang diminta. Namun, desain keduanya harus berasal dari definisi Service yang sudah disediakan atau disetujui user, bukan dari asumsi agent atau template generik semata.

**Service Definition adalah acuan utama untuk memahami service yang akan ditambahkan.** Informasi tersebut dapat diberikan langsung oleh user atau dicatat pada dokumen khusus yang dirujuk dari repository. Sebelum membuat project atau kode, agent wajib mencari dan membaca definisi Service yang relevan.

Service Definition idealnya menjelaskan hal-hal berikut sesuai kebutuhan service:

- **Name and purpose:** nama service dan masalah/tanggung jawab yang dimiliki.
- **Scope and boundaries:** kemampuan/use case yang menjadi tanggung jawab service serta hal yang bukan tanggung jawabnya.
- **Domain ownership:** entity, aggregate, value object, dan business rule yang dimiliki service jika sudah ditentukan.
- **Data ownership:** data yang dikelola, database/persistence yang digunakan, dan batas kepemilikan data jika sudah diputuskan.
- **Public capabilities/contracts:** operasi atau kemampuan yang perlu digunakan consumer; tidak harus mengekspos seluruh internal service.
- **Dependencies and consumers:** module/service lain yang dibutuhkan dan pihak yang akan menggunakan service ini.
- **Hosting and deployment:** apakah implementasi masih menjadi module dalam modular monolith atau memiliki host/deployment tersendiri.
- **Communication mechanism:** in-process, HTTP, gRPC, messaging, atau mekanisme lain hanya jika memang sudah ditentukan.
- **Security requirements:** authentication, authorization, trust boundary, dan data sensitif yang relevan.
- **Constraints and status:** batasan teknis, keputusan yang sudah diterima, serta hal yang masih berupa proposal/pertanyaan.

Tidak semua informasi harus sudah tersedia sejak awal. Jika suatu detail belum ditentukan, tandai sebagai **Belum Diputuskan**. Jangan mengarang domain, endpoint, database, protokol komunikasi, deployment topology, atau dependency untuk mengisi kekosongan. Ajukan pertanyaan hanya untuk keputusan yang benar-benar menghalangi desain atau implementasi.

#### Aturan membentuk `Service.Client` dan `Service.Id`

Ketika Service Definition sudah tersedia dan user meminta implementasinya:

1. **Inspect Service Definition dan implementasi existing.** Pastikan agent memahami tanggung jawab service, use case, data ownership, dependency, consumer, dan batasannya. Periksa juga pola `Client`/`Id` service lain yang sudah ada.
2. **Rancang `Support.<Service>.Client` dari public capabilities.** Isinya hanya kontrak yang diperlukan consumer, seperti request/response DTO, interface, enum, atau constant lintas boundary. Jangan mengekspos entity domain, repository, `DbContext`, EF Core configuration, atau detail internal implementation.
3. **Rancang `Support.<Service>.Id` sebagai implementasi Service.** Tempatkan domain behavior, CQRS handlers, repository/persistence, service teknis, dan entry point sesuai tanggung jawab serta hosting model yang ditentukan dalam Service Definition. Implementasi harus memenuhi kontrak publik yang disepakati.
4. **Jaga dependency direction.** `Client` tidak boleh mereferensikan `Id` atau detail internal implementation. Consumer menggunakan `Client` sebagai boundary. `Id` boleh menggunakan kontrak `Client` miliknya sendiri jika sesuai pola project. Jangan membuat consumer baru bergantung langsung pada project `Id`.
5. **Ikuti modular monolith yang sedang berjalan.** Penambahan pasangan `Client`/`Id` tidak otomatis berarti membuat microservice yang ter-deploy terpisah. Hosting, database terpisah, network transport, dan deployment mandiri hanya diterapkan jika Service Definition dan keputusan user memang menetapkannya.
6. **Jaga service boundary.** Jangan memindahkan domain atau data ownership dari service lain hanya demi kemudahan implementasi. Jika service baru membutuhkan kemampuan service lain, gunakan kontrak dan pola integrasi yang telah disetujui; jangan mengakses repository atau `DbContext` milik service lain secara langsung.
7. **Implementasikan sesuai scope.** Tidak semua service harus memiliki controller HTTP, database sendiri, background worker, atau semua layer. Buat hanya komponen yang memang dibutuhkan oleh Service Definition.
8. **Laporkan keputusan dan gap.** Sebutkan bagian Service Definition yang digunakan, project/komponen yang akan dibuat atau diubah, dependency yang muncul, serta keputusan yang masih belum tersedia.

#### Hal yang tidak boleh diasumsikan

- Tidak setiap feature membutuhkan service baru atau pasangan project `Client`/`Id`.
- Tidak setiap `Service.Id` harus menjadi executable API/host mandiri.
- Tidak setiap service harus memiliki database sendiri.
- Tidak semua komunikasi antarmodule harus menggunakan HTTP/gRPC/message broker saat masih dalam modular monolith.
- Jangan membuat implementasi transport/SDK di `Client` kecuali memang sudah menjadi keputusan dan scope terpisah.
- Jangan menyalin seluruh struktur service existing secara otomatis; gunakan bagian yang relevan dan pertahankan tanggung jawab setiap service.

Jika Service Definition belum ada, bertentangan dengan kode aktual, atau tidak cukup untuk menentukan batas tanggung jawab, berhenti pada analisis dan ajukan pertanyaan/opsi desain. Jangan membuat `Client` dan `Id` berdasarkan tebakan.

---

## 4. Peta folder `Support.Auth.Id/src`

| Path | Tanggung jawab | Boleh | Dilarang |
|---|---|---|---|
| `Commons/Command`, `Commons/CommandQuery` | Interface CQRS | Menambah interface generik jika benar-benar dibutuhkan | Business logic |
| `Controller/` | HTTP boundary | Validasi `ModelState`, mapping request ke command/query, memanggil handler, membentuk HTTP response | Business logic, akses `DbContext`, query langsung |
| `Features/Handler/<Area>/` | Command/Query dan handler per use case | Application orchestration, validasi yang memerlukan repository, memanggil domain dan repository | Akses HTTP/`HttpContext`, SQL mentah, business behavior yang seharusnya milik entity |
| `Domain/Entity/` | Entity, behavior, dan base models | Invariant, factory, state transition | DTO, atribut HTTP, EF query, referensi `Client` |
| `Domain/ValueObject/` | Value object | Nilai immutable dan validasi invariant nilai | HTTP concern atau application orchestration |
| `Domain/Exceptions/` | Exception domain | Tipe exception domain jika memang dibutuhkan | Mapping HTTP response |
| `Domain/Extention/` | Lokasi legacy yang perlu kehati-hatian | Pertahankan yang sudah ada selama belum masuk scope cleanup | Menambah mapping DTO atau web concern baru |
| `Models/DTOs/` | DTO internal `Id` | DTO yang hanya digunakan internal | DTO publik baru yang seharusnya berada di `Client` |
| `Models/Enum/` | Enum internal domain/aplikasi | Enum yang hanya digunakan `Id` | Enum kontrak publik baru |
| `Repositories/Interfaces/` | Abstraksi repository | Method persistence/query yang memang dibutuhkan | Business behavior atau implementasi EF |
| `Repositories/` | Implementasi persistence EF Core | Query, `SaveChanges`, transaksi, persistence mapping | Business rule/use-case orchestration |
| `Repositories/Data/Configuration/` | EF Core configuration | Mapping tabel, index, relationship, query filter | Query use case |
| `Migrations/` | Migration EF Core | File migration yang dibuat user | Pembuatan atau edit manual oleh agent tanpa instruksi eksplisit |
| `Services/Security/` | Komponen keamanan teknis | Hashing, token, password policy, dan utility keamanan sesuai pola yang ada | Business flow use case |
| `Services/BackgroundServices/` | Background processing jika memang berada di folder ini | Integrasi job/worker sesuai penggunaan Hangfire yang sudah ada | Outbox worker atau business logic use case |
| `Middleware/` | Middleware HTTP | Error handling dan concern HTTP lintas request | Business logic |
| `DI.cs` | Registrasi DI utama | Registrasi service/handler | Business logic atau konfigurasi pipeline HTTP |
| `Program.cs` | Composition root dan pipeline | Auth, authorization, Hangfire, DbContext, middleware | Business logic |

### 4.1 Aturan penempatan cepat

| Yang akan dibuat | Lokasi |
|---|---|
| Request/Response publik | `Support.Auth.Client/DTOs/...` |
| Interface lintas module/service | `Support.*.Client/Contract/...` |
| Entity/value object | `Support.Auth.Id/src/Domain/...` |
| Command/Query + handler | `Support.Auth.Id/src/Features/Handler/<Area>/` |
| Repository interface | `Support.Auth.Id/src/Repositories/Interfaces/` |
| EF configuration | `Support.Auth.Id/src/Repositories/Data/Configuration/` |
| Service teknis | `Support.Auth.Id/src/Services/...` |
| Middleware | `Support.Auth.Id/src/Middleware/` |
| Migration | `Support.Auth.Id/src/Migrations/` — dibuat oleh user |

Peta folder adalah gambaran struktur saat ini, bukan izin untuk menambah layer baru. Jika struktur aktual berbeda, periksa repository dan dokumentasi terkait sebelum mengubahnya.

### 4.2 Shared abstractions dan inheritance awareness

Project memiliki base class dan shared abstraction yang menyediakan capability lintas fitur. Agent wajib memeriksa abstraction tersebut sebelum menambahkan method, property, atau behavior baru.

- `BaseRepositories` menyediakan capability persistence dan transaction yang digunakan repository turunannya, termasuk SaveChanges, Begin Transaction, Commit, dan Rollback. Gunakan method yang benar-benar tersedia pada source code; jangan membuat ulang method tersebut di setiap repository.
- `BaseModels` menyediakan properti dan behavior umum entity, termasuk `Id`, `CreatedAt`, `UpdateAt`, `IsDeleted`, `SetUpdate()`, dan `SetSoftDelete()`. Entity yang mewarisi `BaseModels` harus menggunakan behavior tersebut, bukan menduplikasi properti atau method soft delete.
- Repository bertanggung jawab memuat dan menyimpan state. Perubahan state soft delete dilakukan melalui behavior entity/base model yang tersedia, kemudian dipersist melalui mekanisme repository/unit-of-work existing.
- Nama, signature, visibility, cancellation token, dan return type method wajib dikonfirmasi dari source code. Jangan menebak nama method hanya berdasarkan deskripsi capability.
- Inheritance class tidak otomatis membuat semua method tersedia melalui interface. Periksa juga interface yang diinjeksi ke consumer dan static type pada call site. Jika capability ada pada base class tetapi tidak diekspos oleh interface consumer, jelaskan gap tersebut dan ikuti pola boundary existing; jangan membuat method duplikat secara otomatis.
- Katalog capability, cara penelusuran, dan aturan penggunaannya terdapat di [`SHARED-ABSTRACTIONS.md`](SHARED-ABSTRACTIONS.md). Jika katalog berbeda dengan source code, source code menjadi acuan implementasi dan ketidaksesuaian harus dilaporkan.

---

## 5. Konvensi CQRS

### 5.1 Command

- Nama: <Verb><Noun>Command, contoh CreateRoleCommand, AssignRoleCommand, atau ChangePasswordCommand.
- Gunakan ICommandHandler<TCommand> jika tidak perlu mengembalikan data.
- Gunakan ICommandHandler<TCommand, TResult> hanya jika hasil memang dibutuhkan oleh pemanggil.
- Satu command = satu handler = satu use case.
- Command merepresentasikan permintaan untuk melakukan tindakan, mengubah state sistem, atau menghasilkan side effect.
- Command tidak terbatas pada operasi CRUD. Contohnya Login, Logout, Refresh Token, Verify Email, Change Password, dan Assign Role dll.
- Gunakan property dengan init untuk menampung input Command

Handler bertanggung jawab untuk:
1. Melakukan validasi use case yang memerlukan repository atau data eksternal.
2. Memuat aggregate yang diperlukan oleh use case.
3. Memanggil behavior dan invariant domain melalui method entity.
4. Melakukan orchestration terhadap repository atau service yang diperlukan.
5. Menyimpan perubahan satu kali melalui mekanisme persistence yang berlaku.
6. Mengembalikan hasil jika use case membutuhkannya.

Aturan tambahan:
1. Jangan memindahkan business behavior entity ke handler hanya agar handler terlihat lengkap.
2. Jangan menaruh orchestration lintas repository/service di dalam entity.
3. Jangan memuat aggregate yang tidak diperlukan.
4. Validasi invariant domain tetap menjadi tanggung jawab domain entity.
5. Handler bertanggung jawab atas orchestration dan validasi use case yang membutuhkan informasi eksternal atau repository.


### 5.2 Query

- Nama: Get<Entity>Query, Search<Entity>Query, atau List<Entities>Query, sesuai tujuan use case.
- Implementasikan ICommandQuery<TResult>.
- Query harus bersifat read-only dan tidak mengubah state domain atau menghasilkan side effect.
- Gunakan AsNoTracking() untuk query EF Core yang tidak memerlukan entity tracking atau Dapper namun Dapper sebagai prioritas.
- Untuk list gunakan PaginationRequest dan PagedResponse<T> yang sudah tersedia.
- Query tidak memanggil SaveChangesAsync() dan tidak melakukan perubahan state.
- Jangan memuat aggregate lengkap jika projection/DTO query sudah cukup.
- Gunakan property dengan init untuk menampung input Query.

Cakupan Query meliputi:
- Get detail dan Get by ID.
- Get by email atau identifier lainnya.
- List dan pagination.
- Search dan faceted search.
- Filtering dan sorting.
- Lookup dan pencarian data lainnya.

Aturan tambahan:
- Query ditentukan berdasarkan tujuan operasi, bukan semata-mata prefix nama method.
- Jangan membuat Query Handler baru hanya karena terdapat method Get pada repository.
- Bedakan use case Query pada application layer dengan method pengambilan data internal pada repository.
- Gunakan projection jika hanya sebagian data yang dibutuhkan oleh response.


## 5.3 Batas Tanggung Jawab Command dan Query

| Aspek | Command | Query |
| :--- | :--- | :--- |
| **Tujuan** | Melakukan tindakan | Membaca informasi |
| **Perubahan State** | Diperbolehkan | Tidak diperbolehkan |
| **Side Effect** | Diperbolehkan sesuai use case | Tidak diperbolehkan |
| **Handler** | `ICommandHandler` | `IQueryHandler` |
| **Persistence** | Dapat menyimpan perubahan | Read-only |
| **Contoh** | Login, Register, Assign Role | Get User, Search Users, List Roles |


### 5.4 Controller

- Controller hanya menangani HTTP boundary: validasi `ModelState`, mapping request ke command/query, memanggil handler, dan mengembalikan response.
- Dilarang mengakses `DbContext` atau repository secara langsung.
- Dilarang menempatkan business rule di controller.
- Gunakan `[Authorize]`, `[Authorize(Policy = "...")]`, atau `[AllowAnonymous]` secara eksplisit sesuai kebutuhan endpoint.
- Controller tidak menentukan keputusan domain atau melakukan orchestration lintas service.

### 5.5 Transaksi dan background processing

- Satu operasi tulis sederhana cukup menggunakan `SaveChangesAsync()` sesuai repository/unit-of-work yang tersedia.
- Operasi multi-langkah yang harus atomik menggunakan mekanisme transaksi yang sudah tersedia, misalnya `ExecuteInTransactionAsync()`.
- Jangan membuat transaksi baru di dalam transaksi yang sudah berjalan.
- Background job dan queued processing saat ini menggunakan **Hangfire**. Ikuti pola enqueue/scheduling, queue, dan worker yang sudah ada.
- Untuk email atau efek samping eksternal, periksa urutan commit dan enqueue yang digunakan oleh fitur sejenis sebelum menambahkan implementasi baru.
- Jangan mengasumsikan enqueue job dan penyimpanan data bisnis otomatis atomik. Jika requirement membutuhkan konsistensi/ketahanan terhadap retry, analisis failure scenario dan idempotency terlebih dahulu.
- **Jangan menggunakan atau menghidupkan kembali Outbox Pattern/`OutboxMessage`.** Outbox tidak lagi menjadi mekanisme aktif project; background processing telah menggunakan Hangfire.

### 5.6 Exception

Gunakan exception yang sudah tersedia dan ikuti mapping pada `ExceptionMiddleware`:

- `BusinessValidationException` → HTTP 400.
- `UnauthorizedException` → HTTP 401.
- `NotFoundException` → HTTP 404.
- `ConflictException` → HTTP 409.

Jangan melempar `Exception` generik untuk kesalahan bisnis. Jangan menangani exception domain di controller jika middleware memang sudah bertanggung jawab memetakannya.


## 5.7 DTO, Command, Entity, dan Repository Boundaries

Setiap layer memiliki tanggung jawab yang berbeda terhadap penggunaan data.

### 1. Controller / API Boundary
* Menerima Request DTO dari HTTP request.
* Mengubah data request menjadi Command atau Query.
* Mengembalikan Response DTO kepada consumer.
* Jangan meneruskan API Request DTO langsung ke Repository.

### 2. Command / Query
* Command dan Query merupakan representasi kebutuhan *use case* pada *application layer*.
* Command membawa data yang diperlukan untuk menjalankan operasi.
* Query membawa parameter yang diperlukan untuk membaca data.
* Jangan menggunakan Domain Entity sebagai pengganti Command atau Query.

### 3. Handler
* Bertanggung jawab menghubungkan Command/Query dengan Domain, Repository, dan Service.
* Membuat atau memuat Domain Entity sesuai kebutuhan *use case*.
* Menggunakan *behavior method* entity untuk perubahan *state*.
* Melakukan *mapping* Entity ke Response DTO sesuai konvensi *mapping* project.
* Tidak menyerahkan tanggung jawab *business behavior* kepada DTO.

### 4. Domain Entity
* Merupakan representasi objek domain yang memiliki *identity*, *state*, *invariant*, dan *behavior*.
* Dibuat melalui *factory method* atau mekanisme domain yang sudah tersedia.
* Tidak boleh bergantung pada Request DTO, Response DTO, atau DTO dari Client.

### 5. Repository
* Repository bertanggung jawab terhadap *persistence* dan *data access*.
* Untuk operasi *write*, Repository menerima dan menyimpan Domain Entity atau Aggregate yang menjadi tanggung jawabnya.
* Repository tidak menerima API Request DTO, Response DTO, atau Contract DTO sebagai pengganti Domain Entity.
* Repository tidak bertanggung jawab mengubah DTO menjadi Domain Entity.
* Repository tidak boleh memindahkan *business behavior* dari Entity ke dalam implementasi *persistence*.
* Jangan membuat method `AddAsync(RoleDto)`, `UpdateAsync(UserDto)`, atau pola serupa jika seharusnya Repository bekerja dengan Domain Entity.
* Gunakan Entity yang sudah dimuat dari Repository untuk melakukan perubahan *state* melalui *behavior method*, kemudian simpan perubahan menggunakan *persistence mechanism* yang tersedia.

### 6. Query dan Read Projection
* Default project ini adalah *mapping* Entity ke Response DTO dilakukan di Handler.
* Untuk kebutuhan pembacaan yang memerlukan *projection* khusus atau optimasi query, *agent* boleh mengusulkan *read projection* yang sesuai.
* *Read projection* harus memiliki tujuan yang jelas dan bukan sekadar menggunakan API DTO sebagai parameter Repository.
* Jangan mengubah konvensi *mapping* yang sudah ada tanpa alasan teknis dan analisis dampak.

---

### Prinsip Utama
> DTO digunakan untuk pertukaran data antar *boundary*, Command/Query digunakan untuk merepresentasikan *use case*, Domain Entity digunakan untuk *business behavior* dan *state*, sedangkan Repository digunakan untuk *persistence*. Jangan mencampur tanggung jawab tersebut.


### 5.8 Mapping

- Mapping entity ke DTO dilakukan di handler/application layer, bukan di `Domain`.
- Untuk 1–2 call site, lakukan inline. Jangan membuat file mapper/extension hanya untuk pemakaian terbatas.
- Untuk 3+ call site atau mapping yang benar-benar kompleks, pertimbangkan ekstraksi ke application layer.
- Jangan menambah AutoMapper/Mapperly atau menjadikannya pola wajib tanpa persetujuan. `Riok.Mapperly` sudah ada di package, tetapi bukan pola wajib project.

### 5.9 CQRS design checklist

Sebelum membuat use case, tentukan:

1. Apakah kebutuhan ini Command atau Query?
2. Apakah use case mengubah state atau hanya membaca data?
3. Jika ada perubahan state, invariant dan behavior mana yang menjadi tanggung jawab entity?
4. Apa orchestration yang harus dilakukan handler?
5. Apakah repository yang ada sudah cukup, atau memang ada kebutuhan data access baru?
6. Apakah diperlukan service teknis atau integrasi eksternal?
7. Apakah perubahan memerlukan public contract baru di `Client`?
8. Apakah ada kebutuhan transaksi, concurrency control, idempotency, atau retry handling?
9. Apakah ada perubahan authorization/security yang perlu dianalisis?

Checklist ini untuk membantu desain, bukan kewajiban membuat class atau abstraksi untuk setiap pertanyaan.

---

## 6. Konvensi DDD tactical

### 6.1 Entity

- Constructor `private` atau `protected` sesuai kebutuhan EF Core dan pola existing.
- Pembuatan object melalui static factory method, misalnya `User.Register` atau `Role.Create`.
- Setter state domain bersifat `private`.
- Perubahan state dilakukan melalui behavior method, misalnya `UpdateProfile`, `AssignRole`, `Update`, atau `SetSoftDelete`.
- Invariant domain dijaga di entity, bukan di controller atau repository.
- Hindari entity anemic dengan property dan setter publik.
- Jangan memindahkan semua validasi ke entity secara membabi buta. Validasi format input/transport dapat berada di boundary; invariant yang menjaga validitas state domain berada di domain.

### 6.2 Value object

- Value object tidak memiliki identity dan harus menjaga immutability.
- Value object yang sudah menggunakan pola EF owned type dipetakan dengan `OwnsOne` sesuai konfigurasi existing.
- Contoh yang ada: `EmailVerification` dan `PasswordReset`.
- Jangan membuat value object untuk satu field sederhana tanpa kebutuhan invariant atau perilaku nilai yang jelas.

### 6.3 Aggregate dan relationship

- Aggregate root yang tercatat pada desain saat ini adalah `User` dan `Role`.
- `RefreshToken` merupakan domain entity yang berada di dalam lifecycle/aggregate `User`, bukan aggregate root terpisah berdasarkan desain yang tercatat saat ini.
- `User.Roles` ↔ `Role.Users` merupakan kompromi relasi existing. Jangan menyalin pola navigation dua arah ini ke aggregate baru.
- Untuk relasi baru, utamakan referensi ID atau join entity yang eksplisit. Jangan menambah navigation collection dua arah antar aggregate tanpa alasan dan review desain.
- Satu transaksi sebaiknya menjaga konsistensi aggregate yang sedang diubah. Jika satu use case harus mengubah beberapa aggregate, jelaskan alasan, batas konsistensi, dan alternatifnya sebelum implementasi.
- Jangan menjadikan `BaseModels` sebagai pengganti entity/domain behavior. `BaseModels` adalah base class untuk properti persistence bersama; behavior spesifik tetap berada pada entity yang relevan.

### 6.4 Domain event dan Outbox

- Domain event belum menjadi pola aktif dalam implementasi saat ini.
- Jangan menambahkan domain event, event bus, atau event dispatcher tanpa kebutuhan yang jelas dan persetujuan desain.
- `OutboxMessage` dan Outbox worker bukan bagian dari mekanisme aktif project. Mekanisme background processing yang digunakan saat ini adalah Hangfire.
- Jika masih ditemukan source code Outbox worker, jangan diregistrasikan atau digunakan kembali. Karena Outbox sudah dinyatakan tidak dipakai, source code worker Outbox yang tersisa boleh dihapus. Penghapusan tabel, migration, atau data database tetap memerlukan izin eksplisit.

---

## 7. Aturan kontrak (`Support.*.Client`)

### 7.1 Boleh

- DTO request/response publik.
- Interface kontrak, misalnya `IEmailQueueServiceClient` dan `INotificationClient`.
- Konstanta yang memang merupakan bagian dari kontrak.
- Enum yang dipakai lintas consumer.

### 7.2 Dilarang

- Referensi ke `Support.Auth.Id` atau `Support.Notification.Id`.
- EF Core, `DbContext`, entity domain, atau atribut ASP.NET.
- Business logic.
- Implementasi HTTP/SDK di dalam contract library.

### 7.3 Perubahan kontrak

- Perubahan harus additive dan backward-compatible jika memungkinkan.
- Jangan mengubah nama atau menghapus properti DTO yang sudah digunakan consumer tanpa persetujuan.
- Jangan memindahkan DTO atau enum antar project secara oportunistik saat mengerjakan fitur lain.
- Jika perubahan kontrak memaksa implementasi `Id` mengetahui detail consumer, evaluasi ulang batas kontrak dan diskusikan sebelum mengubah dependency.

---

## 8. Aturan anti-redundansi

Aturan ini wajib diperiksa sebelum membuat class, interface, method, service, DTO, atau project baru.

### 8.1 Cari existing terlebih dahulu

Periksa konsep dan implementasi yang sudah ada, setidaknya di:

- `Support.Auth.Id/src/Commons/`
- `Support.Auth.Id/src/Services/`
- `Support.Auth.Id/src/Repositories/Interfaces/`
- `Support.Auth.Id/src/Features/Handler/<Area>/`
- `Support.Auth.Client/`
- `Support.Notification.Client/`

Kemudian:

1. Hitung call site/pemakai yang nyata.
2. Pastikan penempatan sesuai layer.
3. Periksa apakah DTO atau tipe dengan bentuk serupa sudah tersedia.
4. Periksa apakah behavior sudah dimiliki entity atau service existing.
5. Telusuri base class, parent class, interface, extension method, dan shared abstraction yang relevan. Jangan menilai ketersediaan method hanya dari isi file concrete class.
6. Periksa tipe yang digunakan consumer. Method pada concrete class belum tentu terekspos melalui interface yang diinjeksi.
7. Baca [`SHARED-ABSTRACTIONS.md`](SHARED-ABSTRACTIONS.md) sebelum menambahkan capability umum yang mungkin sudah disediakan project.

### 8.2 Aturan ekstraksi sederhana

| Jumlah pemakaian | Default keputusan |
|---|---|
| 1 | Inline; jangan membuat helper/file baru tanpa alasan jelas |
| 2 | Inline atau `private static` pada file yang sama |
| 3+ atau logic kompleks | Pertimbangkan ekstraksi jika meningkatkan kejelasan dan mengurangi duplikasi nyata |

Aturan ini adalah default, bukan angka mutlak. Abstraksi untuk boundary arsitektur/security dapat dibutuhkan meskipun implementasinya baru satu.

### 8.3 Jangan dibuat tanpa alasan teknis

- Mapper/extension untuk satu DTO dengan 1–2 pemakaian.
- Interface dengan satu implementasi, kecuali memang menjadi boundary yang diperlukan (misalnya repository atau external service).
- Service yang hanya meneruskan panggilan tanpa menambah tanggung jawab.
- Base class untuk satu child.
- Wrapper repository untuk satu method tanpa manfaat yang jelas.
- DTO baru yang shape-nya sama dengan DTO existing.
- Enum baru untuk satu nilai yang tidak membutuhkan semantik enum.
- Layer atau project baru hanya untuk memindahkan beberapa file.

### 8.4 Kesalahan sebelumnya yang tidak boleh diulang

- Membuat `RoleMappingExtention` di `Domain` untuk dua handler. Mapping seharusnya berada di handler/application layer.
- Menghitung `IsBuiltIn` di dua tempat (domain dan mapper). Satu konsep harus memiliki satu sumber kebenaran.
- Memindahkan DTO secara oportunistik ketika mengerjakan fitur Role. Perubahan file harus sesuai scope.
- Menambah `RoleDependencyInjection` terpisah tanpa kebutuhan jelas. Default project adalah registrasi terpusat di `DI.cs`; ikuti pola lokal hanya jika sudah ada alasan/kebutuhan.
- Menjalankan aplikasi yang otomatis melakukan migration ke database hanya untuk memeriksa startup. Gunakan build untuk pemeriksaan kompilasi.

---

## 9. Workflow feature dan architecture change

Ada dua tingkat workflow. Gunakan workflow ringan untuk fitur yang mengikuti pola existing. Gunakan workflow desain lengkap jika ide baru memengaruhi arsitektur, dependency, persistence, security, public contract, atau konsistensi lintas komponen.

### 9.1 Feature workflow — fitur mengikuti pola existing

1. **Scope:** pahami apa yang diminta dan batas perubahan.
2. **Inspect:** cari handler, DTO, repository method, entity behavior, exception, dan service yang sudah ada.
3. **Classify:** tentukan Command/Query dan apakah perubahan membutuhkan public contract.
4. **Domain:** ubah entity hanya jika ada behavior atau invariant domain yang memang perlu ditambahkan.
5. **Repository:** tambahkan method interface dan implementasi hanya jika kebutuhan data access belum dipenuhi oleh method yang ada.
6. **Handler:** implementasikan orchestration satu use case per handler.
7. **Controller:** ubah HTTP boundary hanya jika use case perlu diekspos melalui API.
8. **DI:** registrasikan melalui `DI.cs` sebagai default; gunakan pola feature-local hanya jika area tersebut memang sudah menggunakannya.
9. **Verify:** jalankan build dan pemeriksaan lain yang diizinkan.
10. **Report:** laporkan file berubah, alasan, hasil verifikasi, dan hal yang belum diverifikasi.

Urutan tersebut adalah panduan, bukan keharusan membuat perubahan pada semua layer. Query sederhana, misalnya, tidak harus mengubah entity atau controller jika memang tidak diperlukan.

### 9.2 New feature & architecture design workflow

Gunakan workflow ini ketika user membawa ide baru atau ketika implementasi berpotensi mengubah desain teknis yang sudah ada.

| Tahap | Aktivitas |
|---|---|
| 1. Requirement analysis | Pahami masalah/tujuan yang ingin diselesaikan. Jangan mengasumsikan solusi dari nama fitur. |
| 2. Existing analysis | Periksa implementasi, kontrak, dependency, pola yang sudah tersedia, dan Service Definition yang disediakan user jika perubahan menambah/mengubah service. |
| 3. Impact analysis | Identifikasi komponen/layer yang terdampak: Domain, Handler, Repository/DB, API/Client, DI, Security, dan background processing bila relevan. |
| 4. Technical design | Usulkan desain, penempatan kode, alur data, dan batas tanggung jawab. |
| 5. Trade-off review | Jelaskan alternatif yang relevan, kompleksitas, risiko, dan konsekuensi. Tidak perlu membuat banyak alternatif jika desain existing sudah memadai. |
| 6. Decision/approval | Minta keputusan user jika ada perubahan arsitektur, dependency, schema/data, security, package/project, atau trade-off yang belum diputuskan. |
| 7. Implementation | Implementasikan sesuai scope dan keputusan yang disetujui. |
| 8. Verification | Build/test sesuai izin dan kemampuan test harness; laporkan batas verifikasi. |

Ketentuan tambahan:

- Jika user secara jelas meminta implementasi dan desainnya mengikuti keputusan/pola existing, agent tidak perlu meminta approval berulang untuk detail rutin.
- Jika ada pilihan teknis material yang belum ditentukan, jangan memilih diam-diam. Sajikan rekomendasi dan minta keputusan.
- Jika user hanya meminta desain, berhenti pada rancangan; jangan mulai mengubah kode.
- Jangan mengubah ide baru menjadi roadmap atau kewajiban pengembangan berikutnya.

### 9.3 Technical decision record (ADR)

Gunakan ADR untuk mencatat keputusan yang berdampak penting dan kemungkinan akan menjadi acuan perubahan berikutnya, misalnya perubahan batas module, dependency, persistence, security model, public contract, atau infrastructure.

ADR tidak perlu dibuat untuk setiap penambahan class atau fitur rutin. Jika keputusan cukup mengikuti pola existing, dokumentasi task/implementation sudah memadai.

ADR yang dibuat minimal menjelaskan:

- **Context:** masalah atau kebutuhan yang memicu keputusan.
- **Decision:** keputusan yang dipilih dan statusnya (`Proposed`, `Accepted`, atau `Superseded`).
- **Alternatives:** alternatif yang benar-benar relevan dan alasan tidak dipilih.
- **Consequences:** dampak, trade-off, batasan, dan konsekuensi untuk perubahan berikutnya.

Jangan menulis proposal sebagai keputusan yang sudah diterima. Pembuatan atau perubahan file ADR mengikuti scope task dan tidak boleh dilakukan diam-diam untuk keputusan yang belum disetujui.

### 9.4 Template command handler

Contoh berikut hanya template ilustratif. Sesuaikan dengan signature interface, repository, dan exception yang benar-benar tersedia di project.

Gunakan property dengan init untuk menampung input Command

```csharp
public class DoSomethingCommand : ICommand
{
    public Guid Id { get; init; }
}

public class DoSomethingCommandHandler : ICommandHandler<DoSomethingCommand>
{
    private readonly IThingRepositories _repo;

    public DoSomethingCommandHandler(IThingRepositories repo)
    {
        _repo = repo;
    }

    public async Task HandleAsync(
        DoSomethingCommand command,
        CancellationToken cancellationToken = default)
    {
        Thing thing = await _repo.GetByIdAsync(command.Id, cancellationToken)
            ?? throw new NotFoundException($"Thing '{command.Id}' was not found.");

        thing.DoSomething();

        await _repo.SaveChangesAsync(cancellationToken);
    }
}
```

### 9.5 Template query handler

Query harus menggunakan jalur baca yang sesuai dan `AsNoTracking()` di implementasi EF jika tracking tidak dibutuhkan.
Namun
Query akan lebih baik menggunakan Dapper (Prioritas utama menggunkan Dapper)

Gunakan property dengan init untuk menampung input Query.

```csharp
public class GetThingByIdQuery : ICommandQuery<ThingResponse>
{
    public Guid Id { get; init; }
}

public class GetThingByIdQueryHandler
    : ICommandQueryHandler<GetThingByIdQuery, ThingResponse>
{
    private readonly IThingRepositories _repo;

    public GetThingByIdQueryHandler(IThingRepositories repo)
    {
        _repo = repo;
    }

    public async Task<ThingResponse> HandleAsync(
        GetThingByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        Thing thing = await _repo.GetByIdAsync(query.Id, cancellationToken)
            ?? throw new NotFoundException($"Thing '{query.Id}' was not found.");

        return new ThingResponse
        {
            Id = thing.Id,
            Name = thing.Name,
        };
    }
}
```

---

## 10. Database, EF Core, migration, dan soft delete

- Provider database: PostgreSQL melalui `Npgsql.EntityFrameworkCore.PostgreSQL`.
- Jangan memakai `UseInMemoryDatabase` untuk `Support.Auth.Id` sebagai pengganti provider database utama.
- Terapkan query filter `HasQueryFilter(x => !x.IsDeleted)` hanya pada entity yang memang memiliki `IsDeleted` dan mengikuti soft-delete behavior.
- Unique index pada entity soft-delete perlu mempertimbangkan partial index/filter, misalnya `HasFilter("\"IsDeleted\" = false")`, sesuai kebutuhan unique constraint dan provider PostgreSQL.
- Jangan mengasumsikan semua tabel/entity harus menggunakan soft delete.
- Perubahan entity/configuration yang memengaruhi schema harus dilaporkan sebagai kebutuhan migration. Migration dibuat dan dijalankan oleh user.
- Jangan menjalankan `dotnet ef database update` tanpa izin eksplisit.
- Jangan mengedit `AppDbContextModelSnapshot.cs` manual.
- Jangan mengubah tabel atau schema Hangfire secara manual.
- Design-time context berada di `src/Repositories/Data/AppDbContextFactory.cs`. Jangan menjalankan web host hanya untuk membuat migration.

---

## 11. Security dan authorization

- Authentication menggunakan JWT Bearer.
- Role claim menggunakan `ClaimTypes.Role` sesuai konfigurasi existing.
- Policy yang tercatat saat ini: `Admin` dan `User`. Periksa `Program.cs`/registrasi authorization sebelum mengasumsikan konfigurasi masih sama.
- Endpoint admin menggunakan `[Authorize(Policy = "Admin")]` sesuai konvensi existing.
- Authorization saat ini menggunakan role-based policy. Permission-based authorization belum menjadi bagian implementasi yang tercatat dalam dokumen ini.
- Jika muncul kebutuhan untuk memperluas authorization (misalnya permission-based authorization), analisis model role, claims, policy, endpoint, kontrak, dan kebutuhan invalidasi terlebih dahulu. Jangan langsung menambahkan entity, attribute, atau policy baru tanpa desain.
- Ikuti komponen hashing, token generation, dan password policy yang sudah ada. Jangan membuat mekanisme paralel tanpa alasan dan persetujuan.
- Jangan menaruh secret (JWT key, password database, SMTP credential) di source code atau `appsettings` baru. Gunakan konfigurasi/secret mechanism yang sudah digunakan project.
- Jangan mencatat password, token, refresh token, atau password hash ke log.
- Refresh token saat ini disimpan plaintext. Ini adalah technical debt keamanan dan bukan pola yang boleh direplikasi. Perubahan penyimpanan/lifecycle token harus dianalisis sebagai security-sensitive change.

---

## 12. API, logging, dan error handling

- Ikuti pola response/API yang sudah digunakan pada endpoint sejenis.
- Sebelum membuat DTO response baru, periksa `Support.Auth.Client` dan `Support.Auth.Id` untuk tipe yang sudah ada. Jangan membuat tipe ketiga dengan bentuk yang sama.
- Perubahan public contract sebaiknya additive dan backward-compatible. Perubahan breaking harus dijelaskan dan disetujui.
- Gunakan status code dan exception mapping yang konsisten dengan `ExceptionMiddleware` serta endpoint existing.
- Jangan menambahkan logging yang memuat credential, token, hash, atau data sensitif.
- Jika menambah log, prioritaskan informasi operasional yang membantu diagnosis tanpa membocorkan data sensitif.
- Jangan menambah middleware, logging framework, tracing package, atau observability layer baru tanpa kebutuhan dan persetujuan.

---

## 13. Testing dan verifikasi

- Build bukan bukti bahwa behavior sudah benar; build hanya memverifikasi kompilasi.
- Periksa apakah test project dan test convention sudah tersedia sebelum menambahkan test.
- Jika test harness tersedia, tambahkan atau sesuaikan unit test untuk domain behavior dan handler sesuai scope.
- Integration test yang mengakses PostgreSQL atau service eksternal memerlukan izin eksplisit karena dapat memengaruhi data/environment.
- Jangan membuat test project, menambah framework, atau menambah dependency testing tanpa persetujuan.
- Jangan menjalankan `dotnet run`/`dotnet watch` tanpa izin karena startup dapat memicu migration database.
- Verifikasi yang diizinkan tanpa akses database: build, static analysis, dan code review.
- Jika runtime verification diperlukan, jelaskan perintah, target environment, kemungkinan side effect, dan minta izin sebelum menjalankannya.

---

## 14. Build

```bash
# Build project implementasi yang berubah
dotnet build Support.Auth.Id/Support.Auth.Id.csproj

# Build contract project jika DTO/interface publik berubah
dotnet build Support.Auth.Client/Support.Auth.Client.csproj
```

Build project lain yang berubah sesuai project terkait. Jangan menjalankan aplikasi hanya untuk memverifikasi startup.

---

## 15. Definition of Done

Sebuah perubahan dianggap selesai jika kriteria yang relevan sudah dipenuhi:

- [ ] Scope sesuai permintaan; tidak ada file di luar scope yang berubah tanpa alasan/persetujuan.
- [ ] Tidak ada akses database/runtime terlarang; akses khusus hanya dilakukan setelah izin eksplisit.
- [ ] Build project yang berubah berhasil tanpa warning baru yang tidak dijelaskan.
- [ ] Test yang relevan dijalankan jika test harness tersedia dan eksekusinya diizinkan.
- [ ] Tidak ada duplikasi DTO/tipe/fungsi yang tidak diperlukan.
- [ ] Penempatan file sesuai tanggung jawab layer dan struktur project.
- [ ] Domain tetap bersih dari DTO, HTTP, dan EF query.
- [ ] `Client` tidak mereferensikan `Id` atau implementasi internal.
- [ ] Handler/service baru terdaftar di DI jika diperlukan.
- [ ] Perubahan soft delete dan unique index konsisten untuk entity yang terdampak.
- [ ] Tidak ada penggunaan ulang Outbox/OutboxMessage; background processing mengikuti Hangfire yang sudah ada.
- [ ] Perubahan dilaporkan dengan file yang berubah, alasan, hasil verifikasi, migration yang perlu dibuat user, dan hal yang belum diverifikasi.

---

## 16. Kondisi dan technical debt yang diketahui

| `ARCHITECTURE.md` outdated | Isi tidak mencerminkan kondisi aktual | Anggap arsip. Jangan jadikan acuan. Jangan update kecuali diminta. Jika bertentangan dengan kode, kode yang benar. |

Daftar ini adalah catatan kondisi saat dokumen disusun, bukan roadmap atau daftar task otomatis. Periksa kode aktual sebelum menyentuh area terkait karena kondisi dapat berubah.

| Kondisi/utang teknis | Dampak atau catatan | Aturan sementara |
|---|---|---|
| `ApiResponse`, `PagedResponse`, `PaginationMetadata` ada di `Id` dan `Client` | Ada dua sumber tipe response/pagination | Endpoint publik baru gunakan contract yang sesuai di `Client`; jangan membuat versi ketiga. Jangan migrasi tipe existing kecuali memang scope task. |
| `AuthResponse`, `UserProfileResponse`, `ErrorResponse` masih berada di `Id` | Penempatan response publik belum konsisten | Jangan memindahkan secara oportunistik. Evaluasi hanya ketika task menyentuh contract terkait. |
| `RoleMappingExtention` berada di `Domain` | Mapping DTO berada di layer yang tidak tepat dan ada potensi duplikasi `IsBuiltIn` | Jangan menambah pemakaian. Perbaiki hanya ketika masuk scope. |
| `ModelStateExtention` berada di `Domain` | Web concern berada di layer yang tidak tepat | Jangan menambah web concern baru di `Domain`. |
| `RoleDependencyInjection` terpisah | Berbeda dari pola registrasi DI terpusat | Pertahankan selama tidak menghambat task; jangan menambah pola serupa tanpa alasan. |
| Refresh token disimpan plaintext | Technical debt keamanan | Jangan meniru pola ini pada mekanisme token baru. Hardening perlu scope dan analisis tersendiri. |
| JWT key/credential berada di konfigurasi project | Risiko pengelolaan secret | Jangan menambahkan secret baru ke file yang di-commit; ikuti mekanisme konfigurasi yang disetujui. |
| Outbox/`OutboxMessage` sudah tidak digunakan | Background processing telah menggunakan Hangfire | Jangan menggunakan atau mengaktifkan kembali Outbox. Sisa worker dapat dibersihkan dalam scope cleanup; jangan menghapus schema/migration database tanpa izin. |
| `Support.UI` masih scaffold | Belum terintegrasi dengan API | Jangan mengklaim UI sudah terhubung. |
| Belum ada test project yang tercatat | Coverage automated test terbatas | Periksa kondisi repository terbaru; jangan mengklaim tested jika hanya build dan jangan membuat test project tanpa persetujuan. |

---

## 17. Referensi

- [`ARCHITECTURE.md`](ARCHITECTURE.md) — outdated, sengaja tidak diupdate, bukan acuan utama. Dipertahankan sebagai arsip. Gunakan kode aktual sebagai sumber kebenaran. Jangan update kecuali diminta user.
- [`01-DATABASE-MODEL.md`](01-DATABASE-MODEL.md) — model database.
- [`02-COMPONENT-ARCHITECTURE.md`](02-COMPONENT-ARCHITECTURE.md) — komponen dan hubungan antarbagian.
- [`03-DATA-FLOW-SECURITY.md`](03-DATA-FLOW-SECURITY.md) — alur authentication, token, dan authorization.
- [`04-INFRASTRUCTURE-DEPLOYMENT.md`](04-INFRASTRUCTURE-DEPLOYMENT.md) — infrastruktur dan deployment.
- `Launcher/Doc/DDD_STATICPRIVATE.md` — ringkasan private constructor dan static factory.
- [`SHARED-ABSTRACTIONS.md`](SHARED-ABSTRACTIONS.md) — katalog base class, inherited capability, dan aturan pemeriksaan inheritance sebelum menambahkan method atau behavior.

Service Definition atau service-specific design document yang disediakan user juga menjadi referensi untuk penambahan service. Catat/tautkan dokumen tersebut di bagian ini atau pada katalog service jika sudah tersedia. Jika referensi tidak tersedia, berbeda dari implementasi, atau belum diperbarui, jangan mengarang isinya. Gunakan kode aktual sebagai bukti dan laporkan ketidaksesuaian dokumentasi.

---

## 18. Cara menjawab dan mengambil keputusan arsitektur

Saat menjelaskan atau mengubah project:

1. Sebutkan bahwa implementasi saat ini adalah **modular monolith** yang dipakai sebagai fase transisi sebelum kemungkinan perpindahan ke microservices.
2. Jangan menganggap migrasi microservices sebagai pekerjaan sekarang atau alasan untuk melakukan perubahan besar tanpa permintaan. Jika user meminta rencana migrasi, analisis secara khusus.
3. Sebutkan PostgreSQL, lightweight CQRS, dan DDD tactical sebagai fondasi teknis saat ini.
4. Bedakan implementasi aktual, keputusan teknis, proposal/ide, dan technical debt. Jangan mengubah ide menjadi target wajib.
5. Bedakan `Id` (implementasi) dan `Client` (kontrak).
6. Jangan mengklaim pola yang tidak ada di kode. Jika tidak yakin, baca file atau tanyakan.
7. Jangan menyarankan event bus, permission model, audit trail, atau pola arsitektur lain sebagai "perbaikan kecil". Jelaskan dampak teknisnya dan minta keputusan jika perubahan tersebut belum diminta.
8. Prioritaskan konsistensi dengan desain yang sudah dipilih daripada preferensi pribadi agent. Usulkan perubahan desain hanya jika ada alasan teknis yang dapat dijelaskan.
