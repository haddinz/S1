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