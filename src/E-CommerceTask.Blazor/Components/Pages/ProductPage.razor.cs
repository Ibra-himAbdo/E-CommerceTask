namespace E_CommerceTask.Blazor.Components.Pages;

public partial class ProductPage : ComponentBase
{
    [Parameter] public string Id { get; set; }
    private Product? _product;
    private bool _isAuthenticated;
    private bool _isDownloading = false;
    private bool _isInLibrary = false;

    protected override async Task OnInitializedAsync()
    {
        await LoadAuthState();
    }

    protected override async Task OnParametersSetAsync()
    {
        var productServiceResponse = await ProductService.GetByIdAsync(ObjectId.Parse(Id));
        if (productServiceResponse.IsSuccess)
        {
            _product = productServiceResponse.Data;
        }
        else
        {
            Snackbar.Add("Product not found", Severity.Error);
            return;
        }

        if (_isAuthenticated && _product is not null)
        {
            var libraryResponse = await LibraryService.IsInLibraryAsync(_product.Id);
            _isInLibrary = libraryResponse.Data;
        }
    }

    private async Task LoadAuthState()
    {
        var authState = await AuthStateProvider.GetAuthenticationStateAsync();
        _isAuthenticated = authState.User.Identity?.IsAuthenticated ?? false;
    }

    private async Task AddToCart(Product product)
    {
        if (!_isAuthenticated)
        {
            Snackbar.Add("Please sign in to add items to your cart.", Severity.Warning);
            return;
        }

        await CartService.AddItemToCart(product, 1);
    }

    private async Task DownloadProduct()
    {
        _isDownloading = true;
        StateHasChanged();
        await Task.Delay(1500);
        _isDownloading = false;
        Snackbar.Add("Product downloaded successfully!", Severity.Success);
        StateHasChanged();
    }

    private static string ImageUrl(string url)
        => string.IsNullOrEmpty(url) ? "https://fakeimg.pl/600x400" : url;
}