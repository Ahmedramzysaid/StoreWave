using AutoMapper;
using StoreWave.DTOs;
using StoreWave.Models.Entities;

namespace StoreWave.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Category Mappings
            CreateMap<Category, CategoryDto>()
                .ForMember(dest => dest.ProductCount, opt => opt.MapFrom(src => src.Products.Count));
            CreateMap<CategoryDto, Category>();

            // Product Mappings
            CreateMap<Product, ProductDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.SupplierName, opt => opt.MapFrom(src => src.Supplier != null ? src.Supplier.FullName : null))
                .ForMember(dest => dest.AverageRating, opt => opt.MapFrom(src => src.Reviews.Any() ? src.Reviews.Average(r => r.Rating) : 0))
                .ForMember(dest => dest.ReviewCount, opt => opt.MapFrom(src => src.Reviews.Count))
                .ForMember(dest => dest.Reviews, opt => opt.MapFrom(src => src.Reviews.Where(r => r.IsApproved)));
            CreateMap<ProductDto, Product>();

            // Customer Mappings
            CreateMap<Customer, CustomerDto>()
                .ForMember(dest => dest.TotalOrders, opt => opt.MapFrom(src => src.Orders.Count));
            CreateMap<CustomerDto, Customer>();

            // Order Mappings
            CreateMap<Order, OrderDto>()
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer.FullName))
                .ForMember(dest => dest.DriverName, opt => opt.MapFrom(src => src.Driver != null ? src.Driver.FullName : null))
                .ForMember(dest => dest.ItemCount, opt => opt.MapFrom(src => src.OrderItems.Count));
            CreateMap<OrderDto, Order>();

            // OrderItem Mappings
            CreateMap<OrderItem, OrderItemDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.ProductImageUrl, opt => opt.MapFrom(src => src.Product.ImageUrl));
            CreateMap<OrderItemDto, OrderItem>();

            // CartItem Mappings
            CreateMap<CartItem, CartItemDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.ProductImageUrl, opt => opt.MapFrom(src => src.Product.ImageUrl))
                .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.Product.DiscountPrice ?? src.Product.Price))
                .ForMember(dest => dest.StockQuantity, opt => opt.MapFrom(src => src.Product.StockQuantity));
            CreateMap<CartItemDto, CartItem>();

            // Review Mappings
            CreateMap<Review, ReviewDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer.FullName));
            CreateMap<ReviewDto, Review>();
        }
    }
}
