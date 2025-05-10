global using E_CommerceTask.Shared.Models;
global using MongoDB.Bson;

namespace E_CommerceTask.Blazor.Services.CartServices;

public interface ICartService
{
    int TotalItems { get; }
    decimal TotalPrice { get; }
    decimal ShoppingFee { get; }
    event Action? OnChange;
    Task AddToCart(Product product);
    Task AddItemToCart(Product product, int quantity);
    Task RemoveFromCart(ObjectId productId);
    Task DeleteFromCart(ObjectId productId);
    Task ClearCart();
    Task<List<Product>> GetCartItems();
    Task<decimal> GetTotalPrice();
    Task<int> GetTotalItems();
    Task<bool> IsProductInCart(Product product);
    Task AddShoppingFee(ShippingMethod shippingMethod);
    Task UsePromoCode(string code);
}