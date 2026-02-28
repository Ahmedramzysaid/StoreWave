using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace StoreWave.Migrations
{
    /// <inheritdoc />
    public partial class SeedProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CategoryId", "Description", "DiscountPrice", "ImageUrl", "IsFeatured", "Name", "Price", "StockQuantity" },
                values: new object[] { 1, "Portable waterproof Bluetooth speaker with 20-hour battery life", 59.99m, "/images/products/speaker.png", true, "Bluetooth Speaker", 79.99m, 80 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CategoryId", "Description", "DiscountPrice", "ImageUrl", "IsFeatured", "Name", "Price", "StockQuantity" },
                values: new object[] { 1, "7-in-1 USB-C multiport adapter with HDMI and Ethernet", null, "/images/products/usbhub.png", false, "USB-C Hub", 45.99m, 120 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CategoryId", "Description", "DiscountPrice", "ImageUrl", "Name", "Price", "StockQuantity" },
                values: new object[] { 1, "RGB mechanical gaming keyboard with Cherry MX switches", 99.99m, "/images/products/keyboard.png", "Mechanical Keyboard", 129.99m, 60 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "CategoryId", "Description", "ImageUrl", "Name" },
                values: new object[] { 1, "Ergonomic wireless mouse with adjustable DPI", "/images/products/mouse.png", "Wireless Mouse" });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "CategoryId", "CreatedAt", "Description", "DiscountPrice", "ImageUrl", "IsActive", "IsFeatured", "Name", "Price", "StockQuantity", "SupplierId", "UpdatedAt" },
                values: new object[,]
                {
                    { 8, 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Ultra HD webcam with auto-focus and built-in microphone", 74.99m, "/images/products/webcam.png", true, false, "4K Webcam", 89.99m, 45, null, null },
                    { 9, 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "20000mAh fast-charging power bank with dual USB output", null, "/images/products/charger.png", true, false, "Portable Charger", 39.99m, 200, null, null },
                    { 10, 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "True wireless earbuds with active noise cancellation and transparency mode", 159.99m, "/images/products/earbuds.png", true, true, "Noise-Cancelling Earbuds", 199.99m, 70, null, null },
                    { 11, 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Cotton casual shirt for everyday wear", null, "/images/products/shirt.png", true, false, "Men's Casual Shirt", 39.99m, 75, null, null },
                    { 12, 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Elegant summer dress with floral print", 44.99m, "/images/products/dress.png", true, true, "Women's Summer Dress", 59.99m, 40, null, null },
                    { 13, 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Classic blue denim jacket with button closure", 69.99m, "/images/products/denimjacket.png", true, true, "Denim Jacket", 89.99m, 55, null, null },
                    { 14, 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Lightweight breathable running shoes with gel cushioning", 94.99m, "/images/products/sneakers.png", true, true, "Running Sneakers", 119.99m, 90, null, null },
                    { 15, 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Soft merino wool scarf for cold weather", null, "/images/products/scarf.png", true, false, "Wool Scarf", 29.99m, 110, null, null },
                    { 16, 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Genuine leather belt with brushed metal buckle", null, "/images/products/belt.png", true, false, "Leather Belt", 34.99m, 130, null, null },
                    { 17, 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Classic fit polo shirt in assorted colors", 34.99m, "/images/products/polo.png", true, false, "Cotton Polo Shirt", 44.99m, 85, null, null },
                    { 18, 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Stretch chino pants with modern slim fit", null, "/images/products/chinos.png", true, false, "Slim Fit Chinos", 54.99m, 65, null, null },
                    { 19, 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Insulated waterproof parka with faux fur hood", 139.99m, "/images/products/parka.png", true, false, "Winter Parka", 179.99m, 35, null, null },
                    { 20, 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "UV400 polarized sunglasses with metal frame", null, "/images/products/sunglasses.png", true, false, "Sunglasses", 24.99m, 200, null, null },
                    { 21, 3, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Comprehensive guide to C# programming", null, "/images/products/csharpbook.png", true, true, "Programming in C#", 44.99m, 200, null, null },
                    { 22, 3, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Modern web design principles and practices", null, "/images/products/webdesignbook.png", true, false, "The Art of Web Design", 34.99m, 150, null, null },
                    { 23, 3, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "A handbook of agile software craftsmanship by Robert C. Martin", 29.99m, "/images/products/cleancode.png", true, true, "Clean Code", 39.99m, 180, null, null },
                    { 24, 3, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Elements of reusable object-oriented software", null, "/images/products/designpatterns.png", true, false, "Design Patterns", 49.99m, 120, null, null },
                    { 25, 3, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "In-depth guide to data structures and algorithms in Python", 34.99m, "/images/products/dsabook.png", true, false, "Data Structures & Algorithms", 42.99m, 140, null, null },
                    { 26, 3, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Your journey to mastery — 20th anniversary edition", null, "/images/products/pragmatic.png", true, true, "The Pragmatic Programmer", 46.99m, 100, null, null },
                    { 27, 3, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "A modern approach to artificial intelligence fundamentals", 44.99m, "/images/products/aibook.png", true, false, "Introduction to AI", 54.99m, 90, null, null },
                    { 28, 3, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Complete guide to database design and SQL", null, "/images/products/databasebook.png", true, false, "Database Systems", 38.99m, 160, null, null },
                    { 29, 3, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "From beginner to professional JavaScript developer", 24.99m, "/images/products/jsbook.png", true, false, "Learning JavaScript", 32.99m, 170, null, null },
                    { 30, 3, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Mastering AWS, Azure, and Google Cloud platforms", null, "/images/products/cloudbook.png", true, false, "Cloud Computing Essentials", 48.99m, 110, null, null },
                    { 31, 4, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Set of 3 modern ceramic plant pots with drainage holes", 27.99m, "/images/products/plantpots.png", true, true, "Ceramic Plant Pot Set", 34.99m, 80, null, null },
                    { 32, 4, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Adjustable LED desk lamp with 5 brightness levels and USB charging", null, "/images/products/desklamp.png", true, true, "LED Desk Lamp", 44.99m, 95, null, null },
                    { 33, 4, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Ultra-soft fleece throw blanket 150x200cm", 22.99m, "/images/products/blanket.png", true, false, "Throw Blanket", 29.99m, 120, null, null },
                    { 34, 4, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Abstract wall art canvas print set — 3 panels", null, "/images/products/wallart.png", true, false, "Wall Art Canvas", 64.99m, 50, null, null },
                    { 35, 4, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "5-piece stainless steel garden tool kit with carrying bag", 34.99m, "/images/products/gardentools.png", true, false, "Garden Tool Set", 42.99m, 70, null, null },
                    { 36, 4, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Set of 4 soy wax scented candles — lavender, vanilla, rose, cedar", null, "/images/products/candles.png", true, false, "Scented Candle Collection", 24.99m, 150, null, null },
                    { 37, 4, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "3-tier stainless steel kitchen spice and utensil organizer", 31.99m, "/images/products/organizer.png", true, false, "Kitchen Organizer Rack", 39.99m, 85, null, null },
                    { 38, 4, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Set of 4 linen cushion covers with geometric patterns", null, "/images/products/cushions.png", true, false, "Decorative Cushion Covers", 19.99m, 200, null, null },
                    { 39, 4, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Pack of 8 waterproof LED solar pathway lights", 26.99m, "/images/products/solarlights.png", true, true, "Solar Garden Lights", 32.99m, 100, null, null },
                    { 40, 4, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Self-watering indoor herb garden with basil, mint, and parsley seeds", null, "/images/products/herbgarden.png", true, false, "Indoor Herb Garden Kit", 27.99m, 60, null, null },
                    { 41, 5, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Non-slip eco-friendly TPE yoga mat 6mm thick", 22.99m, "/images/products/yogamat.png", true, true, "Yoga Mat", 29.99m, 100, null, null },
                    { 42, 5, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Pair of adjustable dumbbells 5–25 kg with quick-lock system", 169.99m, "/images/products/dumbbells.png", true, true, "Adjustable Dumbbells", 199.99m, 40, null, null },
                    { 43, 5, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Set of 5 resistance bands with different tension levels", null, "/images/products/bands.png", true, false, "Resistance Bands Set", 19.99m, 180, null, null },
                    { 44, 5, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "1-liter insulated stainless steel water bottle", null, "/images/products/waterbottle.png", true, false, "Sports Water Bottle", 24.99m, 200, null, null },
                    { 45, 5, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Speed jump rope with ball bearings and adjustable length", 11.99m, "/images/products/jumprope.png", true, false, "Jump Rope", 14.99m, 150, null, null },
                    { 46, 5, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "High-density foam roller for muscle recovery — 45cm", null, "/images/products/foamroller.png", true, false, "Foam Roller", 24.99m, 110, null, null },
                    { 47, 5, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Official size 7 indoor/outdoor composite leather basketball", 28.99m, "/images/products/basketball.png", true, false, "Basketball", 34.99m, 75, null, null },
                    { 48, 5, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Lightweight graphite tennis racket with vibration dampener", null, "/images/products/tennisracket.png", true, true, "Tennis Racket", 89.99m, 45, null, null },
                    { 49, 5, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Waterproof fitness tracker with heart-rate monitor and sleep tracking", 39.99m, "/images/products/fitnesstracker.png", true, false, "Fitness Tracker Band", 49.99m, 90, null, null },
                    { 50, 5, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "50L waterproof hiking backpack with multiple compartments", 59.99m, "/images/products/backpack.png", true, false, "Camping Backpack", 74.99m, 55, null, null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 34);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 35);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 36);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 37);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 38);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 39);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 40);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 41);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 42);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 43);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 44);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 45);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 46);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 47);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 48);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 49);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 50);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CategoryId", "Description", "DiscountPrice", "ImageUrl", "IsFeatured", "Name", "Price", "StockQuantity" },
                values: new object[] { 2, "Cotton casual shirt for everyday wear", null, "/images/products/shirt.png", false, "Men's Casual Shirt", 39.99m, 75 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CategoryId", "Description", "DiscountPrice", "ImageUrl", "IsFeatured", "Name", "Price", "StockQuantity" },
                values: new object[] { 2, "Elegant summer dress with floral print", 44.99m, "/images/products/dress.png", true, "Women's Summer Dress", 59.99m, 40 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CategoryId", "Description", "DiscountPrice", "ImageUrl", "Name", "Price", "StockQuantity" },
                values: new object[] { 3, "Comprehensive guide to C# programming", null, "/images/products/csharpbook.png", "Programming in C#", 44.99m, 200 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "CategoryId", "Description", "ImageUrl", "Name" },
                values: new object[] { 3, "Modern web design principles and practices", "/images/products/csharpbook.png", "The Art of Web Design" });
        }
    }
}
