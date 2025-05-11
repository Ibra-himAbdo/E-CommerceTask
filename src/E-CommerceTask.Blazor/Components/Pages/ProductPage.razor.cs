namespace E_CommerceTask.Blazor.Components.Pages;

public partial class ProductPage : ComponentBase
{
    [Parameter] public string Id { get; set; }
    private Product? _product;
    private string? _userId;
    private bool _isAuthenticated;
    private bool _isDownloading = false;
    private bool _isInLibrary = false;

    protected override async Task OnInitializedAsync()
    {
        await LoadAuthState();
    }

    protected override async Task OnParametersSetAsync()
    {
        var response = await ProductService.GetByIdAsync(ObjectId.Parse(Id));
        if (response.IsSuccess)
        {
            _product = response.Data;
        }
        else
        {
            Snackbar.Add("Product not found", Severity.Error);
            return;
        }

        if (_isAuthenticated && _product != null)
        {
            _isInLibrary = await LibraryService.IsInLibraryAsync(_product.Id, ObjectId.Parse(_userId!));
        }
    }

    private async Task LoadAuthState()
    {
        var authState = await AuthStateProvider.GetAuthenticationStateAsync();
        _isAuthenticated = authState.User.Identity?.IsAuthenticated ?? false;
        _userId = authState.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
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
        StateHasChanged();
    }

    private static string ImageUrl(string url)
        => string.IsNullOrEmpty(url) ? "https://fakeimg.pl/600x400" : url;

}