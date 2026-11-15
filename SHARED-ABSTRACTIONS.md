# SHARED-ABSTRACTIONS.md — Katalog Shared Abstractions

> **Tujuan:** menjadi katalog awal untuk base class dan capability bersama agar agent tidak membuat ulang method, property, atau behavior yang sebenarnya sudah tersedia melalui inheritance.
>
> **Sumber kebenaran:** source code project. Dokumen ini membantu menemukan dan memahami abstraction, tetapi tidak menggantikan pemeriksaan implementasi aktual.
>
> **Aturan utama:** jangan menyimpulkan suatu capability tidak tersedia hanya karena method tersebut tidak ditulis langsung di concrete class.

---

## 1. Aturan penggunaan dokumen

Sebelum menambahkan method, property, helper, service, atau behavior baru, agent wajib:

1. Membuka class yang akan diubah dan memeriksa deklarasi `: BaseClass` atau interface yang diimplementasikan.
2. Menelusuri seluruh parent/base class sampai tidak ada lagi inheritance yang relevan.
3. Memeriksa interface langsung maupun interface yang diwariskan.
4. Memeriksa extension method yang tersedia melalui namespace yang digunakan.
5. Memeriksa static type pada call site, terutama jika dependency diinjeksi melalui interface.
6. Mencari penggunaan capability yang sama pada fitur/repository/entity lain.
7. Membandingkan hasil pemeriksaan dengan katalog ini.

Jika dokumen dan source code berbeda, **source code adalah acuan implementasi**. Laporkan ketidaksesuaian dokumen; jangan mengarang signature atau mengubah kode hanya agar cocok dengan dokumen.

## 2. BaseRepositories

### 2.1 Peran

`BaseRepositories` adalah base class untuk repository dan menyediakan capability persistence serta transaction bersama. Repository turunan tidak perlu membuat ulang implementasi capability yang sudah disediakan oleh base class.

### 2.2 Capability yang diketahui

Informasi berikut berasal dari penjelasan pemilik project. Nama method, signature, visibility, return type, dan dukungan `CancellationToken` harus selalu diverifikasi langsung pada source code.

| Capability | Tanggung jawab | Aturan penggunaan |
|---|---|---|
| SaveChanges | Menyimpan perubahan yang dilacak oleh persistence context | Gunakan implementasi inherited yang tersedia; jangan membuat implementasi SaveChanges baru di setiap repository. |
| Begin Transaction | Memulai transaction untuk operasi yang perlu atomicity | Gunakan mekanisme transaction existing dan pahami lifecycle-nya sebelum menambahkan transaction baru. |
| Commit Transaction | Melakukan commit transaction yang sedang aktif | Jangan membuat wrapper/implementasi commit kedua tanpa kebutuhan yang terbukti. |
| Rollback Transaction | Membatalkan transaction ketika operasi gagal | Ikuti mekanisme rollback dan disposal yang sudah digunakan base class. |

### 2.3 Aturan bagi repository turunan

- Jika repository mewarisi `BaseRepositories`, periksa method inherited sebelum menyatakan repository tidak memiliki `SaveChanges` atau transaction capability.
- Jangan menduplikasi SaveChanges, Begin, Commit, atau Rollback di setiap repository.
- Jangan menambahkan method repository yang hanya meneruskan pemanggilan ke method base class tanpa alasan boundary yang jelas.
- Repository tetap bertanggung jawab pada data access dan persistence; business behavior tetap berada pada domain entity, sedangkan use-case orchestration berada pada handler.
- Sebelum memakai transaction, periksa implementasi base class untuk mengetahui apakah transaction sudah dikelola, bagaimana commit/rollback dilakukan, dan apakah ada aturan nested transaction.
- Satu operasi tulis sederhana tidak otomatis membutuhkan transaction manual tambahan. Ikuti pola transaksi project yang sudah digunakan.

### 2.4 Catatan interface dan dependency injection

Ingat bahwa method yang tersedia pada base class concrete tidak otomatis menjadi anggota interface repository.

Contoh konseptual:

