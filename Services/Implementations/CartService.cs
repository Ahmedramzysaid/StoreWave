using AutoMapper;
using StoreWave.DTOs;
using StoreWave.Models.Entities;
using StoreWave.Services.Interfaces;
using StoreWave.UnitOfWork;

namespace StoreWave.Services.Implementations
{
    public class CartService : ICartService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CartService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<CartDto> GetCartAsync(int customerId)
        {
            var cartItems = await _unitOfWork.CartItems.GetCartItemsByCustomerAsync(customerId);
            
            var cartDto = new CartDto
            {
                Items = _mapper.Map<List<CartItemDto>>(cartItems)
            };
            
            cartDto.SubTotal = cartDto.Items.Sum(i => i.TotalPrice);
            
            return cartDto;
        }

        public async Task AddItemToCartAsync(int customerId, int productId, int quantity)
        {
            var existingItem = await _unitOfWork.CartItems.GetCartItemAsync(customerId, productId);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
                _unitOfWork.CartItems.Update(existingItem);
            }
            else
            {
                var newItem = new CartItem
                {
                    CustomerId = customerId,
                    ProductId = productId,
                    Quantity = quantity,
                    AddedAt = DateTime.UtcNow
                };
                await _unitOfWork.CartItems.AddAsync(newItem);
            }

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task RemoveItemFromCartAsync(int customerId, int productId)
        {
            var item = await _unitOfWork.CartItems.GetCartItemAsync(customerId, productId);
            if (item != null)
            {
                _unitOfWork.CartItems.Delete(item);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task UpdateItemQuantityAsync(int customerId, int productId, int quantity)
        {
            var item = await _unitOfWork.CartItems.GetCartItemAsync(customerId, productId);
            if (item != null)
            {
                if (quantity <= 0)
                {
                    _unitOfWork.CartItems.Delete(item);
                }
                else
                {
                    item.Quantity = quantity;
                    _unitOfWork.CartItems.Update(item);
                }
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task ClearCartAsync(int customerId)
        {
            await _unitOfWork.CartItems.ClearCartAsync(customerId);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<int> GetCartItemCountAsync(int customerId)
        {
            return await _unitOfWork.CartItems.GetCartItemCountAsync(customerId);
        }
    }
}
