# AltunPital

AltunPital, Sakarya Üniversitesi Bilgisayar Mühendisliği Yüksek Lisans **Nesneye Dayalı Programlama** dersi kapsamında geliştirilen çok şubeli bir **Hastane Yönetim ve Randevu Sistemi**dir.

Proje; hastane şubelerinin, bölümlerin, doktorların, sağlık personelinin, hizmetlerin, ekipmanların ve randevuların merkezi olarak yönetilmesini amaçlar.

## Özellikler

### Yönetici

- Şube oluşturma, düzenleme ve silme
- Şube bazlı bölüm yönetimi
- Doktor ve sağlık personeli yönetimi
- Doktorlara hizmet ve çalışma saatleri atama
- Sağlık hizmetlerinin süre, ücret ve aktiflik bilgilerinin yönetimi
- Şube ekipmanlarının yönetimi
- Randevuların görüntülenmesi
- Arama, filtreleme ve kolon bazlı sıralama
- Şube bazlı merkezi yönetim ekranı

### Hasta

- Yeni hasta kaydı
- Şehir ve şube seçimi
- Şube → Bölüm → Doktor → Hizmet şeklinde randevu oluşturma
- Doktor çalışma saatlerine göre uygun saatlerin görüntülenmesi
- Randevu çakışma kontrolü
- Randevu durumlarının görüntülenmesi
- Randevu iptali
- Profil bilgilerini güncelleme

### Doktor

- Kendi şube, bölüm, uzmanlık ve hizmet bilgilerini görüntüleme
- Çalışma programını görüntüleme
- Kendisine ait randevuları görüntüleme
- Randevu onaylama
- Randevu reddetme
- Randevuyu tamamlandı olarak işaretleme

### Sağlık Personeli

- Sisteme kullanıcı olarak giriş yapabilme
- Şube, bölüm ve pozisyon bilgilerinin görüntülenmesi
- Profil bilgilerinin yönetimi

## Kullanılan Teknolojiler

- C#
- .NET 9
- Windows Forms
- Entity Framework Core
- SQLite
- LINQ
- PBKDF2 tabanlı parola hashing

## Temel Mimari

Proje nesneye dayalı programlama prensipleri doğrultusunda geliştirilmiştir.

Başlıca yapılar:

- `User`
  - `Patient`
  - `Employee`
    - `Doctor`
    - `HealthStaff`
    - `Manager`
- `Hospital`
- `Branch`
- `Department`
- `HealthService`
- `Equipment`
- `Appointment`
- `DoctorSchedule`

Şube-hizmet, şube-ekipman ve doktor-hizmet ilişkileri ayrı ilişki tabloları üzerinden yönetilmektedir.

Kod yapısı genel olarak:

```text
AltunPital
├── Data
│   ├── AppDbContext
│   ├── Entities
│   ├── SeedData
│   └── Migrations
├── Interfaces
├── Services
├── UI
│   └── Desktop
├── Program.cs
└── AltunPital.csproj
```

---

# Kurulum Seçenekleri

Projeyi iki farklı şekilde çalıştırabilirsiniz.

## 1. Installer ile Kurulum

En kolay kullanım yöntemidir.

Hazırlanan:

```text
AltunPital_Setup.exe
```

dosyasını çalıştırın.

Kurulum sihirbazındaki adımları tamamladıktan sonra AltunPital bilgisayara kurulacaktır.

Kurulum sonunda masaüstü ve Başlat menüsünden uygulamaya erişilebilir.

Installer sürümü **self-contained** olarak hazırlandığı için hedef bilgisayarda ayrıca .NET 9 kurulması gerekmez.

İlk çalıştırmada SQLite veritabanı otomatik olarak oluşturulur ve başlangıç verileri sisteme eklenir.

---

## 2. Installer Olmadan Kaynak Koddan Çalıştırma

### Gereksinimler

- Windows
- .NET 9 SDK
- Visual Studio 2022 veya uyumlu bir IDE

Repo'yu klonlayın:

```bash
git clone <REPOSITORY_URL>
```

Proje klasörüne girin:

```bash
cd AltunPital
```

Paketleri geri yükleyin:

```bash
dotnet restore
```

Veritabanını oluşturun:

```bash
dotnet ef database update
```

Projeyi çalıştırın:

```bash
dotnet run
```

Alternatif olarak `AltunPital.sln` dosyası Visual Studio ile açılarak doğrudan çalıştırılabilir.

---

## Release Oluşturma

Windows x64 için bağımsız publish çıktısı oluşturmak için:

```bash
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=false -o ./publish
```

Oluşan `publish` klasörü installer kullanılmadan da başka bir Windows bilgisayara taşınabilir.

Bu durumda:

```text
publish/AltunPital.exe
```

çalıştırılması yeterlidir.

---

# Demo Kullanıcıları

### Yönetici

```text
E-posta: manager@altunpital.com
Şifre: asdf1234!
```

### Doktor

Örnek:

```text
E-posta: emre.karahan@altunpital.com
Şifre: doktor123
```

### Sağlık Personeli

Örnek:

```text
E-posta: selma.aydin@altunpital.com
Şifre: personel123
```

Demo hastaların varsayılan şifresi:

```text
hasta123
```

Yeni hasta hesabı uygulamanın kayıt ekranı üzerinden de oluşturulabilir.

---

# Veritabanı

Proje SQLite kullanmaktadır.

Veritabanı dosyası:

```text
altunpital.db
```

Entity Framework Core migrations ile veritabanı şeması oluşturulur.

Başlangıç verileri `SeedData` üzerinden yalnızca boş veritabanında oluşturulur.

Demo verileri farklı şehirlerde bulunan AltunPital şubelerini, doktorları, sağlık personelini, hizmetleri ve ekipmanları içerir.

---

# Randevu Akışı

Tipik hasta randevu süreci:

```text
Şube
↓
Bölüm
↓
Doktor
↓
Hizmet
↓
Tarih
↓
Uygun Saat
↓
Randevu Oluşturma
↓
Doktor Onayı
↓
Tamamlanma
```

Randevu oluşturulurken:

- doktorun ilgili şubede çalışması,
- doktorun seçilen hizmeti sunması,
- hizmetin şubede aktif olması,
- doktorun çalışma programı,
- mevcut randevu çakışmaları

kontrol edilir.

---

# Projenin Amacı

Bu proje özellikle şu nesneye dayalı programlama konularını uygulamalı olarak göstermek amacıyla geliştirilmiştir:

- Encapsulation
- Inheritance
- Polymorphism
- Interfaces
- Composition
- Entity relationships
- Service layer
- Role-based application flow

Proje eğitim amaçlı geliştirilmiştir ve gerçek bir sağlık kuruluşunda kullanılmak üzere tasarlanmış üretim sistemi değildir.
