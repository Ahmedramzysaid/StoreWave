using Microsoft.AspNetCore.Identity;
using StoreWave.Models.Entities;

namespace StoreWave.Data
{
    public static class DbSeeder
    {
        public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<Customer>>();

            // Seed Roles
            string[] roleNames = { "Admin", "Customer", "Supplier", "Accountant", "WarehouseManager" };
            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    await roleManager.CreateAsync(new IdentityRole<int>(roleName));
                }
            }

            // Seed Admin User
            var adminEmail = "ramzyis258@gmail.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                var newAdmin = new Customer
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FirstName = "System",
                    LastName = "Admin",
                    Address = "HQ",
                    City = "Tech City",
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(newAdmin, "Admin123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(newAdmin, "Admin");
                }
            }

            // Seed Demo Supplier
            var supplierEmail = "arzeka07@gmail.com";
            var supplierUser = await userManager.FindByEmailAsync(supplierEmail);
            if (supplierUser == null)
            {
                var newSupplier = new Customer
                {
                    UserName = supplierEmail,
                    Email = supplierEmail,
                    FirstName = "Demo",
                    LastName = "Supplier",
                    Address = "123 Supplier Street",
                    City = "Vendor City",
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true,
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(newSupplier, "Supplier123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(newSupplier, "Supplier");
                }
            }

            // Seed Demo Accountant
            var accountantEmail = "arzeka177@gmail.com";
            var accountantUser = await userManager.FindByEmailAsync(accountantEmail);
            if (accountantUser == null)
            {
                var newAccountant = new Customer
                {
                    UserName = accountantEmail,
                    Email = accountantEmail,
                    FirstName = "Demo",
                    LastName = "Accountant",
                    Address = "456 Finance Ave",
                    City = "Accounting City",
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true,
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(newAccountant, "Accountant123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(newAccountant, "Accountant");
                }
            }

            // Seed Demo Warehouse Manager
            var warehouseEmail = "ahmedramzysaeed02@gmail.com";
            var warehouseUser = await userManager.FindByEmailAsync(warehouseEmail);
            if (warehouseUser == null)
            {
                var newWarehouse = new Customer
                {
                    UserName = warehouseEmail,
                    Email = warehouseEmail,
                    FirstName = "Demo",
                    LastName = "Warehouse",
                    Address = "789 Storage Blvd",
                    City = "Warehouse City",
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true,
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(newWarehouse, "Warehouse123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(newWarehouse, "WarehouseManager");
                }
            }
        }
    }
}