```csharp
public class AuthRepositories : BaseRepositories, IAuthRepositories
{
    // Method persistence dapat diwarisi dari BaseRepositories.
}
```

Jika consumer menggunakan:

```csharp
private readonly IAuthRepositories _repositories;
```

maka method yang dapat dipanggil melalui `_repositories` ditentukan oleh kontrak `IAuthRepositories` dan interface induknya, bukan semata-mata oleh base class concrete.

Karena itu:

- Periksa deklarasi `IAuthRepositories` dan semua interface induknya.
- Periksa tipe yang didaftarkan di DI.
- Periksa concrete implementation dan base class-nya.
- Jika method ada pada `BaseRepositories`, tetapi tidak tersedia dari interface consumer, jelaskan perbedaan tersebut sebelum mengusulkan perubahan.
- Jangan langsung menyalin implementasi ke `AuthRepositories`, membuat method baru dengan nama serupa, atau mengubah interface. Tentukan dulu apakah use case memang perlu mengakses capability tersebut dan bagaimana pola repository/unit-of-work existing mengeksposnya.

## 3. BaseModels

### 3.1 Peran

`BaseModels` adalah base class untuk entity/model domain yang menyediakan identity, timestamp, soft-delete state, dan behavior umum. Entity turunannya menggunakan capability tersebut daripada mendefinisikan ulang properti atau method yang sama.

### 3.2 Properti dan behavior yang diketahui

Informasi berikut mengikuti desain `BaseModels` yang telah dijelaskan dalam konteks project. Verifikasi tipe, nullability, default, dan implementasi aktual di source code sebelum mengubahnya.

| Anggota | Tanggung jawab | Aturan penggunaan |
|---|---|---|
| `Id` | Identity entity (`Guid`, dengan key mapping) | Jangan mendefinisikan identity kedua pada entity turunan. |
| `CreatedAt` | Waktu pembuatan; default saat ini `DateTime.UtcNow` | Jangan mengisi ulang secara manual pada setiap factory kecuali behavior khusus memang dibutuhkan. |
| `UpdateAt` | Timestamp pembaruan yang nullable | Gunakan behavior update yang tersedia; jangan membuat timestamp property duplikat. |
| `IsDeleted` | Menandai soft-delete state; default saat ini `false` | Jangan membuat flag soft delete kedua pada entity turunan. |
| `SetUpdate()` | Memperbarui timestamp update | Gunakan ketika entity mengubah state sesuai pola yang sudah berlaku. |
| `SetSoftDelete()` | Mengubah `IsDeleted` menjadi `true` dan memperbarui timestamp | Gunakan behavior inherited ini untuk soft delete; jangan membuat ulang method yang sama di repository atau entity turunan. |

### 3.3 Aturan soft delete

Soft delete adalah perubahan state entity melalui behavior `BaseModels` (`SetSoftDelete()`), sedangkan repository/persistence bertanggung jawab menyimpan perubahan state tersebut.

Pola konseptual:

```csharp
// Handler memuat entity melalui repository.
// Entity menggunakan behavior soft delete yang diwarisi dari BaseModels.
entity.SetSoftDelete();

// Perubahan state kemudian disimpan melalui persistence capability existing.
await repositories.SaveChangesAsync(cancellationToken);
```

> Contoh di atas bersifat konseptual. Pastikan nama variabel, method repository, dan signature yang sebenarnya sesuai dengan source code. Jangan menganggap setiap interface repository mengekspos `SaveChangesAsync` jika belum diperiksa.

Aturan:

- Jangan menambahkan `SoftDelete()` ke repository hanya untuk mengulang `SetSoftDelete()` yang sudah dimiliki entity/base model.
- Jangan mendefinisikan ulang `IsDeleted`, `UpdateAt`, atau `SetSoftDelete()` pada setiap entity turunan.
- Jangan melakukan hard delete sebagai pengganti soft delete kecuali user secara eksplisit meminta dan desain mengizinkannya.
- Global query filter dan partial unique index adalah concern EF Core configuration; jangan memindahkannya ke domain entity.
- Periksa apakah entity benar-benar mewarisi `BaseModels` sebelum menggunakan aturan ini. Jangan mengasumsikan semua entity atau semua tabel memiliki soft delete.

