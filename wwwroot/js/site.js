// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// ShopSmart JavaScript - Cart, SignalR Hubs, and Notifications

$(document).ready(function () {
    updateCartBadge();
    initializeSignalR();
});

// ==========================================
// CART BADGE FUNCTIONALITY
// ==========================================
function updateCartBadge() {
    $.get('/Cart/GetCartCount', function (data) {
        var badge = $('#cartBadge');
        badge.text(data.count);
        
        if (data.count > 0) {
            badge.show();
        } else {
            badge.hide();
        }
    }).fail(function() {
        // Silently fail if user is not authenticated
    });
}

// ==========================================
// SIGNALR HUB CONNECTIONS
// ==========================================
var orderConnection = null;
var notificationConnection = null;
var cartConnection = null;

function initializeSignalR() {
    // Initialize Order Hub (always connect for order notifications)
    initializeOrderHub();
    
    // Initialize Notification Hub (connect for all users)
    initializeNotificationHub();
    
    // Initialize Cart Hub only if user is authenticated
    if (document.body.dataset.userId) {
        initializeCartHub();
    }
}

// ==========================================
// ORDER HUB
// ==========================================
function initializeOrderHub() {
    orderConnection = new signalR.HubConnectionBuilder()
        .withUrl("/orderHub")
        .withAutomaticReconnect([0, 2000, 5000, 10000, 30000])
        .configureLogging(signalR.LogLevel.Warning)
        .build();

    // Receive order notification (admin toast)
    orderConnection.on("ReceiveOrderNotification", function (orderNum, total) {
        showOrderToast(orderNum, total);
    });

    // Receive order status update (for customers tracking orders)
    orderConnection.on("ReceiveOrderStatusUpdate", function (orderNumber, status, shippedDate, deliveredDate) {
        showNotification('info', 'Order Update', 
            `Your order ${orderNumber} status has been updated to: ${status}`);
        
        // Update UI if on orders page
        updateOrderStatusUI(orderNumber, status, shippedDate, deliveredDate);
    });

    // Receive new order for admin dashboard
    orderConnection.on("ReceiveNewOrderForAdmin", function (order) {
        showNotification('success', 'New Order!', 
            `Order ${order.orderNumber} received for $${order.totalAmount.toFixed(2)}`);
        
        // Refresh admin dashboard if on that page
        if (window.refreshAdminDashboard) {
            window.refreshAdminDashboard();
        }
    });

    // Connection state handlers
    orderConnection.onreconnecting(function() {
        console.log("Reconnecting to Order Hub...");
    });

    orderConnection.onreconnected(function() {
        console.log("Reconnected to Order Hub");
        joinOrderHubGroups();
    });

    orderConnection.onclose(function() {
        console.log("Disconnected from Order Hub");
    });

    // Start connection
    startOrderConnection();
}

function startOrderConnection() {
    orderConnection.start()
        .then(function() {
            console.log("Connected to Order Hub");
            joinOrderHubGroups();
        })
        .catch(function(err) {
            console.error("Order Hub connection failed: " + err.toString());
            // Retry after 5 seconds
            setTimeout(startOrderConnection, 5000);
        });
}

function joinOrderHubGroups() {
    // Join admin group if user is admin
    if (document.body.dataset.isAdmin === 'true') {
        orderConnection.invoke("JoinAdminGroup").catch(function(err) {
            console.error("Failed to join admin group: " + err.toString());
        });
    }
    
    // Join customer group if authenticated
    var userId = document.body.dataset.userId;
    if (userId) {
        orderConnection.invoke("JoinCustomerGroup", parseInt(userId)).catch(function(err) {
            console.error("Failed to join customer group: " + err.toString());
        });
    }
}

// Track a specific order
function trackOrder(orderNumber) {
    if (orderConnection && orderConnection.state === signalR.HubConnectionState.Connected) {
        orderConnection.invoke("TrackOrder", orderNumber).catch(function(err) {
            console.error("Failed to track order: " + err.toString());
        });
    }
}

