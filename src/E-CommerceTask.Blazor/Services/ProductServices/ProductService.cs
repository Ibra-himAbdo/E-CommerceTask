namespace E_CommerceTask.Blazor.Services.ProductServices;

public class ProductService : IProductService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly HttpClient _httpClient;
    private readonly ApiSettings _apiSettings;

    public ProductService(
        IHttpClientFactory clientFactory,
        IOptions<ApiSettings> apiSettings,
        IOptions<JsonSerializerOptions> jsonOptions,
        IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
        _jsonOptions = jsonOptions.Value;
        _httpClient = clientFactory.CreateClient(apiSettings.Value.ApiName);
        _apiSettings = apiSettings.Value;
    }

    private string? GetJwtToken()
    {
        if (!_httpContextAccessor.HttpContext!.Request.Cookies.ContainsKey(BlazorConstants.AuthCookieName))
            return null;
        var token = _httpContextAccessor.HttpContext.Request.Cookies[BlazorConstants.AuthCookieName];
        return string.IsNullOrEmpty(token) ? null : token;
    }

    private void SetAuthorizationHeader()
    {
        var token = GetJwtToken();
        if (string.IsNullOrEmpty(token))
            return;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    public async Task<ServiceResponse<IEnumerable<Product>>> GetAllAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync(_apiSettings.Endpoints!.Product);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<ServiceResponse<IEnumerable<Product>>>(_jsonOptions)
                   ?? ServiceResponse<IEnumerable<Product>>.Failure("No products found");
        }
        catch (Exception ex)
        {
            return ServiceResponse<IEnumerable<Product>>.Failure($"Failed to get products: {ex.Message}");
        }
    }

    public async Task<ServiceResponse<Product>> GetByIdAsync(ObjectId id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_apiSettings.Endpoints!.Product}/{id.ToString()}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<ServiceResponse<Product>>(_jsonOptions)
                       ?? ServiceResponse<Product>.Failure("Product data was empty");
            }

            var errorResponse = await response.Content.ReadFromJsonAsync<ServiceResponse<Product>>(_jsonOptions);
            return errorResponse ?? ServiceResponse<Product>.Failure("Product not found");
        }
        catch (Exception ex)
        {
            return ServiceResponse<Product>.Failure($"Failed to get product: {ex.Message}");
        }
    }

    public async Task<ServiceResponse<Product>> CreateAsync(Product product)
    {
        try
        {
            SetAuthorizationHeader();
            var content = new StringContent(
                JsonSerializer.Serialize(product, _jsonOptions),
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PostAsync(_apiSettings.Endpoints!.Product, content);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<ServiceResponse<Product>>(_jsonOptions)
                       ?? ServiceResponse<Product>.Failure("Created product data was empty");
            }

            var errorResponse = await response.Content.ReadFromJsonAsync<ServiceResponse<Product>>(_jsonOptions);
            return errorResponse ?? ServiceResponse<Product>.Failure("Failed to create product");
        }
        catch (Exception ex)
        {
            return ServiceResponse<Product>.Failure($"Failed to create product: {ex.Message}");
        }
    }

    public async Task<ServiceResponse<bool>> UpdateAsync(Product product)
    {
        try
        {
            SetAuthorizationHeader();
            var content = new StringContent(
                JsonSerializer.Serialize(product, _jsonOptions),
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PutAsync(
                $"{_apiSettings.Endpoints!.Product}/{product.Id.ToString()}",
                content);

            if (response.IsSuccessStatusCode)
            {
                return ServiceResponse<bool>.Success(true, "Product updated successfully");
            }

            var errorResponse = await response.Content.ReadFromJsonAsync<ServiceResponse<bool>>(_jsonOptions);
            return errorResponse ?? ServiceResponse<bool>.Failure("Failed to update product");
        }
        catch (Exception ex)
        {
            return ServiceResponse<bool>.Failure($"Failed to update product: {ex.Message}");
        }
    }

    public async Task<ServiceResponse<bool>> DeleteAsync(ObjectId id)
    {
        try
        {
            SetAuthorizationHeader();
            var response = await _httpClient.DeleteAsync($"{_apiSettings.Endpoints!.Product}/{id.ToString()}");

            if (response.IsSuccessStatusCode)
            {
                return ServiceResponse<bool>.Success(true, "Product deleted successfully");
            }

            var errorResponse = await response.Content.ReadFromJsonAsync<ServiceResponse<bool>>(_jsonOptions);
            return errorResponse ?? ServiceResponse<bool>.Failure("Failed to delete product");
        }
        catch (Exception ex)
        {
            return ServiceResponse<bool>.Failure($"Failed to delete product: {ex.Message}");
        }
    }

    public async Task<ServiceResponse<IEnumerable<ProductCategory>>> GetCategoriesAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync(_apiSettings.Endpoints!.ProductCategory);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<ServiceResponse<IEnumerable<ProductCategory>>>(_jsonOptions)
                   ?? ServiceResponse<IEnumerable<ProductCategory>>.Failure("No categories found");
        }
        catch (Exception ex)
        {
            return ServiceResponse<IEnumerable<ProductCategory>>.Failure($"Failed to get categories: {ex.Message}");
        }
    }
}