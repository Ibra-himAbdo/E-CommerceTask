namespace E_CommerceTask.Blazor.Components.Pages;

public partial class Home : ComponentBase
{
    private int _pageIndex = 1;
    private const int PageSize = 12;
    private List<Product> _products = [];
    private List<ProductCategory> categories = [];
    private int _totalCount = 0;

    // Filter state
    private ProductFilters _selectedFilter = ProductFilters.All;
    private IEnumerable<int> _selectedCategoryIds = [];

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        await LoadInitialData();
        await ApplyCombinedFilters();
    }

    private async Task LoadInitialData()
    {
        categories = await DbContext.Categories.ToListAsync();
    }

    private async Task ApplyCombinedFilters()
    {
        // Base query with includes
        IQueryable<Product> query = DbContext.Products
            .Include(p => p.Category);

        // Apply category filter if any categories are selected
        if (_selectedCategoryIds.Any() && _selectedCategoryIds.First() != 0)
        {
            query = query.Where(p => _selectedCategoryIds.Contains(p.CategoryId));
        }

        // Get total count BEFORE pagination for pagination controls
        _totalCount = await query.CountAsync();

        // Apply sorting BEFORE pagination
        query = _selectedFilter switch
        {
            ProductFilters.TopRated => query.OrderByDescending(p => p.Rate),
            ProductFilters.Newest => query.OrderByDescending(p => p.DateAdded),
            ProductFilters.Oldest => query.OrderBy(p => p.DateAdded),
            ProductFilters.PriceLowToHigh => query.OrderBy(p => p.Price),
            ProductFilters.PriceHighToLow => query.OrderByDescending(p => p.Price),
            _ => query.OrderBy(p => p.Id),
        };

        // Apply pagination LAST
        var pagedQuery = query
            .Skip((_pageIndex - 1) * PageSize)
            .Take(PageSize);

        _products = await pagedQuery.ToListAsync();
        StateHasChanged();
    }

    private async Task OnPageChanged(int newPage)
    {
        _pageIndex = newPage;
        await ApplyCombinedFilters();
    }

    // Your existing methods
    private async Task OnSelectedCategoriesChanged(IEnumerable<int> selectedCategoryIds)
    {
        _selectedCategoryIds = selectedCategoryIds;
        _pageIndex = 1; // Reset to first page when filters change
        await ApplyCombinedFilters();
    }

    private async Task OnSelectedFilterChanged(ProductFilters filter)
    {
        _selectedFilter = filter;
        _pageIndex = 1; // Reset to first page when sort changes
        await ApplyCombinedFilters();
    }
}