namespace StoreWave.DTOs
{
    /// <summary>
    /// PayPal order creation request
    /// </summary>
    public class CreatePayPalOrderDto
    {
        public string Currency { get; set; } = "USD";
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public string ReturnUrl { get; set; } = string.Empty;
        public string CancelUrl { get; set; } = string.Empty;
    }

    /// <summary>
    /// PayPal order response
    /// </summary>
    public class PayPalOrderResponseDto
    {
        public string OrderId { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string ApprovalUrl { get; set; } = string.Empty;
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
    }

    /// <summary>
    /// PayPal capture response
    /// </summary>
    public class PayPalCaptureResponseDto
    {
        public string OrderId { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string PayerId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public string TransactionId { get; set; } = string.Empty;
    }

    /// <summary>
    /// PayPal configuration from appsettings
    /// </summary>
    public class PayPalSettings
    {
        public string ClientId { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;
        public string Mode { get; set; } = "sandbox"; // sandbox or live
        public string BaseUrl => Mode == "sandbox" 
            ? "https://api-m.sandbox.paypal.com" 
            : "https://api-m.paypal.com";
    }
}
