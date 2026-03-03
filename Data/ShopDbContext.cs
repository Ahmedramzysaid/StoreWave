using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StoreWave.Models.Entities;

namespace StoreWave.Data
{
    public class ShopDbContext : IdentityDbContext<Customer, IdentityRole<int>, int>
    {
        public ShopDbContext(DbContextOptions<ShopDbContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        // Customers DbSet is provided by IdentityDbContext as Users, but we can keep this alias or remove it.
        // Keeping it for compatibility with existing code that uses _context.Customers
        public DbSet<Customer> Customers { get; set; } 
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Product Configuration
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasIndex(e => e.Name);
                entity.HasIndex(e => e.IsActive);
                entity.HasIndex(e => e.IsFeatured);
                
                entity.HasOne(e => e.Category)
                    .WithMany(c => c.Products)
                    .HasForeignKey(e => e.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Supplier)
                    .WithMany(s => s.SupplierProducts)
                    .HasForeignKey(e => e.SupplierId)
                    .IsRequired(false)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Category Configuration
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasIndex(e => e.Name).IsUnique();
            });

            // Order Configuration
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasIndex(e => e.OrderNumber).IsUnique();
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.OrderDate);

                entity.HasOne(e => e.Customer)
                    .WithMany(c => c.Orders)
                    .HasForeignKey(e => e.CustomerId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Driver)
                    .WithMany()
                    .HasForeignKey(e => e.DriverId)
                    .IsRequired(false)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // OrderItem Configuration
            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.HasOne(e => e.Order)
                    .WithMany(o => o.OrderItems)
                    .HasForeignKey(e => e.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Product)
                    .WithMany(p => p.OrderItems)
                    .HasForeignKey(e => e.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // CartItem Configuration
            modelBuilder.Entity<CartItem>(entity =>
            {
                entity.HasIndex(e => new { e.CustomerId, e.ProductId }).IsUnique();

                entity.HasOne(e => e.Customer)
                    .WithMany(c => c.CartItems)
                    .HasForeignKey(e => e.CustomerId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Product)
                    .WithMany(p => p.CartItems)
                    .HasForeignKey(e => e.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Review Configuration
            modelBuilder.Entity<Review>(entity =>
            {
                entity.HasIndex(e => new { e.CustomerId, e.ProductId }).IsUnique();

                entity.HasOne(e => e.Product)
                    .WithMany(p => p.Reviews)
                    .HasForeignKey(e => e.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Customer)
                    .WithMany(c => c.Reviews)
                    .HasForeignKey(e => e.CustomerId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ChatMessage Configuration
            modelBuilder.Entity<ChatMessage>(entity =>
            {
                entity.HasIndex(e => e.OrderId);
                entity.HasIndex(e => e.SentAt);

                entity.HasOne(e => e.Order)
                    .WithMany(o => o.ChatMessages)
                    .HasForeignKey(e => e.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Sender)
                    .WithMany()
                    .HasForeignKey(e => e.SenderId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Customize Identity Table Names (Optional, keeping default AspNet* names is fine)
            modelBuilder.Entity<Customer>().ToTable("Users");
            modelBuilder.Entity<IdentityRole<int>>().ToTable("Roles");
            modelBuilder.Entity<IdentityUserClaim<int>>().ToTable("UserClaims");
            modelBuilder.Entity<IdentityUserRole<int>>().ToTable("UserRoles");
            modelBuilder.Entity<IdentityUserLogin<int>>().ToTable("UserLogins");
            modelBuilder.Entity<IdentityRoleClaim<int>>().ToTable("RoleClaims");
            modelBuilder.Entity<IdentityUserToken<int>>().ToTable("UserTokens");

            // Seed Data (Products Only)
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            var seedDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            // Seed Categories
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Electronics", Description = "Electronic devices and gadgets", IsActive = true, CreatedAt = seedDate },
                new Category { Id = 2, Name = "Clothing", Description = "Fashion and apparel", IsActive = true, CreatedAt = seedDate },
                new Category { Id = 3, Name = "Books", Description = "Books and publications", IsActive = true, CreatedAt = seedDate },
                new Category { Id = 4, Name = "Home & Garden", Description = "Home decor and garden supplies", IsActive = true, CreatedAt = seedDate },
                new Category { Id = 5, Name = "Sports", Description = "Sports equipment and accessories", IsActive = true, CreatedAt = seedDate }
            );

            // Seed Products — 10 per category (50 total)
            modelBuilder.Entity<Product>().HasData(

                // ── Electronics (CategoryId = 1) ──
                new Product { Id = 1,  Name = "Wireless Headphones",       Description = "High-quality Bluetooth headphones with noise cancellation",          Price = 149.99m, DiscountPrice = 119.99m, StockQuantity = 50,  CategoryId = 1, IsActive = true, IsFeatured = true,  CreatedAt = seedDate, ImageUrl = "/images/products/headphones.png" },
                new Product { Id = 2,  Name = "Smart Watch",               Description = "Feature-rich smartwatch with health tracking",                       Price = 299.99m,                          StockQuantity = 30,  CategoryId = 1, IsActive = true, IsFeatured = true,  CreatedAt = seedDate, ImageUrl = "/images/products/smartwatch.png" },
                new Product { Id = 3,  Name = "Laptop Stand",              Description = "Ergonomic aluminum laptop stand",                                    Price = 49.99m,  DiscountPrice = 39.99m,  StockQuantity = 100, CategoryId = 1, IsActive = true, IsFeatured = false, CreatedAt = seedDate, ImageUrl = "/images/products/laptopstand.png" },
                new Product { Id = 4,  Name = "Bluetooth Speaker",         Description = "Portable waterproof Bluetooth speaker with 20-hour battery life",    Price = 79.99m,  DiscountPrice = 59.99m,  StockQuantity = 80,  CategoryId = 1, IsActive = true, IsFeatured = true,  CreatedAt = seedDate, ImageUrl = "/images/products/speaker.png" },
                new Product { Id = 5,  Name = "USB-C Hub",                 Description = "7-in-1 USB-C multiport adapter with HDMI and Ethernet",              Price = 45.99m,                           StockQuantity = 120, CategoryId = 1, IsActive = true, IsFeatured = false, CreatedAt = seedDate, ImageUrl = "/images/products/usbhub.png" },
                new Product { Id = 6,  Name = "Mechanical Keyboard",       Description = "RGB mechanical gaming keyboard with Cherry MX switches",             Price = 129.99m, DiscountPrice = 99.99m,  StockQuantity = 60,  CategoryId = 1, IsActive = true, IsFeatured = false, CreatedAt = seedDate, ImageUrl = "/images/products/keyboard.png" },
                new Product { Id = 7,  Name = "Wireless Mouse",            Description = "Ergonomic wireless mouse with adjustable DPI",                       Price = 34.99m,                           StockQuantity = 150, CategoryId = 1, IsActive = true, IsFeatured = false, CreatedAt = seedDate, ImageUrl = "/images/products/mouse.png" },
                new Product { Id = 8,  Name = "4K Webcam",                 Description = "Ultra HD webcam with auto-focus and built-in microphone",             Price = 89.99m,  DiscountPrice = 74.99m,  StockQuantity = 45,  CategoryId = 1, IsActive = true, IsFeatured = false, CreatedAt = seedDate, ImageUrl = "/images/products/webcam.png" },
                new Product { Id = 9,  Name = "Portable Charger",          Description = "20000mAh fast-charging power bank with dual USB output",             Price = 39.99m,                           StockQuantity = 200, CategoryId = 1, IsActive = true, IsFeatured = false, CreatedAt = seedDate, ImageUrl = "/images/products/charger.png" },
                new Product { Id = 10, Name = "Noise-Cancelling Earbuds",  Description = "True wireless earbuds with active noise cancellation and transparency mode", Price = 199.99m, DiscountPrice = 159.99m, StockQuantity = 70, CategoryId = 1, IsActive = true, IsFeatured = true, CreatedAt = seedDate, ImageUrl = "/images/products/earbuds.png" },

                // ── Clothing (CategoryId = 2) ──
                new Product { Id = 11, Name = "Men's Casual Shirt",        Description = "Cotton casual shirt for everyday wear",                              Price = 39.99m,                           StockQuantity = 75,  CategoryId = 2, IsActive = true, IsFeatured = false, CreatedAt = seedDate, ImageUrl = "/images/products/shirt.png" },
                new Product { Id = 12, Name = "Women's Summer Dress",      Description = "Elegant summer dress with floral print",                             Price = 59.99m,  DiscountPrice = 44.99m,  StockQuantity = 40,  CategoryId = 2, IsActive = true, IsFeatured = true,  CreatedAt = seedDate, ImageUrl = "/images/products/dress.png" },
                new Product { Id = 13, Name = "Denim Jacket",              Description = "Classic blue denim jacket with button closure",                       Price = 89.99m,  DiscountPrice = 69.99m,  StockQuantity = 55,  CategoryId = 2, IsActive = true, IsFeatured = true,  CreatedAt = seedDate, ImageUrl = "/images/products/denimjacket.png" },
                new Product { Id = 14, Name = "Running Sneakers",          Description = "Lightweight breathable running shoes with gel cushioning",            Price = 119.99m, DiscountPrice = 94.99m,  StockQuantity = 90,  CategoryId = 2, IsActive = true, IsFeatured = true,  CreatedAt = seedDate, ImageUrl = "/images/products/sneakers.png" },
                new Product { Id = 15, Name = "Wool Scarf",                Description = "Soft merino wool scarf for cold weather",                             Price = 29.99m,                           StockQuantity = 110, CategoryId = 2, IsActive = true, IsFeatured = false, CreatedAt = seedDate, ImageUrl = "/images/products/scarf.png" },
                new Product { Id = 16, Name = "Leather Belt",              Description = "Genuine leather belt with brushed metal buckle",                      Price = 34.99m,                           StockQuantity = 130, CategoryId = 2, IsActive = true, IsFeatured = false, CreatedAt = seedDate, ImageUrl = "/images/products/belt.png" },
                new Product { Id = 17, Name = "Cotton Polo Shirt",         Description = "Classic fit polo shirt in assorted colors",                           Price = 44.99m,  DiscountPrice = 34.99m,  StockQuantity = 85,  CategoryId = 2, IsActive = true, IsFeatured = false, CreatedAt = seedDate, ImageUrl = "/images/products/polo.png" },
                new Product { Id = 18, Name = "Slim Fit Chinos",           Description = "Stretch chino pants with modern slim fit",                            Price = 54.99m,                           StockQuantity = 65,  CategoryId = 2, IsActive = true, IsFeatured = false, CreatedAt = seedDate, ImageUrl = "/images/products/chinos.png" },
                new Product { Id = 19, Name = "Winter Parka",              Description = "Insulated waterproof parka with faux fur hood",                       Price = 179.99m, DiscountPrice = 139.99m, StockQuantity = 35,  CategoryId = 2, IsActive = true, IsFeatured = false, CreatedAt = seedDate, ImageUrl = "/images/products/parka.png" },
                new Product { Id = 20, Name = "Sunglasses",                Description = "UV400 polarized sunglasses with metal frame",                         Price = 24.99m,                           StockQuantity = 200, CategoryId = 2, IsActive = true, IsFeatured = false, CreatedAt = seedDate, ImageUrl = "/images/products/sunglasses.png" },

                // ── Books (CategoryId = 3) ──
                new Product { Id = 21, Name = "Programming in C#",         Description = "Comprehensive guide to C# programming",                              Price = 44.99m,                           StockQuantity = 200, CategoryId = 3, IsActive = true, IsFeatured = true,  CreatedAt = seedDate, ImageUrl = "/images/products/csharpbook.png" },
                new Product { Id = 22, Name = "The Art of Web Design",     Description = "Modern web design principles and practices",                          Price = 34.99m,                           StockQuantity = 150, CategoryId = 3, IsActive = true, IsFeatured = false, CreatedAt = seedDate, ImageUrl = "/images/products/webdesignbook.png" },
                new Product { Id = 23, Name = "Clean Code",                Description = "A handbook of agile software craftsmanship by Robert C. Martin",      Price = 39.99m,  DiscountPrice = 29.99m,  StockQuantity = 180, CategoryId = 3, IsActive = true, IsFeatured = true,  CreatedAt = seedDate, ImageUrl = "/images/products/cleancode.png" },
                new Product { Id = 24, Name = "Design Patterns",           Description = "Elements of reusable object-oriented software",                       Price = 49.99m,                           StockQuantity = 120, CategoryId = 3, IsActive = true, IsFeatured = false, CreatedAt = seedDate, ImageUrl = "/images/products/designpatterns.png" },
                new Product { Id = 25, Name = "Data Structures & Algorithms", Description = "In-depth guide to data structures and algorithms in Python",      Price = 42.99m,  DiscountPrice = 34.99m,  StockQuantity = 140, CategoryId = 3, IsActive = true, IsFeatured = false, CreatedAt = seedDate, ImageUrl = "/images/products/dsabook.png" },
                new Product { Id = 26, Name = "The Pragmatic Programmer",  Description = "Your journey to mastery — 20th anniversary edition",                  Price = 46.99m,                           StockQuantity = 100, CategoryId = 3, IsActive = true, IsFeatured = true,  CreatedAt = seedDate, ImageUrl = "/images/products/pragmatic.png" },
                new Product { Id = 27, Name = "Introduction to AI",        Description = "A modern approach to artificial intelligence fundamentals",            Price = 54.99m,  DiscountPrice = 44.99m,  StockQuantity = 90,  CategoryId = 3, IsActive = true, IsFeatured = false, CreatedAt = seedDate, ImageUrl = "/images/products/aibook.png" },
                new Product { Id = 28, Name = "Database Systems",          Description = "Complete guide to database design and SQL",                            Price = 38.99m,                           StockQuantity = 160, CategoryId = 3, IsActive = true, IsFeatured = false, CreatedAt = seedDate, ImageUrl = "/images/products/databasebook.png" },
                new Product { Id = 29, Name = "Learning JavaScript",       Description = "From beginner to professional JavaScript developer",                  Price = 32.99m,  DiscountPrice = 24.99m,  StockQuantity = 170, CategoryId = 3, IsActive = true, IsFeatured = false, CreatedAt = seedDate, ImageUrl = "/images/products/jsbook.png" },
                new Product { Id = 30, Name = "Cloud Computing Essentials",Description = "Mastering AWS, Azure, and Google Cloud platforms",                     Price = 48.99m,                           StockQuantity = 110, CategoryId = 3, IsActive = true, IsFeatured = false, CreatedAt = seedDate, ImageUrl = "/images/products/cloudbook.png" },

                // ── Home & Garden (CategoryId = 4) ──
                new Product { Id = 31, Name = "Ceramic Plant Pot Set",     Description = "Set of 3 modern ceramic plant pots with drainage holes",              Price = 34.99m,  DiscountPrice = 27.99m,  StockQuantity = 80,  CategoryId = 4, IsActive = true, IsFeatured = true,  CreatedAt = seedDate, ImageUrl = "/images/products/plantpots.png" },
                new Product { Id = 32, Name = "LED Desk Lamp",             Description = "Adjustable LED desk lamp with 5 brightness levels and USB charging",  Price = 44.99m,                           StockQuantity = 95,  CategoryId = 4, IsActive = true, IsFeatured = true,  CreatedAt = seedDate, ImageUrl = "/images/products/desklamp.png" },
                new Product { Id = 33, Name = "Throw Blanket",             Description = "Ultra-soft fleece throw blanket 150x200cm",                            Price = 29.99m,  DiscountPrice = 22.99m,  StockQuantity = 120, CategoryId = 4, IsActive = true, IsFeatured = false, CreatedAt = seedDate, ImageUrl = "/images/products/blanket.png" },
                new Product { Id = 34, Name = "Wall Art Canvas",           Description = "Abstract wall art canvas print set — 3 panels",                        Price = 64.99m,                           StockQuantity = 50,  CategoryId = 4, IsActive = true, IsFeatured = false, CreatedAt = seedDate, ImageUrl = "/images/products/wallart.png" },
                new Product { Id = 35, Name = "Garden Tool Set",           Description = "5-piece stainless steel garden tool kit with carrying bag",             Price = 42.99m,  DiscountPrice = 34.99m,  StockQuantity = 70,  CategoryId = 4, IsActive = true, IsFeatured = false, CreatedAt = seedDate, ImageUrl = "/images/products/gardentools.png" },
                new Product { Id = 36, Name = "Scented Candle Collection", Description = "Set of 4 soy wax scented candles — lavender, vanilla, rose, cedar",    Price = 24.99m,                           StockQuantity = 150, CategoryId = 4, IsActive = true, IsFeatured = false, CreatedAt = seedDate, ImageUrl = "/images/products/candles.png" },
                new Product { Id = 37, Name = "Kitchen Organizer Rack",    Description = "3-tier stainless steel kitchen spice and utensil organizer",            Price = 39.99m,  DiscountPrice = 31.99m,  StockQuantity = 85,  CategoryId = 4, IsActive = true, IsFeatured = false, CreatedAt = seedDate, ImageUrl = "/images/products/organizer.png" },
                new Product { Id = 38, Name = "Decorative Cushion Covers", Description = "Set of 4 linen cushion covers with geometric patterns",                Price = 19.99m,                           StockQuantity = 200, CategoryId = 4, IsActive = true, IsFeatured = false, CreatedAt = seedDate, ImageUrl = "/images/products/cushions.png" },
                new Product { Id = 39, Name = "Solar Garden Lights",       Description = "Pack of 8 waterproof LED solar pathway lights",                        Price = 32.99m,  DiscountPrice = 26.99m,  StockQuantity = 100, CategoryId = 4, IsActive = true, IsFeatured = true,  CreatedAt = seedDate, ImageUrl = "/images/products/solarlights.png" },
                new Product { Id = 40, Name = "Indoor Herb Garden Kit",    Description = "Self-watering indoor herb garden with basil, mint, and parsley seeds",  Price = 27.99m,                           StockQuantity = 60,  CategoryId = 4, IsActive = true, IsFeatured = false, CreatedAt = seedDate, ImageUrl = "/images/products/herbgarden.png" },

                // ── Sports (CategoryId = 5) ──
                new Product { Id = 41, Name = "Yoga Mat",                  Description = "Non-slip eco-friendly TPE yoga mat 6mm thick",                         Price = 29.99m,  DiscountPrice = 22.99m,  StockQuantity = 100, CategoryId = 5, IsActive = true, IsFeatured = true,  CreatedAt = seedDate, ImageUrl = "/images/products/yogamat.png" },
                new Product { Id = 42, Name = "Adjustable Dumbbells",      Description = "Pair of adjustable dumbbells 5–25 kg with quick-lock system",           Price = 199.99m, DiscountPrice = 169.99m, StockQuantity = 40,  CategoryId = 5, IsActive = true, IsFeatured = true,  CreatedAt = seedDate, ImageUrl = "/images/products/dumbbells.png" },
                new Product { Id = 43, Name = "Resistance Bands Set",      Description = "Set of 5 resistance bands with different tension levels",              Price = 19.99m,                           StockQuantity = 180, CategoryId = 5, IsActive = true, IsFeatured = false, CreatedAt = seedDate, ImageUrl = "/images/products/bands.png" },
                new Product { Id = 44, Name = "Sports Water Bottle",       Description = "1-liter insulated stainless steel water bottle",                       Price = 24.99m,                           StockQuantity = 200, CategoryId = 5, IsActive = true, IsFeatured = false, CreatedAt = seedDate, ImageUrl = "/images/products/waterbottle.png" },
                new Product { Id = 45, Name = "Jump Rope",                 Description = "Speed jump rope with ball bearings and adjustable length",             Price = 14.99m,  DiscountPrice = 11.99m,  StockQuantity = 150, CategoryId = 5, IsActive = true, IsFeatured = false, CreatedAt = seedDate, ImageUrl = "/images/products/jumprope.png" },
                new Product { Id = 46, Name = "Foam Roller",               Description = "High-density foam roller for muscle recovery — 45cm",                  Price = 24.99m,                           StockQuantity = 110, CategoryId = 5, IsActive = true, IsFeatured = false, CreatedAt = seedDate, ImageUrl = "/images/products/foamroller.png" },
                new Product { Id = 47, Name = "Basketball",                Description = "Official size 7 indoor/outdoor composite leather basketball",          Price = 34.99m,  DiscountPrice = 28.99m,  StockQuantity = 75,  CategoryId = 5, IsActive = true, IsFeatured = false, CreatedAt = seedDate, ImageUrl = "/images/products/basketball.png" },
                new Product { Id = 48, Name = "Tennis Racket",             Description = "Lightweight graphite tennis racket with vibration dampener",            Price = 89.99m,                           StockQuantity = 45,  CategoryId = 5, IsActive = true, IsFeatured = true,  CreatedAt = seedDate, ImageUrl = "/images/products/tennisracket.png" },
                new Product { Id = 49, Name = "Fitness Tracker Band",      Description = "Waterproof fitness tracker with heart-rate monitor and sleep tracking", Price = 49.99m,  DiscountPrice = 39.99m,  StockQuantity = 90,  CategoryId = 5, IsActive = true, IsFeatured = false, CreatedAt = seedDate, ImageUrl = "/images/products/fitnesstracker.png" },
                new Product { Id = 50, Name = "Camping Backpack",          Description = "50L waterproof hiking backpack with multiple compartments",             Price = 74.99m,  DiscountPrice = 59.99m,  StockQuantity = 55,  CategoryId = 5, IsActive = true, IsFeatured = false, CreatedAt = seedDate, ImageUrl = "/images/products/backpack.png" }
            );
        }
    }
}
