namespace E_CommerceTask.Blazor.Components.Pages.Components;

public partial class SearchComponent : ComponentBase
{
    private readonly bool _coerceText = true;
    private string _searchText = string.Empty;
    private List<string> _products = [];

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        _products = await DbContext.Products
            .Select(p => p.Name)
            .ToListAsync();
    }

    private async Task<IEnumerable<string>> Search(string value, CancellationToken token)
    {
        await Task.Delay(5, token);
        if (string.IsNullOrWhiteSpace(value))
            return [];
        return _products.Where(x => x.Contains(value, StringComparison.InvariantCultureIgnoreCase));
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
        var matchingProducts = await DbContext.Products
            .Where(p => p.NormalizedName.Contains(name))
            .ToListAsync();

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