# Rookie-MechkeyShop - Phase 1

## Link Weekly Report
-   Week 1: https://drive.google.com/drive/folders/10Xur1ME2guIPuDCG-lwsXr4_gEkThzpd?usp=sharing
-   Week 2: https://drive.google.com/file/d/1KgVIuf9x-N1KSwedu2jqpmB2Z-kp6yLd/view?usp=drive_link
-   Week 3: https://drive.google.com/file/d/1_XW82XyHNXeg1QbVN2FXHRxNaxT4m8sA/view

## System Architecture
<div>
  <img src="https://bsnnwuphmgsqfpvlqdwn.supabase.co/storage/v1/object/public/assets//MechKeyShop-Architecture.png" />
</div>

## Main Features
### CRUD
- ApplicationUser
- Category
- Product
- ProductRating
- Order
### Other
- **Caching**: Using Redis to cache query data.
- **Reverse Proxy**: Admin site communicates with the API via YARP.
- **Messaging**: Send asynchronous events (e.g. order created, product deleted) through RabbitMQ using MassTransit.
- **Cloud**: Upload product images to Supabase (Storage Bucket).
## Technologies Used 🧰

| Technology                   | Documentation |
|-----------------------------|----------------|
| [.NET 8](https://learn.microsoft.com/en-us/dotnet/) |  [docs](https://learn.microsoft.com/en-us/dotnet/) |
| [.NET Aspire](https://learn.microsoft.com/en-us/dotnet/aspire/) |  [docs](https://learn.microsoft.com/en-us/dotnet/aspire/) |
| [ASP.NET Core MVC](https://learn.microsoft.com/en-us/aspnet/core/mvc/)| [docs](https://learn.microsoft.com/en-us/aspnet/core/mvc/) |
| [React.js](https://reactjs.org/docs/getting-started.html) | [docs](https://reactjs.org/docs/getting-started.html) |
| [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/) | [docs](https://learn.microsoft.com/en-us/ef/core/) |
| [SQL Server](https://learn.microsoft.com/en-us/sql/sql-server/)  | [docs](https://learn.microsoft.com/en-us/sql/sql-server/) |
| [Redis](https://redis.io/docs/) | [docs](https://redis.io/docs/) |
| [RabbitMQ](https://www.rabbitmq.com/documentation.html) | [docs](https://www.rabbitmq.com/documentation.html) |
| [MassTransit](https://masstransit.io/documentation/)  | [docs](https://masstransit.io/documentation/) |
| [Supabase](https://supabase.com/docs) | [docs](https://supabase.com/docs) |
| [YARP (Yet Another Reverse Proxy)](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/reverse-proxy) | [docs](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/reverse-proxy) |
| [Clean Architecture](https://jasontaylor.dev/clean-architecture-getting-started/) | [blog](https://jasontaylor.dev/clean-architecture-getting-started/) |


