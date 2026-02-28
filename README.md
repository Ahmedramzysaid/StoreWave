<div align="center">

# 🌊 StoreWave

### *Enterprise E-Commerce Platform*

[![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-9.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![SQL Server](https://img.shields.io/badge/SQL%20Server-2022-CC2927?style=for-the-badge&logo=microsoftsqlserver&logoColor=white)](https://www.microsoft.com/sql-server)
[![Bootstrap](https://img.shields.io/badge/Bootstrap-5.3-7952B3?style=for-the-badge&logo=bootstrap&logoColor=white)](https://getbootstrap.com/)
[![SignalR](https://img.shields.io/badge/SignalR-Real--Time-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/apps/aspnet/signalr)
[![PayPal](https://img.shields.io/badge/PayPal-Integrated-00457C?style=for-the-badge&logo=paypal&logoColor=white)](https://developer.paypal.com/)
[![License](https://img.shields.io/badge/License-MIT-green?style=for-the-badge)](LICENSE)

**A full-featured, role-based e-commerce platform built with ASP.NET Core MVC, featuring real-time notifications, PayPal payments, email automation, and a multi-role dashboard system.**

[Live Demo](https://shopmartcommerce.runasp.net) · [Report Bug](#) · [Request Feature](#)

</div>

---

## 📋 Table of Contents

- [System Overview](#-system-overview)
- [Role-Based Features](#-role-based-features)
- [System Architecture](#-system-architecture)
- [Backend Architecture & Components](#-backend-architecture--components)
- [Data Flow in Backend](#-data-flow-in-backend)
- [UML Class Diagram](#-uml-class-diagram)
- [Project Package Structure](#-project-package-structure)
- [Tools & Technologies](#-tools--technologies)
- [Hosting & Deployment](#-hosting--deployment-monsterasp)

---

## 🔭 System Overview

**StoreWave** is a comprehensive, enterprise-grade e-commerce platform built using the **ASP.NET Core 9.0 MVC** architecture. It provides a complete online shopping experience with a sophisticated multi-role management system.

### Key Highlights

| Feature | Description |
|---------|-------------|
| 🛒 **Full Shopping Experience** | Product browsing, search, filtering, cart management, and checkout |
| 💳 **PayPal Integration** | Secure online payments via PayPal REST API |
| 📧 **Email Automation** | OTP verification, order confirmations, role-based notifications via SMTP |
| 🔔 **Real-Time Updates** | Live order tracking, cart sync, and chat via SignalR WebSockets |
| 👥 **5-Role System** | Admin, Customer, Supplier, Accountant, Warehouse Manager |
| 🗺️ **Location Services** | GPS-based address detection via Nominatim/OpenStreetMap |
| 🔒 **Security** | ASP.NET Identity with role-based authorization, anti-forgery tokens |
| 📊 **Analytics Dashboards** | Revenue reports, sales analytics, payment method statistics |

---

## 👥 Role-Based Features

StoreWave implements a granular **Role-Based Access Control (RBAC)** system with 5 distinct roles, each with dedicated dashboards and functionality:

### 🔑 Admin — *Full System Control*

| Feature | Description |
|---------|-------------|
| 📊 Dashboard | Total revenue, orders, products, customers at a glance |
| 👤 User Management | View all users, assign roles, activate/deactivate accounts |
| 📦 Order Management | View all orders, update status (Pending → Processing → Shipped → Delivered) |
| 🛍️ Product Management | Full CRUD for all products across all suppliers |
| 📂 Category Management | Create, edit, delete product categories |
| 💬 Order Chat | Real-time chat support with customers on specific orders |
| ⭐ Review Management | Monitor and manage product reviews |

### 🛒 Customer — *Shopping Experience*

| Feature | Description |
|---------|-------------|
| 🏠 Home Page | Featured products, categories, search functionality |
| 🔍 Product Browsing | Filter by category, search by name, view product details |
| 🛒 Shopping Cart | Add/remove items, update quantities, real-time cart sync |
| 💳 Checkout | Cash on Delivery or PayPal payment options |
| 📦 Order Tracking | View order history, track status, order details |
| 💬 Order Chat | Real-time chat with admin/support for specific orders |
| ⭐ Product Reviews | Rate and review purchased products |
| 👤 Profile Management | Edit personal info, shipping address, GPS location |

### 📦 Supplier — *Product Supply Management*

| Feature | Description |
|---------|-------------|
| 📊 Dashboard | Total products, active listings, out-of-stock alerts |
| 🛍️ Product Management | Create, edit, delete own products with image upload |
| 📋 Order Visibility | View orders containing their supplied products |
| 📸 Image Management | Upload and manage product images |

### 💰 Accountant — *Financial Analytics*

| Feature | Description |
|---------|-------------|
| 📊 Financial Dashboard | Total revenue, order count, average order value, today's/monthly sales |
| 📈 Sales Reports | Date-range filtered sales analysis |
| 💵 Revenue Analytics | Monthly revenue trends over 12 months |
| 💳 Payment Methods | Payment method distribution and statistics |
| 📋 Order Details | Drill-down into individual order financials |

### 🏭 Warehouse Manager — *Inventory Control*

| Feature | Description |
|---------|-------------|
| 📊 Dashboard | Total products, low stock alerts, out-of-stock count, total inventory |
| 📦 Inventory Management | Full inventory listing sorted by stock level |
| ✏️ Stock Updates | Update stock quantities for individual products |
| ⚠️ Low Stock Alerts | Products with stock ≤ 10 flagged for restocking |

---

## 🏗️ System Architecture

StoreWave follows a classic **3-Tier Architecture** separating the Presentation, Business Logic, and Data layers:

### Architecture Tiers

```mermaid
graph TB
    subgraph "🖥️ Presentation Layer"
        A[Razor Views - .cshtml]
        B[HTML5 / CSS3 / JavaScript]
        C[Bootstrap 5 + jQuery]
        D[SignalR Client]
    end

    subgraph "⚙️ Business Logic Layer"
        E[Controllers]
        F[Services Layer]
        G[FluentValidation]
        H[AutoMapper]
        I[SignalR Hubs]
        J[Identity Auth]
    end

    subgraph "💾 Data Access Layer"
        K[Repository Pattern]
        L[Unit of Work]
        M[Entity Framework Core]
        N[(SQL Server Database)]
    end

    subgraph "🌐 External Services"
        O[PayPal REST API]
        P[Gmail SMTP]
        Q[Nominatim Geocoding]
    end

    A --> E
    E --> F
    F --> G
    F --> H
    F --> K
    K --> L
    L --> M
    M --> N
    F --> I
    F --> O
    F --> P
    E --> J
    A --> D
    D --> I
    A --> Q
```

---

## 🔧 Backend Architecture & Components

The backend follows **Clean Architecture** principles with clear separation of concerns:

```mermaid
graph LR
    subgraph "Controllers Layer"
        C1[AccountController]
        C2[AdminController]
        C3[AccountantController]
        C4[CartController]
        C5[ProductsController]
        C6[PaymentController]
        C7[SupplierController]
        C8[WarehouseController]
        C9[OrdersController]
        C10[CategoriesController]
        C11[ReviewsController]
        C12[HomeController]
    end

    subgraph "Service Layer"
        S1[ProductService]
        S2[OrderService]
        S3[CartService]
        S4[CustomerService]
        S5[CategoryService]
        S6[ReviewService]
        S7[EmailService]
        S8[EmailTemplateService]
        S9[FileService]
        S10[PayPalService]
        S11[ChatService]
    end

    subgraph "SignalR Hubs"
        H1[OrderHub]
        H2[NotificationHub]
        H3[CartHub]
        H4[ChatHub]
    end

    subgraph "Data Layer"
        R1[Repository Pattern]
        U1[Unit of Work]
        EF[Entity Framework Core]
        DB[(SQL Server)]
    end

    subgraph "External APIs"
        PP[PayPal REST API]
        EM[Gmail SMTP Server]
        GEO[Nominatim API]
    end

    C1 --> S4
    C1 --> S7
    C2 --> S2
    C2 --> S1
    C2 --> S11
    C3 --> S2
    C4 --> S3
    C4 --> S7
    C5 --> S1
    C5 --> S5
    C6 --> S10
    C7 --> S1
    C8 --> S1

    S1 --> R1
    S2 --> R1
    S3 --> R1
    S4 --> R1
    S5 --> R1
    S6 --> R1
    S11 --> R1

    R1 --> U1
    U1 --> EF
    EF --> DB

    S10 --> PP
    S7 --> EM
    S8 --> S7

    C4 --> H1
    C4 --> H2
    C4 --> H3
    C2 --> H4
```

### Component Responsibilities

| Component | Role | Pattern |
|-----------|------|---------|
| **Controllers** | Handle HTTP requests, route to services | MVC Controller |
| **Services** | Business logic, validation, orchestration | Service Layer |
| **Repositories** | Data access abstraction | Repository Pattern |
| **Unit of Work** | Transaction management | Unit of Work Pattern |
| **DTOs** | Data transfer between layers | Data Transfer Object |
| **ViewModels** | View-specific data models | MVVM-inspired |
| **Validators** | Input validation rules | FluentValidation |
| **Mappings** | Entity ↔ DTO conversion | AutoMapper Profile |
| **Hubs** | Real-time bidirectional communication | SignalR Hub |

---

## 🔄 Data Flow in Backend

This diagram illustrates how data flows through the StoreWave backend when processing a request:

### Request Processing Pipeline

```mermaid
sequenceDiagram
    participant Browser
    participant Middleware
    participant Controller
    participant Service
    participant Validator
    participant UoW as Unit of Work
    participant Repo as Repository
    participant EF as EF Core
    participant DB as SQL Server
    participant Mapper as AutoMapper
    participant Hub as SignalR Hub
    participant Email as Email Service

    Browser->>Middleware: HTTP Request
    Middleware->>Middleware: Authentication & Session
    Middleware->>Controller: Route to Action

    Controller->>Service: Call Business Method
    Service->>Validator: Validate Input (FluentValidation)
    Validator-->>Service: Validation Result

    Service->>UoW: Begin Transaction
    UoW->>Repo: Data Operation
    Repo->>EF: LINQ Query
    EF->>DB: SQL Command
    DB-->>EF: Result Set
    EF-->>Repo: Entity Objects
    Repo-->>UoW: Complete
    UoW-->>Service: Commit Transaction

    Service->>Mapper: Entity → DTO
    Mapper-->>Service: Mapped DTO

    Service->>Hub: Push Real-Time Update
    Service->>Email: Send Notification

    Service-->>Controller: Return DTO
    Controller->>Controller: Render Razor View
    Controller-->>Browser: HTML Response

    Hub-->>Browser: WebSocket Push
```

### Checkout Flow (PayPal)

```mermaid
sequenceDiagram
    participant Customer
    participant CartController
    participant PaymentController
    participant CartService
    participant PayPalService
    participant OrderService
    participant PayPal as PayPal API
    participant Email as Email Service

    Customer->>CartController: POST /Cart/Checkout
    CartController->>CartService: GetCartAsync()
    CartService-->>CartController: Cart Items

    Customer->>PaymentController: POST /Payment/CreatePayPalOrder
    PaymentController->>PayPalService: CreateOrderAsync()
    PayPalService->>PayPal: Create Order API
    PayPal-->>PayPalService: Approval URL
    PayPalService-->>PaymentController: Redirect URL
    PaymentController-->>Customer: Redirect to PayPal

    Customer->>PayPal: Approve Payment
    PayPal-->>Customer: Redirect Back

    Customer->>PaymentController: GET /Payment/PayPalSuccess
    PaymentController->>PayPalService: CaptureOrderAsync()
    PayPalService->>PayPal: Capture Payment
    PayPal-->>PayPalService: Payment Confirmed

    PaymentController->>OrderService: CreateOrderAsync()
    PaymentController->>CartService: ClearCartAsync()
    PaymentController->>Email: Send Order Confirmation
    PaymentController-->>Customer: Order Success Page
```

---

## 📐 UML Class Diagram

### Core Entity Relationships

```mermaid
classDiagram
    class Customer {
        +int Id
        +string FirstName
        +string LastName
        +string Email
        +string Address
        +string City
        +string Country
        +string PostalCode
        +double Latitude
        +double Longitude
        +DateTime CreatedAt
        +bool IsActive
        +string FullName
        +ICollection~Order~ Orders
        +ICollection~Review~ Reviews
        +ICollection~CartItem~ CartItems
        +ICollection~Product~ SupplierProducts
    }

    class Product {
        +int Id
        +string Name
        +string Description
        +decimal Price
        +decimal DiscountPrice
        +int StockQuantity
        +string ImageUrl
        +bool IsActive
        +bool IsFeatured
        +DateTime CreatedAt
        +int CategoryId
        +int SupplierId
        +decimal CurrentPrice
        +bool IsOnSale
        +int DiscountPercentage
    }

    class Order {
        +int Id
        +string OrderNumber
        +DateTime OrderDate
        +decimal SubTotal
        +decimal ShippingCost
        +decimal TotalAmount
        +OrderStatus Status
        +string ShippingAddress
        +PaymentMethod PaymentMethod
        +string Notes
        +int CustomerId
        +ICollection~OrderItem~ OrderItems
        +ICollection~ChatMessage~ ChatMessages
        +GenerateOrderNumber()
    }

    class OrderItem {
        +int Id
        +int OrderId
        +int ProductId
        +string ProductName
        +decimal UnitPrice
        +int Quantity
        +decimal TotalPrice
    }

    class CartItem {
        +int Id
        +int CustomerId
        +int ProductId
        +int Quantity
        +DateTime AddedAt
    }

    class Category {
        +int Id
        +string Name
        +string Description
        +string ImageUrl
        +bool IsActive
        +ICollection~Product~ Products
    }

    class Review {
        +int Id
        +int ProductId
        +int CustomerId
        +int Rating
        +string Comment
        +DateTime CreatedAt
    }

    class ChatMessage {
        +int Id
        +int OrderId
        +int SenderId
        +string Message
        +DateTime SentAt
        +bool IsRead
    }

    class OrderStatus {
        <<enumeration>>
        Pending
        Processing
        Shipped
        Delivered
        Cancelled
    }

    class PaymentMethod {
        <<enumeration>>
        CashOnDelivery
        PayPal
    }

    Customer "1" --> "*" Order : places
    Customer "1" --> "*" Review : writes
    Customer "1" --> "*" CartItem : has
    Customer "1" --> "*" Product : supplies
    Order "1" --> "*" OrderItem : contains
    Order "1" --> "*" ChatMessage : has
    Order --> OrderStatus : status
    Order --> PaymentMethod : payment
    Product "1" --> "*" OrderItem : ordered in
    Product "1" --> "*" Review : reviewed by
    Product "1" --> "*" CartItem : added to
    Category "1" --> "*" Product : categorizes
    ChatMessage --> Customer : sent by
    ChatMessage --> Order : belongs to
```

### Service Layer Interfaces

```mermaid
classDiagram
    class IProductService {
        <<interface>>
        +GetAllProductsAsync()
        +GetProductByIdAsync(id)
        +GetProductsByCategoryAsync(categoryId)
        +SearchProductsAsync(searchTerm)
        +CreateProductAsync(productDto)
        +UpdateProductAsync(productDto)
        +DeleteProductAsync(id)
    }

    class IOrderService {
        <<interface>>
        +CreateOrderAsync(userId, orderDto)
        +GetOrderByIdAsync(id)
        +GetOrdersByCustomerAsync(customerId)
        +GetRecentOrdersAsync()
        +UpdateOrderStatusAsync(id, status)
        +GetTotalSalesAsync()
        +GetTotalOrdersAsync()
    }

    class ICartService {
        <<interface>>
        +GetCartAsync(userId)
        +AddToCartAsync(userId, productId, qty)
        +RemoveFromCartAsync(userId, productId)
        +UpdateQuantityAsync(userId, productId, qty)
        +ClearCartAsync(userId)
        +GetCartCountAsync(userId)
    }

    class IEmailService {
        <<interface>>
        +SendEmailAsync(to, subject, body)
    }

    class IPayPalService {
        <<interface>>
        +CreateOrderAsync(request)
        +CaptureOrderAsync(orderId)
    }

    class IChatService {
        <<interface>>
        +GetMessagesAsync(orderId)
        +SendMessageAsync(message)
        +GetOrderChatsAsync()
        +MarkAsReadAsync(orderId, userId)
    }

    class IUnitOfWork {
        <<interface>>
        +Products: IProductRepository
        +Categories: ICategoryRepository
        +Orders: IOrderRepository
        +Customers: ICustomerRepository
        +CartItems: ICartItemRepository
        +Reviews: IReviewRepository
        +SaveChangesAsync()
    }
```

---

## 📁 Project Package Structure

```
📁 StoreWave/
│
├── 📁 Controllers/              # 12 MVC Controllers
│   ├── AccountController.cs         # Auth: Login, Register, OTP, Password Reset
│   ├── AdminController.cs           # Admin Dashboard, Users, Orders, Chat
│   ├── AccountantController.cs      # Financial Reports, Revenue, Sales
│   ├── CartController.cs            # Shopping Cart, Checkout
│   ├── CategoriesController.cs      # Category CRUD
│   ├── HomeController.cs            # Home Page, Privacy
│   ├── OrdersController.cs          # Customer Order History
│   ├── PaymentController.cs         # PayPal Payment Flow
│   ├── ProductsController.cs        # Product CRUD & Browsing
│   ├── ReviewsController.cs         # Product Reviews
│   ├── SupplierController.cs        # Supplier Product Management
│   └── WarehouseController.cs       # Inventory & Stock Management
│
├── 📁 Models/
│   ├── 📁 Entities/             # 8 Domain Entities
│   │   ├── Customer.cs              # User (extends IdentityUser<int>)
│   │   ├── Product.cs               # Product with pricing & stock
│   │   ├── Order.cs                 # Order with status tracking
│   │   ├── OrderItem.cs             # Line items in orders
│   │   ├── CartItem.cs              # Shopping cart entries
│   │   ├── Category.cs              # Product categories
│   │   ├── Review.cs                # Product reviews & ratings
│   │   └── ChatMessage.cs           # Real-time order chat messages
│   ├── 📁 Enums/
│   │   ├── OrderStatus.cs           # Pending → Processing → Shipped → Delivered
│   │   └── PaymentMethod.cs         # CashOnDelivery, PayPal
│   └── ErrorViewModel.cs
│
├── 📁 Views/                    # 51 Razor Views (.cshtml)
│   ├── 📁 Account/   (7)           # Login, Register, Profile, OTP, Password
│   ├── 📁 Admin/     (7)           # Dashboard, Users, Orders, Chat
│   ├── 📁 Accountant/ (5)          # Financial Reports, Revenue
│   ├── 📁 Cart/      (3)           # Cart, Checkout, Order Success
│   ├── 📁 Categories/ (5)          # CRUD Views
│   ├── 📁 Home/      (2)           # Index, Privacy
│   ├── 📁 Orders/    (2)           # Order History, Details
│   ├── 📁 Products/  (5)           # Catalog, CRUD Views
│   ├── 📁 Supplier/  (5)           # Dashboard, Product Management
│   ├── 📁 Warehouse/ (4)           # Inventory, Stock Updates
│   └── 📁 Shared/    (4)           # Layout, Login Partial, Error
│
├── 📁 Services/
│   ├── 📁 Interfaces/          # 11 Service Contracts
│   └── 📁 Implementations/    # 11 Service Implementations
│
├── 📁 Repositories/
│   ├── 📁 Interfaces/          # 7 Repository Contracts
│   └── 📁 Implementations/    # 7 Repository Implementations
│
├── 📁 DTOs/                     # 7 Data Transfer Objects
├── 📁 ViewModels/               # 12 View Models
├── 📁 Validators/               # 4 FluentValidation Rules
├── 📁 Hubs/                     # 4 SignalR Hubs
├── 📁 Data/                     # DbContext & Seeder
├── 📁 Mappings/                 # AutoMapper Profile
├── 📁 UnitOfWork/               # Unit of Work Pattern
├── 📁 Migrations/               # EF Core Migrations
├── 📁 wwwroot/                  # Static Files (CSS, JS, Images)
│
├── 📄 Program.cs                # Application Entry Point & DI Config
├── 📄 appsettings.json          # Configuration
├── 📄 StoreWave.csproj          # Project File
└── 📄 StoreWave.sln             # Solution File
```

---

## 🛠️ Tools & Technologies

### Backend Stack

| Technology | Version | Purpose |
|------------|---------|---------|
| **ASP.NET Core MVC** | 9.0 | Web framework & MVC pattern |
| **Entity Framework Core** | 9.0 | ORM & database migrations |
| **ASP.NET Identity** | 9.0 | Authentication & authorization |
| **SignalR** | 9.0 | Real-time WebSocket communication |
| **FluentValidation** | 11.3 | Server-side input validation |
| **AutoMapper** | 12.0 | Object-to-object mapping |
| **C#** | 13 | Primary programming language |

### Frontend Stack

| Technology | Version | Purpose |
|------------|---------|---------|
| **Razor Views** | - | Server-side HTML templating |
| **Bootstrap** | 5.3 | Responsive UI framework |
| **jQuery** | 3.7 | DOM manipulation & AJAX |
| **JavaScript** | ES6+ | Client-side interactivity |
| **CSS3** | - | Custom styling & animations |
| **Font Awesome** | 6.x | Icon library |
| **Leaflet.js** | - | Interactive maps |

### Database & Storage

| Technology | Version | Purpose |
|------------|---------|---------|
| **SQL Server** | 2022 | Relational database |
| **EF Core Migrations** | - | Database schema versioning |
| **Redis** *(optional)* | - | Distributed caching |

### External Services & APIs

| Service | Purpose |
|---------|---------|
| **PayPal REST API** | Online payment processing |
| **Gmail SMTP** | Transactional email delivery |
| **Nominatim API** | Reverse geocoding & address lookup |
| **OpenStreetMap** | Interactive location maps |

### Development Tools

| Tool | Purpose |
|------|---------|
| **Visual Studio 2022** | Primary IDE |
| **Git & GitHub** | Version control |
| **Postman** | API testing |
| **SQL Server Management Studio** | Database management |
| **NuGet** | Package management |

---

## 🌐 Hosting & Deployment (MonsterASP)

StoreWave is deployed on **MonsterASP.NET** hosting platform:

### Deployment Architecture

```mermaid
graph TB
    subgraph "👨‍💻 Developer"
        DEV[Visual Studio 2022]
        GIT[GitHub Repository]
    end

    subgraph "☁️ MonsterASP Hosting"
        IIS["IIS Web Server"]
        APP["StoreWave Application<br/>ASP.NET Core 9.0"]
        SSL["SSL Certificate<br/>HTTPS Enabled"]
        MSSQL["SQL Server Database<br/>StoreWave DB"]
    end

    subgraph "🌍 External Services"
        PP[PayPal API]
        SMTP[Gmail SMTP Server]
        NOM[Nominatim Geocoding]
    end

    subgraph "👥 Users"
        ADMIN[Admin]
        CUST[Customers]
        SUPP[Suppliers]
        ACCT[Accountant]
        WH[Warehouse Manager]
    end

    DEV -->|"Web Deploy"| IIS
    DEV -->|"Push"| GIT

    IIS --> APP
    APP --> SSL
    APP <--> MSSQL

    APP <-->|HTTPS| PP
    APP -->|SMTP| SMTP
    APP -->|HTTP| NOM

    ADMIN -->|HTTPS| SSL
    CUST -->|HTTPS| SSL
    SUPP -->|HTTPS| SSL
    ACCT -->|HTTPS| SSL
    WH -->|HTTPS| SSL
```

### Hosting Configuration

| Parameter | Value |
|-----------|-------|
| **Provider** | MonsterASP.NET |
| **Runtime** | ASP.NET Core 9.0 |
| **Web Server** | IIS (Internet Information Services) |
| **Database** | SQL Server on MonsterASP |
| **Domain** | `shopmartcommerce.runasp.net` |
| **Protocol** | HTTPS with SSL |
| **Deployment** | Web Deploy from Visual Studio |

### Deployment Steps

1. **Build** the project in Release mode:
   ```bash
   dotnet publish -c Release
   ```
2. **Configure** the production connection string in `appsettings.json`
3. **Deploy** via Web Deploy or FTP to MonsterASP server
4. **Run Migrations** automatically on first startup via `context.Database.Migrate()`
5. **Seed Data** — roles and admin account are auto-created by `DbSeeder`

---

## 🚀 Getting Started (Local Development)

### Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download)
- [SQL Server](https://www.microsoft.com/sql-server) (LocalDB or Express)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or VS Code

### Setup

```bash
# Clone the repository
git clone https://github.com/your-repo/StoreWave.git

# Navigate to project
cd StoreWave

# Restore packages
dotnet restore

# Update connection string in appsettings.json
# Server=.\SQLEXPRESS04;Database=StoreWave;Trusted_Connection=True;

# Run the application
dotnet run
```

### Default Seeded Accounts

| Role | Email | Password |
|------|-------|----------|
| Admin | `ramzyis258@gmail.com` | `Admin123!` |
| Supplier | `arzeka07@gmail.com` | `Supplier123!` |
| Accountant | `arzeka177@gmail.com` | `Accountant123!` |
| Warehouse | `ahmedramzysaeed02@gmail.com` | `Warehouse123!` |

---

<div align="center">

### Built with ❤️ by the StoreWave Engineering Team

**[⬆ Back to Top](#-storewave)**

</div>