// Stop tracking an order
function stopTrackingOrder(orderNumber) {
    if (orderConnection && orderConnection.state === signalR.HubConnectionState.Connected) {
        orderConnection.invoke("StopTrackingOrder", orderNumber).catch(function(err) {
            console.error("Failed to stop tracking order: " + err.toString());
        });
    }
}

// ==========================================
// NOTIFICATION HUB
// ==========================================
function initializeNotificationHub() {
    notificationConnection = new signalR.HubConnectionBuilder()
        .withUrl("/notificationHub")
        .withAutomaticReconnect([0, 2000, 5000, 10000, 30000])
        .configureLogging(signalR.LogLevel.Warning)
        .build();

    // General notification receiver
    notificationConnection.on("NotificationReceived", function (notification) {
        showNotification(notification.type, notification.title, notification.message);
    });

    // Low stock alert (admin only)
    notificationConnection.on("LowStockAlert", function (productId, productName, currentStock) {
        showNotification('warning', 'Low Stock Alert', 
            `${productName} is running low! Only ${currentStock} left in stock.`);
    });

    // New review notification
    notificationConnection.on("NewReviewReceived", function (productId, productName, rating, customerName) {
        var stars = '⭐'.repeat(rating);
        showNotification('info', 'New Review', 
            `${customerName} left a ${stars} review on ${productName}`);
    });

    // Back in stock notification
    notificationConnection.on("BackInStock", function (productId, productName) {
        showNotification('success', 'Back in Stock!', 
            `${productName} is now available!`);
    });

    // New product in category
    notificationConnection.on("NewProductInCategory", function (categoryId, productId, productName) {
        showNotification('info', 'New Arrival', 
            `Check out our new product: ${productName}`);
    });

    // Promotional alert
    notificationConnection.on("PromotionalAlert", function (title, message, couponCode) {
        var fullMessage = couponCode ? `${message} Use code: ${couponCode}` : message;
        showNotification('success', title, fullMessage);
    });

    // Connection state handlers
    notificationConnection.onreconnected(function() {
        console.log("Reconnected to Notification Hub");
        joinNotificationHubGroups();
    });

    // Start connection
    startNotificationConnection();
}

function startNotificationConnection() {
    notificationConnection.start()
        .then(function() {
            console.log("Connected to Notification Hub");
            joinNotificationHubGroups();
        })
        .catch(function(err) {
            console.error("Notification Hub connection failed: " + err.toString());
            setTimeout(startNotificationConnection, 5000);
        });
}

function joinNotificationHubGroups() {
    // Join admin notifications if admin
    if (document.body.dataset.isAdmin === 'true') {
        notificationConnection.invoke("JoinAdminNotifications").catch(function(err) {
            console.error("Failed to join admin notifications: " + err.toString());
        });
    }
    
    // Subscribe to promotions
    notificationConnection.invoke("SubscribeToPromotions").catch(function(err) {
        // Silent fail - promotions are optional
    });
}

// Watch a product for back-in-stock notifications
function watchProduct(productId) {
    if (notificationConnection && notificationConnection.state === signalR.HubConnectionState.Connected) {
        notificationConnection.invoke("WatchProduct", productId).catch(function(err) {
            console.error("Failed to watch product: " + err.toString());
        });
    }
}

// Stop watching a product
function stopWatchingProduct(productId) {
    if (notificationConnection && notificationConnection.state === signalR.HubConnectionState.Connected) {
        notificationConnection.invoke("StopWatchingProduct", productId).catch(function(err) {
            console.error("Failed to stop watching product: " + err.toString());
        });
    }
}

