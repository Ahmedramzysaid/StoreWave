using StoreWave.DTOs;

namespace StoreWave.Services.Interfaces
{
    /// <summary>
    /// Service interface for PayPal payment operations
    /// </summary>
    public interface IPayPalService
    {
        /// <summary>
        /// Creates a PayPal order and returns the approval URL for redirect
        /// </summary>
        Task<PayPalOrderResponseDto> CreateOrderAsync(CreatePayPalOrderDto request);

        /// <summary>
        /// Captures the payment after user approves the order
        /// </summary>
        Task<PayPalCaptureResponseDto> CaptureOrderAsync(string orderId);

        /// <summary>
        /// Gets the details of a PayPal order
        /// </summary>
        Task<PayPalOrderResponseDto> GetOrderDetailsAsync(string orderId);
    }
}
