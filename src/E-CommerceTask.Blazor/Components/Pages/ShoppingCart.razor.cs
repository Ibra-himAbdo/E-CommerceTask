namespace E_CommerceTask.Blazor.Components.Pages;

public partial class ShoppingCart : ComponentBase
{
    private List<Product> _products = [];

    protected override async Task OnInitializedAsync()
    {
        _products = await CartService.GetCartItems();
    }

    private async Task AddItem(Product product, int quantity)
        => await CartService.AddItemToCart(product, quantity);

    private async Task RemoveItem(int productId)
        => await CartService.RemoveFromCart(productId);

    private async Task ClearCart()
        => await CartService.ClearCart();

    private static string ImageUrl(string url)
        => string.IsNullOrEmpty(url) ? "https://fakeimg.pl/600x400" : url;
}