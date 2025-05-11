namespace E_CommerceTask.Blazor.Components.Pages;

public partial class Home : ComponentBase
{
    private int _pageIndex = 1;
    private const int PageSize = 12;
    private IEnumerable<Product> _products = [];
    private IEnumerable<Product> _filteredProducts = [];
    private IEnumerable<ProductCategory> _categories = [];
    private int _totalCount = 0;

    // Filter state
    private ProductFilters _selectedFilter = ProductFilters.All;
    private IEnumerable<ObjectId> _selectedCategoryIds = [];

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        await LoadInitialData();
        await ApplyCombinedFilters();
    }

    private async Task LoadInitialData()
    {
        var categoryResponse = await ProductService.GetCategoriesAsync();
        var productsResponse = await ProductService.GetAllAsync();
        if (categoryResponse.IsSuccess && productsResponse.IsSuccess)
        {
            _categories = categoryResponse.Data ?? [];
            _products = productsResponse.Data ?? [];
        }
        else
        {
            Console.WriteLine($"Error loading categories: {categoryResponse.Message}");
            Console.WriteLine($"Error loading products: {productsResponse.Message}");
        }

    }

    private Task ApplyCombinedFilters()
    {
        // Start with all products
        var query = _products.AsQueryable();

        // Apply category filter if any categories are selected
        if (_selectedCategoryIds.Any() && !_selectedCategoryIds.Contains(ObjectId.Empty))
        {
            query = query.Where(p => _selectedCategoryIds.Contains(p.CategoryId));
        }

        // Apply sorting
        query = _selectedFilter switch
        {
            ProductFilters.TopRated => query.OrderByDescending(p => p.Rate),
            ProductFilters.Newest => query.OrderByDescending(p => p.DateAdded),
            ProductFilters.Oldest => query.OrderBy(p => p.DateAdded),
            ProductFilters.PriceLowToHigh => query.OrderBy(p => p.Price),
            ProductFilters.PriceHighToLow => query.OrderByDescending(p => p.Price),
            _ => query.OrderBy(p => p.Id),
        };

        // Get total count BEFORE pagination
        _totalCount = query.Count();

        // Apply pagination
        _filteredProducts = query
            .Skip((_pageIndex - 1) * PageSize)
            .Take(PageSize)
            .ToList(); // Materialize the query

        StateHasChanged();
        return Task.CompletedTask;
    }

    private async Task OnPageChanged(int newPage)
    {
        _pageIndex = newPage;
        await ApplyCombinedFilters();
    }

    // Your existing methods
    private async Task OnSelectedCategoriesChanged(IEnumerable<ObjectId> selectedCategoryIds)
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