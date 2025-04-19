namespace E_CommerceTask.Blazor.Components.Pages;

public partial class Home : ComponentBase
{
    private List<Product> _products = [];

    protected override void OnInitialized()
    {
        // Dummy product list
        _products = Enumerable.Range(1, 6)
            .Select(i => new Product
            {
                Id = i,
                Name = $"Product {i}",
                Description = $"Description for Product {i}",
                Price = Math.Round((decimal)(Random.Shared.NextDouble() * 100), 2),
                ImageUrl = $"https://fakeimg.pl/{600 + i * 120}x{400 + i * 50}",
                Quantity = 1
            })
            .ToList();
    }

    private async Task AddItem(Product product, int quantity)
        => await CartService.AddItemToCart(product, quantity);

    private static string ImageUrl(string url)
        => string.IsNullOrEmpty(url) ? "https://fakeimg.pl/600x400" : url;

}