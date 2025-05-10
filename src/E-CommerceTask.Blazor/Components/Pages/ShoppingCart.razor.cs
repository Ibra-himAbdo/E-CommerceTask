using E_CommerceTask.Shared.Models;

namespace E_CommerceTask.Blazor.Components.Pages;

public partial class ShoppingCart : ComponentBase
{
    private List<Product> _products = [];
    private bool _isAuthenticated;

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthStateProvider.GetAuthenticationStateAsync();
        _isAuthenticated = authState.User.Identity?.IsAuthenticated ?? false;
        _products = await CartService.GetCartItems();
    }
    public string Code { get; set; } = "DISCOUNT10";

    private async Task UseCode()
    {
        await CartService.UsePromoCode(Code);
        Console.WriteLine(Code);
    }

    private async Task AddItem(Product product, int quantity)
        => await CartService.AddItemToCart(product, quantity);

    private async Task RemoveItem(ObjectId productId)
        => await CartService.RemoveFromCart(productId);

    private async Task DeleteFromCart(ObjectId productId)
        => await CartService.DeleteFromCart(productId);

    private static string ImageUrl(string url)
        => string.IsNullOrEmpty(url) ? "https://fakeimg.pl/600x400" : url;
}