using E_CommerceTask.Shared.Models;

namespace E_CommerceTask.Blazor.Components.Pages.Components;

public partial class ProductCard : ComponentBase
{
    [Parameter] public Product? Product { get; set; }
    private bool _isAuthenticated;

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthStateProvider.GetAuthenticationStateAsync();
        _isAuthenticated = authState.User.Identity?.IsAuthenticated ?? false;
    }

    private async Task AddItem(Product product, int quantity)
    {
        if (!_isAuthenticated)
        {
            Snackbar.Add("Please login to add items to your cart.", Severity.Warning);
            return;
        }

        await CartService.AddItemToCart(product, quantity);
    }

    private Task DeleteProduct(ObjectId productId)
    {
        var parameters = new DialogParameters<DeleteProductDialog>
        {
            { x => x.Id, productId }
        };
        var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall };
        StateHasChanged();
        return DialogService.ShowAsync<DeleteProductDialog>("Delete Product", parameters, options);
    }

    private static string ImageUrl(string url)
        => string.IsNullOrEmpty(url) ? "https://fakeimg.pl/600x400" : url;
}