namespace E_CommerceTask.Blazor.Components.Pages.Components;

public partial class SearchComponent : ComponentBase
{
    private readonly bool _coerceText = true;
    private string _searchText = string.Empty;
    private IEnumerable<string> _productNames = [];
    private IEnumerable<Product> _allProducts = [];

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        var productsResponse = await ProductService.GetAllAsync();
        if (productsResponse.IsSuccess)
        {
            _allProducts = productsResponse.Data ?? [];
            _productNames = _allProducts
                .Select(p => p.Name)
                .Distinct()
                .OrderBy(x => x);
        }
    }

    private async Task<IEnumerable<string>> Search(string value, CancellationToken token)
    {
        await Task.Delay(5, token);
        return string.IsNullOrWhiteSpace(value)
            ? []
            : _productNames.Where(x => x.Contains(value, StringComparison.InvariantCultureIgnoreCase));
    }

    private async Task HandleKeyDown(KeyboardEventArgs args)
    {
        switch (args.Key)
        {
            case "Enter" when !string.IsNullOrWhiteSpace(_searchText):
                await ExecuteSearch(_searchText.ToUpper());
                _searchText = string.Empty;
                StateHasChanged();
                break;
            case "Escape":
                _searchText = string.Empty;
                StateHasChanged();
                break;
        }
    }

    private async Task ExecuteSearch(string name)
    {
        var matchingProducts = _allProducts
            .Where(p => p.NormalizedName.Contains(name.ToUpper()))
            .ToList();

        switch (matchingProducts.Count)
        {
            case 1:
                NavigationManager.NavigateTo($"/product/{matchingProducts[0].Id}");
                break;
            case > 1:
                NavigationManager.NavigateTo($"/search?query={Uri.EscapeDataString(_searchText)}");
                break;
            default:
                // Optional: Show "no results" message
                break;
        }

        _searchText = string.Empty;
        StateHasChanged();
    }
}