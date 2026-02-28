using Microsoft.Extensions.Options;
using StoreWave.DTOs;
using StoreWave.Services.Interfaces;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace StoreWave.Services.Implementations
{
    /// <summary>
    /// PayPal payment service implementation using REST API
    /// </summary>
    public class PayPalService : IPayPalService
    {
        private readonly HttpClient _httpClient;
        private readonly PayPalSettings _settings;
        private readonly ILogger<PayPalService> _logger;
        private string? _accessToken;
        private DateTime _tokenExpiry;

        public PayPalService(HttpClient httpClient, IOptions<PayPalSettings> settings, ILogger<PayPalService> logger)
        {
            _httpClient = httpClient;
            _settings = settings.Value;
            _logger = logger;
        }

        /// <summary>
        /// Gets OAuth access token from PayPal
        /// </summary>
        private async Task<string> GetAccessTokenAsync()
        {
            if (!string.IsNullOrEmpty(_accessToken) && DateTime.UtcNow < _tokenExpiry)
            {
                return _accessToken;
            }

            var authValue = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_settings.ClientId}:{_settings.ClientSecret}"));
            
            var request = new HttpRequestMessage(HttpMethod.Post, $"{_settings.BaseUrl}/v1/oauth2/token");
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", authValue);
            request.Content = new FormUrlEncodedContent(new[] { new KeyValuePair<string, string>("grant_type", "client_credentials") });

            try
            {
                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                using var document = JsonDocument.Parse(content);
                
                _accessToken = document.RootElement.GetProperty("access_token").GetString()!;
                var expiresIn = document.RootElement.GetProperty("expires_in").GetInt32();
                _tokenExpiry = DateTime.UtcNow.AddSeconds(expiresIn - 60); // Expire 1 minute early

                return _accessToken;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get PayPal access token");
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<PayPalOrderResponseDto> CreateOrderAsync(CreatePayPalOrderDto request)
        {
            try
            {
                var accessToken = await GetAccessTokenAsync();

                var orderRequest = new
                {
                    intent = "CAPTURE",
                    purchase_units = new[]
                    {
                        new
                        {
                            amount = new
                            {
                                currency_code = request.Currency,
                                value = request.Amount.ToString("F2")
                            },
                            description = request.Description
                        }
                    },
                    application_context = new
                    {
                        return_url = request.ReturnUrl,
                        cancel_url = request.CancelUrl,
                        brand_name = "StoreWave",
                        landing_page = "LOGIN",
                        user_action = "PAY_NOW"
                    }
                };

                var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"{_settings.BaseUrl}/v2/checkout/orders");
                httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                httpRequest.Content = new StringContent(JsonSerializer.Serialize(orderRequest), Encoding.UTF8, "application/json");

                var response = await _httpClient.SendAsync(httpRequest);
                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("PayPal create order failed: {Content}", content);
                    return new PayPalOrderResponseDto
                    {
                        Success = false,
                        ErrorMessage = $"PayPal error: {content}"
                    };
                }

                using var document = JsonDocument.Parse(content);
                var orderId = document.RootElement.GetProperty("id").GetString()!;
                var status = document.RootElement.GetProperty("status").GetString()!;

                // Find approval URL from links
                var approvalUrl = "";
                foreach (var link in document.RootElement.GetProperty("links").EnumerateArray())
                {
                    if (link.GetProperty("rel").GetString() == "approve")
                    {
                        approvalUrl = link.GetProperty("href").GetString()!;
                        break;
                    }
                }

                return new PayPalOrderResponseDto
                {
                    OrderId = orderId,
                    Status = status,
                    ApprovalUrl = approvalUrl,
                    Success = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create PayPal order");
                return new PayPalOrderResponseDto
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        /// <inheritdoc />
        public async Task<PayPalCaptureResponseDto> CaptureOrderAsync(string orderId)
        {
            try
            {
                var accessToken = await GetAccessTokenAsync();

                var request = new HttpRequestMessage(HttpMethod.Post, $"{_settings.BaseUrl}/v2/checkout/orders/{orderId}/capture");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                request.Content = new StringContent("{}", Encoding.UTF8, "application/json");

                var response = await _httpClient.SendAsync(request);
                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("PayPal capture failed: {Content}", content);
                    return new PayPalCaptureResponseDto
                    {
                        Success = false,
                        ErrorMessage = $"PayPal error: {content}"
                    };
                }

                using var document = JsonDocument.Parse(content);
                var status = document.RootElement.GetProperty("status").GetString()!;

                var captureResult = new PayPalCaptureResponseDto
                {
                    OrderId = orderId,
                    Status = status,
                    Success = status == "COMPLETED"
                };

                // Extract payer and transaction info
                if (document.RootElement.TryGetProperty("payer", out var payer))
                {
                    captureResult.PayerId = payer.GetProperty("payer_id").GetString() ?? "";
                }

                if (document.RootElement.TryGetProperty("purchase_units", out var purchaseUnits))
                {
                    var firstUnit = purchaseUnits.EnumerateArray().First();
                    if (firstUnit.TryGetProperty("payments", out var payments))
                    {
                        if (payments.TryGetProperty("captures", out var captures))
                        {
                            var capture = captures.EnumerateArray().First();
                            captureResult.TransactionId = capture.GetProperty("id").GetString() ?? "";
                            captureResult.Amount = decimal.Parse(capture.GetProperty("amount").GetProperty("value").GetString()!);
                        }
                    }
                }

                return captureResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to capture PayPal order");
                return new PayPalCaptureResponseDto
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        /// <inheritdoc />
        public async Task<PayPalOrderResponseDto> GetOrderDetailsAsync(string orderId)
        {
            try
            {
                var accessToken = await GetAccessTokenAsync();

                var request = new HttpRequestMessage(HttpMethod.Get, $"{_settings.BaseUrl}/v2/checkout/orders/{orderId}");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                var response = await _httpClient.SendAsync(request);
                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return new PayPalOrderResponseDto
                    {
                        Success = false,
                        ErrorMessage = $"PayPal error: {content}"
                    };
                }

                using var document = JsonDocument.Parse(content);
                return new PayPalOrderResponseDto
                {
                    OrderId = orderId,
                    Status = document.RootElement.GetProperty("status").GetString()!,
                    Success = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get PayPal order details");
                return new PayPalOrderResponseDto
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }
    }
}
