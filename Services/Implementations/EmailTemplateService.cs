using StoreWave.Services.Interfaces;

namespace StoreWave.Services.Implementations
{
    public class EmailTemplateService : IEmailTemplateService
    {
        private string BaseTemplate(string title, string accentColor, string content)
        {
            return $@"
<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>{title}</title>
</head>
<body style=""margin:0; padding:0; background-color:#f0f2f5; font-family:'Segoe UI',Roboto,'Helvetica Neue',Arial,sans-serif;"">
    <table role=""presentation"" width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""background-color:#f0f2f5; padding:30px 0;"">
        <tr>
            <td align=""center"">
                <table role=""presentation"" width=""600"" cellpadding=""0"" cellspacing=""0"" style=""max-width:600px; width:100%;"">
                    
                    <!-- Header -->
                    <tr>
                        <td style=""background: linear-gradient(135deg, #1a1a2e 0%, #16213e 50%, #0f3460 100%); padding:35px 40px; border-radius:16px 16px 0 0; text-align:center;"">
                            <table role=""presentation"" width=""100%"" cellpadding=""0"" cellspacing=""0"">
                                <tr>
                                    <td align=""center"">
                                        <div style=""display:inline-block; background:rgba(255,255,255,0.15); border-radius:12px; padding:10px 16px; margin-bottom:12px;"">
                                            <span style=""font-size:28px; color:#fff; font-weight:700; letter-spacing:1px;"">🛒 StoreWave</span>
                                        </div>
                                    </td>
                                </tr>
                                <tr>
                                    <td align=""center"" style=""padding-top:8px;"">
                                        <span style=""color:rgba(255,255,255,0.7); font-size:13px; letter-spacing:2px; text-transform:uppercase;"">{title}</span>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    
                    <!-- Accent Bar -->
                    <tr>
                        <td style=""height:4px; background: linear-gradient(90deg, {accentColor}, {accentColor}88);""></td>
                    </tr>
                    
                    <!-- Body -->
                    <tr>
                        <td style=""background-color:#ffffff; padding:40px; border-radius:0 0 16px 16px; box-shadow:0 4px 24px rgba(0,0,0,0.08);"">
                            {content}
                        </td>
                    </tr>
                    
                    <!-- Footer -->
                    <tr>
                        <td style=""padding:30px 40px; text-align:center;"">
                            <table role=""presentation"" width=""100%"" cellpadding=""0"" cellspacing=""0"">
                                <tr>
                                    <td align=""center"" style=""padding-bottom:16px;"">
                                        <span style=""display:inline-block; background:#1a1a2e; color:#fff; border-radius:8px; padding:8px 16px; font-size:14px; font-weight:600;"">🛒 StoreWave</span>
                                    </td>
                                </tr>
                                <tr>
                                    <td align=""center"" style=""color:#999; font-size:12px; line-height:20px;"">
                                        <p style=""margin:0 0 4px 0;"">This is an automated message from StoreWave.</p>
                                        <p style=""margin:0 0 4px 0;"">Please do not reply directly to this email.</p>
                                        <p style=""margin:12px 0 0 0; color:#bbb;"">&copy; {DateTime.Now.Year} StoreWave. All rights reserved.</p>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    
                </table>
            </td>
        </tr>
    </table>
</body>
</html>";
        }

        // ─── Customer Emails ────────────────────────────────────────────

        public string WelcomeEmail(string customerName)
        {
            var content = $@"
                <div style=""text-align:center; margin-bottom:24px;"">
                    <div style=""display:inline-block; background:#e8f5e9; border-radius:50%; width:64px; height:64px; line-height:64px; font-size:32px;"">🎉</div>
                </div>
                <h1 style=""color:#1a1a2e; font-size:24px; font-weight:700; text-align:center; margin:0 0 8px 0;"">Welcome to StoreWave!</h1>
                <p style=""color:#666; font-size:15px; text-align:center; margin:0 0 28px 0;"">We're thrilled to have you on board, <strong style=""color:#1a1a2e;"">{customerName}</strong>!</p>
                
                <div style=""background: linear-gradient(135deg, #f8f9ff 0%, #f0f4ff 100%); border-radius:12px; padding:24px; margin-bottom:24px;"">
                    <p style=""color:#333; font-size:14px; line-height:24px; margin:0;"">Your account has been created successfully. Here's what you can do:</p>
                    <table role=""presentation"" width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""margin-top:16px;"">
                        <tr>
                            <td style=""padding:8px 0;"">
                                <span style=""display:inline-block; background:#4CAF50; color:#fff; border-radius:50%; width:24px; height:24px; text-align:center; line-height:24px; font-size:12px; margin-right:10px;"">✓</span>
                                <span style=""color:#333; font-size:14px;"">Browse thousands of products</span>
                            </td>
                        </tr>
                        <tr>
                            <td style=""padding:8px 0;"">
                                <span style=""display:inline-block; background:#2196F3; color:#fff; border-radius:50%; width:24px; height:24px; text-align:center; line-height:24px; font-size:12px; margin-right:10px;"">✓</span>
                                <span style=""color:#333; font-size:14px;"">Track your orders in real-time</span>
                            </td>
                        </tr>
                        <tr>
                            <td style=""padding:8px 0;"">
                                <span style=""display:inline-block; background:#FF9800; color:#fff; border-radius:50%; width:24px; height:24px; text-align:center; line-height:24px; font-size:12px; margin-right:10px;"">✓</span>
                                <span style=""color:#333; font-size:14px;"">Leave reviews and earn rewards</span>
                            </td>
                        </tr>
                    </table>
                </div>

                <div style=""text-align:center;"">
                    <a href=""#"" style=""display:inline-block; background:linear-gradient(135deg, #0f3460 0%, #1a1a2e 100%); color:#fff; text-decoration:none; padding:14px 40px; border-radius:8px; font-size:15px; font-weight:600; letter-spacing:0.5px;"">Start Shopping →</a>
                </div>
                
                <p style=""color:#999; font-size:13px; text-align:center; margin-top:24px;"">Thank you for choosing StoreWave!</p>";

            return BaseTemplate("Welcome", "#4CAF50", content);
        }

        public string OtpEmail(string customerName, string otp)
        {
            var content = $@"
                <div style=""text-align:center; margin-bottom:24px;"">
                    <div style=""display:inline-block; background:#fff3e0; border-radius:50%; width:64px; height:64px; line-height:64px; font-size:32px;"">🔐</div>
                </div>
                <h1 style=""color:#1a1a2e; font-size:24px; font-weight:700; text-align:center; margin:0 0 8px 0;"">Password Reset Request</h1>
                <p style=""color:#666; font-size:15px; text-align:center; margin:0 0 28px 0;"">Hi <strong style=""color:#1a1a2e;"">{customerName}</strong>, use the code below to reset your password.</p>
                
                <div style=""background:linear-gradient(135deg, #1a1a2e 0%, #0f3460 100%); border-radius:16px; padding:32px; text-align:center; margin-bottom:24px;"">
                    <p style=""color:rgba(255,255,255,0.7); font-size:12px; text-transform:uppercase; letter-spacing:3px; margin:0 0 12px 0;"">Your Verification Code</p>
                    <div style=""display:inline-block; background:rgba(255,255,255,0.1); border:2px dashed rgba(255,255,255,0.3); border-radius:12px; padding:16px 40px;"">
                        <span style=""font-size:36px; font-weight:800; color:#fff; letter-spacing:8px;"">{otp}</span>
                    </div>
                    <p style=""color:rgba(255,255,255,0.5); font-size:12px; margin:16px 0 0 0;"">⏱ This code expires in <strong style=""color:#FF9800;"">15 minutes</strong></p>
                </div>

                <div style=""background:#fff8e1; border-left:4px solid #FF9800; border-radius:0 8px 8px 0; padding:16px 20px; margin-bottom:20px;"">
                    <p style=""color:#e65100; font-size:13px; font-weight:600; margin:0 0 4px 0;"">⚠️ Security Notice</p>
                    <p style=""color:#bf360c; font-size:13px; margin:0; line-height:20px;"">If you didn't request this code, please ignore this email. Never share this code with anyone.</p>
                </div>

                <p style=""color:#999; font-size:13px; text-align:center;"">Need help? Contact our support team.</p>";

            return BaseTemplate("Password Reset", "#FF9800", content);
        }

        public string PasswordResetSuccessEmail(string customerName)
        {
            var content = $@"
                <div style=""text-align:center; margin-bottom:24px;"">
                    <div style=""display:inline-block; background:#e8f5e9; border-radius:50%; width:64px; height:64px; line-height:64px; font-size:32px;"">✅</div>
                </div>
                <h1 style=""color:#1a1a2e; font-size:24px; font-weight:700; text-align:center; margin:0 0 8px 0;"">Password Changed Successfully</h1>
                <p style=""color:#666; font-size:15px; text-align:center; margin:0 0 28px 0;"">Hi <strong style=""color:#1a1a2e;"">{customerName}</strong>, your password has been updated.</p>
                
                <div style=""background:linear-gradient(135deg, #e8f5e9 0%, #f1f8e9 100%); border-radius:12px; padding:24px; text-align:center; margin-bottom:24px;"">
                    <p style=""color:#2e7d32; font-size:14px; font-weight:600; margin:0 0 8px 0;"">🛡️ Your account is secure</p>
                    <p style=""color:#558b2f; font-size:13px; margin:0; line-height:20px;"">Your password was changed on {DateTime.UtcNow:MMMM dd, yyyy} at {DateTime.UtcNow:hh:mm tt} UTC.</p>
                </div>

                <div style=""background:#fce4ec; border-left:4px solid #e53935; border-radius:0 8px 8px 0; padding:16px 20px; margin-bottom:20px;"">
                    <p style=""color:#c62828; font-size:13px; font-weight:600; margin:0 0 4px 0;"">🚨 Didn't make this change?</p>
                    <p style=""color:#b71c1c; font-size:13px; margin:0; line-height:20px;"">If you did not change your password, please contact our support team immediately to secure your account.</p>
                </div>

                <div style=""text-align:center;"">
                    <a href=""#"" style=""display:inline-block; background:linear-gradient(135deg, #0f3460 0%, #1a1a2e 100%); color:#fff; text-decoration:none; padding:14px 40px; border-radius:8px; font-size:15px; font-weight:600;"">Login to Your Account</a>
                </div>";

            return BaseTemplate("Password Changed", "#4CAF50", content);
        }

        public string OrderConfirmationEmail(string customerName, string orderNumber, List<(string ProductName, int Quantity, decimal Price)> items, decimal total)
        {
            var itemRows = string.Join("", items.Select(item => $@"
                <tr>
                    <td style=""padding:12px 16px; border-bottom:1px solid #f0f0f0; color:#333; font-size:14px;"">{item.ProductName}</td>
                    <td style=""padding:12px 16px; border-bottom:1px solid #f0f0f0; color:#666; font-size:14px; text-align:center;"">{item.Quantity}</td>
                    <td style=""padding:12px 16px; border-bottom:1px solid #f0f0f0; color:#333; font-size:14px; text-align:right; font-weight:600;"">${item.Price:F2}</td>
                </tr>"));

            var content = $@"
                <div style=""text-align:center; margin-bottom:24px;"">
                    <div style=""display:inline-block; background:#e3f2fd; border-radius:50%; width:64px; height:64px; line-height:64px; font-size:32px;"">📦</div>
                </div>
                <h1 style=""color:#1a1a2e; font-size:24px; font-weight:700; text-align:center; margin:0 0 8px 0;"">Order Confirmed!</h1>
                <p style=""color:#666; font-size:15px; text-align:center; margin:0 0 8px 0;"">Thank you for your purchase, <strong style=""color:#1a1a2e;"">{customerName}</strong>!</p>
                <p style=""text-align:center; margin:0 0 28px 0;"">
                    <span style=""display:inline-block; background:#1a1a2e; color:#fff; border-radius:20px; padding:6px 18px; font-size:13px; font-weight:600; letter-spacing:1px;"">{orderNumber}</span>
                </p>

                <!-- Order Items Table -->
                <table role=""presentation"" width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""border-radius:12px; overflow:hidden; border:1px solid #e8e8e8; margin-bottom:20px;"">
                    <tr style=""background:linear-gradient(135deg, #1a1a2e 0%, #0f3460 100%);"">
                        <td style=""padding:12px 16px; color:#fff; font-size:13px; font-weight:600;"">Product</td>
                        <td style=""padding:12px 16px; color:#fff; font-size:13px; font-weight:600; text-align:center;"">Qty</td>
                        <td style=""padding:12px 16px; color:#fff; font-size:13px; font-weight:600; text-align:right;"">Price</td>
                    </tr>
                    {itemRows}
                </table>

                <!-- Total -->
                <div style=""background:linear-gradient(135deg, #f8f9ff 0%, #f0f4ff 100%); border-radius:12px; padding:20px; text-align:right; margin-bottom:24px;"">
                    <span style=""color:#666; font-size:14px;"">Total Amount: </span>
                    <span style=""color:#1a1a2e; font-size:24px; font-weight:800;"">${total:F2}</span>
                </div>

                <div style=""background:#e8f5e9; border-radius:12px; padding:16px 20px; text-align:center; margin-bottom:20px;"">
                    <p style=""color:#2e7d32; font-size:14px; margin:0;"">📬 We'll send you updates as your order progresses.</p>
                </div>

                <p style=""color:#999; font-size:13px; text-align:center;"">Thank you for shopping with StoreWave!</p>";

            return BaseTemplate("Order Confirmation", "#2196F3", content);
        }

        public string OrderStatusEmail(string customerName, string orderNumber, string newStatus, string statusMessage)
        {
            var (emoji, color, bgColor) = newStatus.ToLower() switch
            {
                "confirmed" => ("✅", "#4CAF50", "#e8f5e9"),
                "processing" => ("⚙️", "#2196F3", "#e3f2fd"),
                "shipped" => ("🚚", "#FF9800", "#fff3e0"),
                "delivered" => ("🎉", "#4CAF50", "#e8f5e9"),
                "cancelled" => ("❌", "#f44336", "#ffebee"),
                _ => ("📋", "#607D8B", "#eceff1")
            };

            var content = $@"
                <div style=""text-align:center; margin-bottom:24px;"">
                    <div style=""display:inline-block; background:{bgColor}; border-radius:50%; width:64px; height:64px; line-height:64px; font-size:32px;"">{emoji}</div>
                </div>
                <h1 style=""color:#1a1a2e; font-size:24px; font-weight:700; text-align:center; margin:0 0 8px 0;"">Order Status Update</h1>
                <p style=""color:#666; font-size:15px; text-align:center; margin:0 0 8px 0;"">Hi <strong style=""color:#1a1a2e;"">{customerName}</strong>, your order has been updated.</p>
                <p style=""text-align:center; margin:0 0 28px 0;"">
                    <span style=""display:inline-block; background:#1a1a2e; color:#fff; border-radius:20px; padding:6px 18px; font-size:13px; font-weight:600;"">{orderNumber}</span>
                </p>

                <div style=""background:{bgColor}; border-radius:16px; padding:28px; text-align:center; margin-bottom:24px;"">
                    <p style=""color:#666; font-size:12px; text-transform:uppercase; letter-spacing:2px; margin:0 0 8px 0;"">Current Status</p>
                    <div style=""display:inline-block; background:{color}; color:#fff; border-radius:24px; padding:10px 32px;"">
                        <span style=""font-size:18px; font-weight:700; letter-spacing:1px;"">{emoji} {newStatus}</span>
                    </div>
                </div>

                <div style=""background:#f8f9ff; border-radius:12px; padding:20px; margin-bottom:20px;"">
                    <p style=""color:#333; font-size:14px; line-height:22px; margin:0;"">{statusMessage}</p>
                </div>

                <p style=""color:#999; font-size:13px; text-align:center;"">Thank you for shopping with StoreWave!</p>";

            return BaseTemplate("Order Update", color, content);
        }

        // ─── Role-Specific Emails ───────────────────────────────────────

        public string AdminNewOrderEmail(string orderNumber, string customerName, decimal total, int itemCount)
        {
            var content = $@"
                <div style=""text-align:center; margin-bottom:24px;"">
                    <div style=""display:inline-block; background:#ede7f6; border-radius:50%; width:64px; height:64px; line-height:64px; font-size:32px;"">🔔</div>
                </div>
                <h1 style=""color:#1a1a2e; font-size:24px; font-weight:700; text-align:center; margin:0 0 8px 0;"">New Order Received</h1>
                <p style=""color:#666; font-size:15px; text-align:center; margin:0 0 28px 0;"">A new order has been placed on StoreWave.</p>

                <table role=""presentation"" width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""margin-bottom:24px;"">
                    <tr>
                        <td width=""50%"" style=""padding:8px;"">
                            <div style=""background:linear-gradient(135deg, #1a1a2e 0%, #0f3460 100%); border-radius:12px; padding:20px; text-align:center;"">
                                <p style=""color:rgba(255,255,255,0.6); font-size:11px; text-transform:uppercase; letter-spacing:2px; margin:0 0 6px 0;"">Order Number</p>
                                <p style=""color:#fff; font-size:16px; font-weight:700; margin:0;"">{orderNumber}</p>
                            </div>
                        </td>
                        <td width=""50%"" style=""padding:8px;"">
                            <div style=""background:linear-gradient(135deg, #4CAF50 0%, #2e7d32 100%); border-radius:12px; padding:20px; text-align:center;"">
                                <p style=""color:rgba(255,255,255,0.6); font-size:11px; text-transform:uppercase; letter-spacing:2px; margin:0 0 6px 0;"">Total Amount</p>
                                <p style=""color:#fff; font-size:16px; font-weight:700; margin:0;"">${total:F2}</p>
                            </div>
                        </td>
                    </tr>
                </table>

                <div style=""background:#f8f9ff; border-radius:12px; padding:20px; margin-bottom:20px;"">
                    <table role=""presentation"" width=""100%"" cellpadding=""0"" cellspacing=""0"">
                        <tr>
                            <td style=""padding:6px 0; color:#666; font-size:14px;"">👤 Customer:</td>
                            <td style=""padding:6px 0; color:#333; font-size:14px; font-weight:600; text-align:right;"">{customerName}</td>
                        </tr>
                        <tr>
                            <td style=""padding:6px 0; color:#666; font-size:14px;"">📦 Items:</td>
                            <td style=""padding:6px 0; color:#333; font-size:14px; font-weight:600; text-align:right;"">{itemCount} product(s)</td>
                        </tr>
                        <tr>
                            <td style=""padding:6px 0; color:#666; font-size:14px;"">🕐 Date:</td>
                            <td style=""padding:6px 0; color:#333; font-size:14px; font-weight:600; text-align:right;"">{DateTime.UtcNow:MMMM dd, yyyy hh:mm tt}</td>
                        </tr>
                    </table>
                </div>

                <p style=""color:#999; font-size:13px; text-align:center;"">Login to the Admin Dashboard to manage this order.</p>";

            return BaseTemplate("Admin Alert", "#7C4DFF", content);
        }

        public string AccountantOrderEmail(string orderNumber, decimal total, string paymentMethod, string customerName)
        {
            var content = $@"
                <div style=""text-align:center; margin-bottom:24px;"">
                    <div style=""display:inline-block; background:#fff8e1; border-radius:50%; width:64px; height:64px; line-height:64px; font-size:32px;"">💰</div>
                </div>
                <h1 style=""color:#1a1a2e; font-size:24px; font-weight:700; text-align:center; margin:0 0 8px 0;"">Financial Notification</h1>
                <p style=""color:#666; font-size:15px; text-align:center; margin:0 0 28px 0;"">A new transaction requires your attention.</p>

                <div style=""background:linear-gradient(135deg, #1a1a2e 0%, #0f3460 100%); border-radius:16px; padding:28px; text-align:center; margin-bottom:24px;"">
                    <p style=""color:rgba(255,255,255,0.6); font-size:12px; text-transform:uppercase; letter-spacing:3px; margin:0 0 8px 0;"">Transaction Amount</p>
                    <p style=""color:#4CAF50; font-size:36px; font-weight:800; margin:0;"">${total:F2}</p>
                </div>

                <div style=""background:#f8f9ff; border-radius:12px; padding:20px; margin-bottom:20px;"">
                    <table role=""presentation"" width=""100%"" cellpadding=""0"" cellspacing=""0"">
                        <tr>
                            <td style=""padding:8px 0; color:#666; font-size:14px; border-bottom:1px solid #eee;"">📋 Order Number:</td>
                            <td style=""padding:8px 0; color:#333; font-size:14px; font-weight:600; text-align:right; border-bottom:1px solid #eee;"">{orderNumber}</td>
                        </tr>
                        <tr>
                            <td style=""padding:8px 0; color:#666; font-size:14px; border-bottom:1px solid #eee;"">👤 Customer:</td>
                            <td style=""padding:8px 0; color:#333; font-size:14px; font-weight:600; text-align:right; border-bottom:1px solid #eee;"">{customerName}</td>
                        </tr>
                        <tr>
                            <td style=""padding:8px 0; color:#666; font-size:14px; border-bottom:1px solid #eee;"">💳 Payment Method:</td>
                            <td style=""padding:8px 0; color:#333; font-size:14px; font-weight:600; text-align:right; border-bottom:1px solid #eee;"">{paymentMethod}</td>
                        </tr>
                        <tr>
                            <td style=""padding:8px 0; color:#666; font-size:14px;"">📅 Date:</td>
                            <td style=""padding:8px 0; color:#333; font-size:14px; font-weight:600; text-align:right;"">{DateTime.UtcNow:MMMM dd, yyyy}</td>
                        </tr>
                    </table>
                </div>

                <p style=""color:#999; font-size:13px; text-align:center;"">Please review this transaction in the Financials Dashboard.</p>";

            return BaseTemplate("Financial Alert", "#FF9800", content);
        }

        public string SupplierOrderEmail(string supplierName, string orderNumber, List<(string ProductName, int Quantity, decimal Price)> items)
        {
            var itemRows = string.Join("", items.Select(item => $@"
                <tr>
                    <td style=""padding:12px 16px; border-bottom:1px solid #f0f0f0; color:#333; font-size:14px;"">{item.ProductName}</td>
                    <td style=""padding:12px 16px; border-bottom:1px solid #f0f0f0; color:#666; font-size:14px; text-align:center;"">{item.Quantity}</td>
                    <td style=""padding:12px 16px; border-bottom:1px solid #f0f0f0; color:#333; font-size:14px; text-align:right; font-weight:600;"">${item.Price:F2}</td>
                </tr>"));

            var supplierTotal = items.Sum(i => i.Price * i.Quantity);

            var content = $@"
                <div style=""text-align:center; margin-bottom:24px;"">
                    <div style=""display:inline-block; background:#e8f5e9; border-radius:50%; width:64px; height:64px; line-height:64px; font-size:32px;"">🏪</div>
                </div>
                <h1 style=""color:#1a1a2e; font-size:24px; font-weight:700; text-align:center; margin:0 0 8px 0;"">New Order for Your Products</h1>
                <p style=""color:#666; font-size:15px; text-align:center; margin:0 0 8px 0;"">Hello <strong style=""color:#1a1a2e;"">{supplierName}</strong>, your products have been ordered!</p>
                <p style=""text-align:center; margin:0 0 28px 0;"">
                    <span style=""display:inline-block; background:#1a1a2e; color:#fff; border-radius:20px; padding:6px 18px; font-size:13px; font-weight:600;"">{orderNumber}</span>
                </p>

                <!-- Product Items Table -->
                <table role=""presentation"" width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""border-radius:12px; overflow:hidden; border:1px solid #e8e8e8; margin-bottom:20px;"">
                    <tr style=""background:linear-gradient(135deg, #4CAF50 0%, #2e7d32 100%);"">
                        <td style=""padding:12px 16px; color:#fff; font-size:13px; font-weight:600;"">Product</td>
                        <td style=""padding:12px 16px; color:#fff; font-size:13px; font-weight:600; text-align:center;"">Qty</td>
                        <td style=""padding:12px 16px; color:#fff; font-size:13px; font-weight:600; text-align:right;"">Unit Price</td>
                    </tr>
                    {itemRows}
                </table>

                <div style=""background:#e8f5e9; border-radius:12px; padding:16px 20px; text-align:right; margin-bottom:20px;"">
                    <span style=""color:#666; font-size:14px;"">Your Revenue: </span>
                    <span style=""color:#2e7d32; font-size:22px; font-weight:800;"">${supplierTotal:F2}</span>
                </div>

                <p style=""color:#999; font-size:13px; text-align:center;"">Please prepare these items. Check the Supplier Dashboard for details.</p>";

            return BaseTemplate("Supplier Order", "#4CAF50", content);
        }

        public string WarehouseOrderEmail(string orderNumber, List<(string ProductName, int Quantity)> items, string shippingAddress)
        {
            var itemRows = string.Join("", items.Select((item, index) => $@"
                <tr style=""background:{(index % 2 == 0 ? "#fff" : "#fafafa")};"">
                    <td style=""padding:10px 16px; border-bottom:1px solid #f0f0f0; color:#333; font-size:14px;"">{item.ProductName}</td>
                    <td style=""padding:10px 16px; border-bottom:1px solid #f0f0f0; color:#333; font-size:14px; text-align:center; font-weight:700;"">{item.Quantity}x</td>
                </tr>"));

            var content = $@"
                <div style=""text-align:center; margin-bottom:24px;"">
                    <div style=""display:inline-block; background:#e3f2fd; border-radius:50%; width:64px; height:64px; line-height:64px; font-size:32px;"">🏭</div>
                </div>
                <h1 style=""color:#1a1a2e; font-size:24px; font-weight:700; text-align:center; margin:0 0 8px 0;"">Order Ready for Processing</h1>
                <p style=""color:#666; font-size:15px; text-align:center; margin:0 0 28px 0;"">A new order needs to be prepared for shipment.</p>

                <div style=""background:linear-gradient(135deg, #1a1a2e 0%, #0f3460 100%); border-radius:12px; padding:20px; text-align:center; margin-bottom:20px;"">
                    <p style=""color:rgba(255,255,255,0.6); font-size:11px; text-transform:uppercase; letter-spacing:2px; margin:0 0 6px 0;"">Order Number</p>
                    <p style=""color:#fff; font-size:20px; font-weight:700; margin:0;"">{orderNumber}</p>
                </div>

                <p style=""color:#1a1a2e; font-size:15px; font-weight:700; margin:0 0 8px 0;"">📋 Items to Pack:</p>
                <table role=""presentation"" width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""border-radius:12px; overflow:hidden; border:1px solid #e8e8e8; margin-bottom:20px;"">
                    <tr style=""background:linear-gradient(135deg, #0097a7 0%, #00838f 100%);"">
                        <td style=""padding:10px 16px; color:#fff; font-size:13px; font-weight:600;"">Product</td>
                        <td style=""padding:10px 16px; color:#fff; font-size:13px; font-weight:600; text-align:center;"">Quantity</td>
                    </tr>
                    {itemRows}
                </table>

                <div style=""background:#e0f2f1; border-radius:12px; padding:16px 20px; margin-bottom:20px;"">
                    <p style=""color:#00695c; font-size:13px; font-weight:600; margin:0 0 6px 0;"">📍 Shipping Address:</p>
                    <p style=""color:#004d40; font-size:14px; margin:0; line-height:22px;"">{shippingAddress}</p>
                </div>

                <p style=""color:#999; font-size:13px; text-align:center;"">Please process this order in the Warehouse Dashboard.</p>";

            return BaseTemplate("Warehouse Alert", "#00BCD4", content);
        }
    }
}
