# Ringkasan Private Constructor & Static Factory Method (DDD / Rich Domain Model)

## 1. Class BUKAN Object

```csharp
public class User
{
}
```

`User` hanya blueprint / cetakan.

Object baru ada ketika:

```csharp
User user = new User();
```

---

# 2. Instance Method Butuh Object

Contoh:

```csharp
public void Login()
{
}
```

Cara pakai:

```csharp
user.Login();
```

Artinya:

- method biasa (`non-static`)
- membutuhkan object terlebih dahulu

---

# 3. Private Constructor

Contoh:

```csharp
private User()
{
}
```

Tujuan:

- mencegah object dibuat sembarangan
- memaksa creation lewat aturan domain

Maka ini ERROR:

```csharp
new User();
```

---

# 4. Kenapa Butuh Static Factory Method?

Karena object belum ada.

Kalau method biasa:

```csharp
public User Register()
{
}
```

maka harus:

```csharp
user.Register();
```

Padahal:

- object `user` belum ada
- constructor private

Jadi tidak mungkin.

---

# 5. Solusinya = Static Method

Contoh:

```csharp
public static User Register()
{
    return new User();
}
```

Cara pakai:

```csharp
User user = User.Register();
```

Karena:

- `static` milik class
- tidak butuh object

---

# 6. Perbedaan Utama

## Constructor Private

Mengontrol:

```text
cara object dibuat
```

---

## Private Setter

Mengontrol:

```text
cara data diubah
```

Contoh:

```csharp
public string Email { get; private set; }
```

Artinya:

- tidak bisa diubah sembarangan dari luar class

---

# 7. Tujuan Utama DDD Entity

Entity harus:

- menjaga validitas dirinya sendiri
- tidak boleh berada di state invalid
- semua perubahan lewat business behavior

---

# 8. Contoh Rich Domain Entity

```csharp
User.Register()
user.AssignRole()
user.ChangePassword()
user.ValidateLogin()
```

Business logic ada di entity,
bukan tercecer di service/controller.

---

# 9. Analogi Sederhana

## Public Constructor

```text
siapa saja boleh bikin object
```

---

## Private Constructor + Static Factory

```text
object hanya bisa dibuat lewat jalur resmi/domain rule
```

---

# 10. Inti Konsep

```text
private constructor
    ↓
mencegah new object sembarangan
    ↓
butuh factory method
    ↓
factory dipanggil dari class
    ↓
maka method harus static
```
