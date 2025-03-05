# Paket ve Ambalaj Ürün Yönetim Sistemi (PAUYS.Mvc)

**PAUYS.Mvc**, paket ve ambalaj ürünlerinin yönetimini kolaylaştırmak amacıyla geliştirilmiş bir ASP.NET MVC uygulamasıdır. Bu sistem, ürünlerin stok takibi, sipariş yönetimi ve kullanıcı yetkilendirme gibi işlevleri desteklemektedir.

## Özellikler

- **Ürün Yönetimi**: Ürün ekleme, güncelleme, silme ve listeleme işlemleri.
- **Stok Takibi**: Ürünlerin stok durumlarının izlenmesi ve güncellenmesi.
- **Sipariş Yönetimi**: Sipariş oluşturma, onaylama ve iptal etme işlemleri.
- **Kullanıcı Yetkilendirme**: Rol bazlı erişim kontrolü (Yönetici, Satış Temsilcisi, Depo Görevlisi).
- **Raporlama**: Satış ve stok raporlarının oluşturulması.

## Teknolojiler

- **ASP.NET MVC**
- **Entity Framework**
- **SQL Server**
- **Bootstrap**
- **jQuery**

## Kurulum

### Gereksinimler

- Visual Studio 2022 veya üzeri
- SQL Server 2019 veya üzeri
- .NET Framework 4.8

### Adımlar

1. **Depoyu Klonlayın**: Proje dosyalarını yerel makinenize indirin.

   ```bash
   git clone https://github.com/Minelka/PAUYS.Mvc.git
   ```

2. **Proje Dizini**: Proje dizinine gidin.

   ```bash
   cd PAUYS.Mvc
   ```

3. **Veritabanı Bağlantısı**: `Web.config` dosyasında veritabanı bağlantı dizesini kendi SQL Server ayarlarınıza göre güncelleyin.

   ```xml
   <connectionStrings>
       <add name="PAUYSContext" connectionString="Server=.;Database=PAUYSDB;Trusted_Connection=True;" providerName="System.Data.SqlClient" />
   </connectionStrings>
   ```

4. **Veritabanı Oluşturma**: Package Manager Console üzerinden aşağıdaki komutu çalıştırarak veritabanını oluşturun.

   ```powershell
   Update-Database
   ```

5. **Uygulamayı Çalıştırın**: Visual Studio'da projeyi başlatarak uygulamayı çalıştırın.

## Kullanım

Uygulama varsayılan olarak `http://localhost:5000` adresinde çalışacaktır. Ana sayfada ürün listesi görüntülenir. Üst menüden aşağıdaki sayfalara erişebilirsiniz:

- **Ürünler**: Ürün ekleme, güncelleme ve silme işlemleri.
- **Siparişler**: Yeni sipariş oluşturma ve mevcut siparişleri yönetme.
- **Raporlar**: Satış ve stok raporlarını görüntüleme.

## Proje Yapısı

```
PAUYS.Mvc
├── Controllers
│   ├── HomeController.cs
│   ├── ProductController.cs
│   └── OrderController.cs
├── Models
│   ├── Product.cs
│   ├── Order.cs
│   └── User.cs
├── Views
│   ├── Home
│   ├── Product
│   └── Order
├── Scripts
│   └── custom.js
└── Content
    ├── css
    └── images
```

- **Controllers**: Kullanıcı isteklerini işleyen denetleyiciler.
- **Models**: Veritabanı tablolarına karşılık gelen veri modelleri.
- **Views**: Kullanıcıya gösterilen arayüz sayfaları.
- **Scripts**: jQuery ve özel JavaScript dosyaları.
- **Content**: CSS ve görseller gibi statik dosyalar.

## Lisans

Bu proje açık kaynak değildir ve yalnızca izinli kullanıcılar tarafından kullanılabilir.