// ==========================================
// CART HUB
// ==========================================
function initializeCartHub() {
    cartConnection = new signalR.HubConnectionBuilder()
        .withUrl("/cartHub")
        .withAutomaticReconnect([0, 2000, 5000, 10000, 30000])
        .configureLogging(signalR.LogLevel.Warning)
        .build();

    // Cart item added on another device/tab
    cartConnection.on("CartItemAdded", function (data) {
        showNotification('info', 'Cart Updated', 
            `${data.productName} was added to your cart from another device.`);
        updateCartBadge();
        refreshCartIfVisible();
    });

    // Cart item removed on another device/tab
    cartConnection.on("CartItemRemoved", function (data) {
        showNotification('info', 'Cart Updated', 
            'An item was removed from your cart on another device.');
        updateCartBadge();
        refreshCartIfVisible();
    });

    // Cart quantity updated on another device/tab
    cartConnection.on("CartQuantityUpdated", function (data) {
        updateCartBadge();
        refreshCartIfVisible();
    });

    // Cart cleared on another device/tab
    cartConnection.on("CartCleared", function (data) {
        showNotification('info', 'Cart Cleared', 
            'Your cart was cleared on another device.');
        updateCartBadge();
        refreshCartIfVisible();
    });

    // Cart sync required
    cartConnection.on("CartSyncRequired", function (data) {
        updateCartBadge();
        refreshCartIfVisible();
    });

    // Full cart update from server
    cartConnection.on("CartUpdated", function (data) {
        updateCartBadge();
        if (window.updateCartDisplay) {
            window.updateCartDisplay(data);
        }
    });

    // Connection state handlers
    cartConnection.onreconnected(function() {
        console.log("Reconnected to Cart Hub");
        joinCartHubGroups();
    });

    // Start connection
    startCartConnection();
}

function startCartConnection() {
    cartConnection.start()
        .then(function() {
            console.log("Connected to Cart Hub");
            joinCartHubGroups();
        })
        .catch(function(err) {
            console.error("Cart Hub connection failed: " + err.toString());
            setTimeout(startCartConnection, 5000);
        });
}

function joinCartHubGroups() {
    var userId = document.body.dataset.userId;
    if (userId) {
        cartConnection.invoke("JoinCartGroup", parseInt(userId)).catch(function(err) {
            console.error("Failed to join cart group: " + err.toString());
        });
    }
}

// Notify cart item added (call after successful add to cart)
function notifyCartItemAdded(productId, productName, quantity, newCartCount) {
    var userId = document.body.dataset.userId;
    if (cartConnection && cartConnection.state === signalR.HubConnectionState.Connected && userId) {
        cartConnection.invoke("NotifyItemAdded", parseInt(userId), productId, productName, quantity, newCartCount)
            .catch(function(err) {
                console.error("Failed to notify cart item added: " + err.toString());
            });
    }
}

// Notify cart item removed
function notifyCartItemRemoved(productId, newCartCount) {
    var userId = document.body.dataset.userId;
    if (cartConnection && cartConnection.state === signalR.HubConnectionState.Connected && userId) {
        cartConnection.invoke("NotifyItemRemoved", parseInt(userId), productId, newCartCount)
            .catch(function(err) {
                console.error("Failed to notify cart item removed: " + err.toString());
            });
    }
}

// Notify cart quantity updated
function notifyCartQuantityUpdated(productId, newQuantity, newCartCount) {
    var userId = document.body.dataset.userId;
    if (cartConnection && cartConnection.state === signalR.HubConnectionState.Connected && userId) {
        cartConnection.invoke("NotifyQuantityUpdated", parseInt(userId), productId, newQuantity, newCartCount)
            .catch(function(err) {
                console.error("Failed to notify cart quantity updated: " + err.toString());
            });
    }
}

// Notify cart cleared
function notifyCartCleared() {
    var userId = document.body.dataset.userId;
    if (cartConnection && cartConnection.state === signalR.HubConnectionState.Connected && userId) {
        cartConnection.invoke("NotifyCartCleared", parseInt(userId))
            .catch(function(err) {
                console.error("Failed to notify cart cleared: " + err.toString());
            });
    }
}

// Refresh cart display if on cart page
function refreshCartIfVisible() {
    if (window.refreshCart) {
        window.refreshCart();
    }
}

// ==========================================
// UI HELPERS
// ==========================================

