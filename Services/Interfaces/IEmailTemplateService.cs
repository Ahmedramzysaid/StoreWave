namespace StoreWave.Services.Interfaces
{
    public interface IEmailTemplateService
    {
        string WelcomeEmail(string customerName);
        string OtpEmail(string customerName, string otp);
        string PasswordResetSuccessEmail(string customerName);
        string OrderConfirmationEmail(string customerName, string orderNumber, List<(string ProductName, int Quantity, decimal Price)> items, decimal total);
        string OrderStatusEmail(string customerName, string orderNumber, string newStatus, string statusMessage);
        string AdminNewOrderEmail(string orderNumber, string customerName, decimal total, int itemCount);
        string AccountantOrderEmail(string orderNumber, decimal total, string paymentMethod, string customerName);
        string SupplierOrderEmail(string supplierName, string orderNumber, List<(string ProductName, int Quantity, decimal Price)> items);
        string WarehouseOrderEmail(string orderNumber, List<(string ProductName, int Quantity)> items, string shippingAddress);
    }
}