## 4. Cara membedakan tanggung jawab

| Tanggung jawab | Lokasi yang diutamakan |
|---|---|
| Menjaga invariant dan mengubah state entity, termasuk soft delete | Domain entity / base model behavior yang diwarisi |
| Mengambil dan menyimpan data | Repository / persistence abstraction |
| SaveChanges dan transaction lifecycle yang digunakan bersama | `BaseRepositories` atau abstraction persistence existing |
| Mengatur urutan use case dan memanggil domain/repository | CQRS handler |
| Mengekspos capability repository kepada consumer | Repository interface / unit-of-work contract yang memang digunakan project |

Jangan memindahkan behavior domain ke repository hanya karena repository menjadi tempat penyimpanan data. Jangan pula membuat application handler mengulang implementasi persistence yang telah tersedia.

## 5. Checklist inheritance sebelum membuat method baru

Agent wajib menjawab pertanyaan ini sebelum menyimpulkan capability belum tersedia:

- [ ] Apa concrete class yang sedang digunakan?
- [ ] Apa base class langsungnya?
- [ ] Apakah ada base class lain di atasnya?
- [ ] Interface apa yang diimplementasikan, termasuk interface induk?
- [ ] Apakah method yang dicari diwarisi dari base class?
- [ ] Apakah capability disediakan oleh extension method?
- [ ] Apa static type pada call site atau constructor injection?
- [ ] Apakah capability terlihat melalui interface yang digunakan consumer?
- [ ] Apakah ada implementasi serupa pada base class, shared service, entity, atau repository lain?
- [ ] Jika capability memang tidak dapat diakses, apakah masalahnya ketiadaan implementasi atau hanya ketiadaan exposure melalui interface?

Jika jawaban belum jelas, lakukan inspeksi source code terlebih dahulu. Jangan membuat method pengganti sebagai jalan pintas.

## 6. Aturan ketika menambah atau mengubah shared abstraction

- Jangan menambah method ke base class hanya untuk menghindari satu atau dua baris kode pada satu fitur.
- Perubahan pada base class dapat berdampak ke semua class turunannya. Sebelum mengubahnya, cari seluruh consumer/derived class dan analisis dampaknya.
- Jangan menambahkan business-specific behavior ke `BaseRepositories` atau `BaseModels`.
- `BaseRepositories` hanya menampung capability persistence/transaction yang memang reusable pada repository turunannya.
- `BaseModels` hanya menampung properti atau behavior entity umum yang memang berlaku lintas entity.
- Jika kebutuhan hanya berlaku pada satu aggregate/entity, letakkan behavior pada entity tersebut, bukan pada base class.
- Jika perubahan base class mengubah persistence, transaction, atau lifecycle entity, sertakan impact analysis dan test/build yang relevan.
- Perbarui dokumen ini ketika shared abstraction berubah, tetapi jangan menjadikan dokumentasi sebagai alasan untuk menambah abstraction yang tidak dibutuhkan.

## 7. Informasi yang perlu dilengkapi dari source code

Bagian berikut sengaja tidak mengarang lokasi atau signature. Lengkapi berdasarkan source code repository saat dokumen ini ditempatkan:

| Abstraction | File path aktual | Detail yang perlu dicatat |
|---|---|---|
| `BaseRepositories` | Perlu diverifikasi di repository | Namespace, base class/interface, daftar method persis, signature, visibility, transaction disposal/lifecycle. |
| `BaseModels` | Perlu diverifikasi di repository | Namespace, properti persis, tipe/nullability, constructor, default value, implementasi `SetUpdate()` dan `SetSoftDelete()`. |
| `IAuthRepositories` | Perlu diverifikasi di repository | Base interface, method yang diekspos, dan apakah shared persistence capability dapat diakses melalui interface ini. |

Jangan mengisi tabel ini berdasarkan tebakan. Gunakan deklarasi dan implementasi source code sebagai referensi.