// Show order toast (legacy support)
function showOrderToast(orderNum, total) {
    var toastOrderNum = document.getElementById("toastOrderNum");
    var toastTotal = document.getElementById("toastTotal");
    
    if (toastOrderNum && toastTotal) {
        toastOrderNum.innerText = orderNum;
        toastTotal.innerText = "$" + total.toFixed(2);
        
        var toastEl = document.getElementById('orderToast');
        var toast = new bootstrap.Toast(toastEl);
        toast.show();
    } else {
        // Fallback to notification
        showNotification('success', 'New Order!', 
            `Order ${orderNum} received for $${total.toFixed(2)}`);
    }
}

// Generic notification display
function showNotification(type, title, message) {
    // Check if notification container exists, create if not
    var container = document.getElementById('notification-container');
    if (!container) {
        container = document.createElement('div');
        container.id = 'notification-container';
        container.style.cssText = 'position: fixed; top: 20px; right: 20px; z-index: 9999; max-width: 350px;';
        document.body.appendChild(container);
    }

    // Create notification element
    var notification = document.createElement('div');
    notification.className = 'toast show';
    notification.setAttribute('role', 'alert');
    notification.style.cssText = 'margin-bottom: 10px;';
    
    var bgColor = type === 'success' ? 'bg-success' : 
                  type === 'warning' ? 'bg-warning' : 
                  type === 'error' ? 'bg-danger' : 'bg-info';
    var textColor = type === 'warning' ? 'text-dark' : 'text-white';
    
    notification.innerHTML = `
        <div class="toast-header ${bgColor} ${textColor}">
            <strong class="me-auto">${escapeHtml(title)}</strong>
            <small>Just now</small>
            <button type="button" class="btn-close btn-close-white" data-bs-dismiss="toast"></button>
        </div>
        <div class="toast-body">
            ${escapeHtml(message)}
        </div>
    `;
    
    container.appendChild(notification);
    
    // Initialize Bootstrap toast
    var toast = new bootstrap.Toast(notification, { delay: 5000 });
    toast.show();
    
    // Remove from DOM after hidden
    notification.addEventListener('hidden.bs.toast', function() {
        notification.remove();
    });
}

// Escape HTML to prevent XSS
function escapeHtml(text) {
    var div = document.createElement('div');
    div.textContent = text;
    return div.innerHTML;
}

// Update order status in UI (for order tracking pages)
function updateOrderStatusUI(orderNumber, status, shippedDate, deliveredDate) {
    var statusBadge = document.querySelector(`[data-order-number="${orderNumber}"] .order-status`);
    if (statusBadge) {
        statusBadge.textContent = status;
        statusBadge.className = 'badge order-status ' + getStatusBadgeClass(status);
    }
    
    // Update dates if displayed
    if (shippedDate) {
        var shippedEl = document.querySelector(`[data-order-number="${orderNumber}"] .shipped-date`);
        if (shippedEl) {
            shippedEl.textContent = new Date(shippedDate).toLocaleDateString();
        }
    }
    
    if (deliveredDate) {
        var deliveredEl = document.querySelector(`[data-order-number="${orderNumber}"] .delivered-date`);
        if (deliveredEl) {
            deliveredEl.textContent = new Date(deliveredDate).toLocaleDateString();
        }
    }
}

function getStatusBadgeClass(status) {
    switch (status.toLowerCase()) {
        case 'pending': return 'bg-warning text-dark';
        case 'confirmed': return 'bg-info';
        case 'processing': return 'bg-primary';
        case 'shipped': return 'bg-secondary';
        case 'delivered': return 'bg-success';
        case 'cancelled': return 'bg-danger';
        default: return 'bg-secondary';
    }
}

// Export functions for use in views
window.ShopSmart = {
    updateCartBadge: updateCartBadge,
    trackOrder: trackOrder,
    stopTrackingOrder: stopTrackingOrder,
    watchProduct: watchProduct,
    stopWatchingProduct: stopWatchingProduct,
    notifyCartItemAdded: notifyCartItemAdded,
    notifyCartItemRemoved: notifyCartItemRemoved,
    notifyCartQuantityUpdated: notifyCartQuantityUpdated,
    notifyCartCleared: notifyCartCleared,
    showNotification: showNotification
};
