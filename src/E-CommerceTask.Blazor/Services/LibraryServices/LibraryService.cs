namespace E_CommerceTask.Blazor.Services.LibraryServices;

public class LibraryService : ILibraryService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly HttpClient _httpClient;
    private readonly ApiSettings _apiSettings;

    public LibraryService(
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
        return !_httpContextAccessor.HttpContext!.Request.Cookies.ContainsKey(BlazorConstants.AuthCookieName)
            ? null
            : _httpContextAccessor.HttpContext.Request.Cookies[BlazorConstants.AuthCookieName];
    }

    private void SetAuthorizationHeader()
    {
        var token = GetJwtToken();
        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }
    }

    public async Task<ServiceResponse<IEnumerable<Product>>> GetUserLibraryAsync()
    {
        try
        {
            SetAuthorizationHeader();
            var response = await _httpClient.GetAsync(_apiSettings.Endpoints!.Library);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<ServiceResponse<IEnumerable<Product>>>(_jsonOptions)
                       ?? ServiceResponse<IEnumerable<Product>>.Failure("Library data was empty");
            }

            var errorResponse =
                await response.Content.ReadFromJsonAsync<ServiceResponse<IEnumerable<Product>>>(_jsonOptions);
            return errorResponse ?? ServiceResponse<IEnumerable<Product>>.Failure("Failed to get user library");
        }
        catch (Exception ex)
        {
            return ServiceResponse<IEnumerable<Product>>.Failure($"Failed to get user library: {ex.Message}");
        }
    }

    public async Task<ServiceResponse<bool>> AddToLibraryAsync(ObjectId productId)
    {
        try
        {
            SetAuthorizationHeader();
            var response = await _httpClient.PostAsync(
                $"{_apiSettings.Endpoints!.Library}/add/{productId}",
                null);

            if (response.IsSuccessStatusCode)
            {
                return ServiceResponse<bool>.Success(true, "Product added to library");
            }

            var errorResponse = await response.Content.ReadFromJsonAsync<ServiceResponse<bool>>(_jsonOptions);
            return errorResponse ?? ServiceResponse<bool>.Failure("Failed to add product to library");
        }
        catch (Exception ex)
        {
            return ServiceResponse<bool>.Failure($"Failed to add product to library: {ex.Message}");
        }
    }

    public async Task<ServiceResponse<bool>> AddToLibraryAsync(List<ObjectId> productIds)
    {
        try
        {
            SetAuthorizationHeader();
            var content = new StringContent(
                JsonSerializer.Serialize(productIds, _jsonOptions),
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PostAsync(
                $"{_apiSettings.Endpoints!.Library}/add-multiple",
                content);

            if (response.IsSuccessStatusCode)
            {
                return ServiceResponse<bool>.Success(true, "Products added to library");
            }

            var errorResponse = await response.Content.ReadFromJsonAsync<ServiceResponse<bool>>(_jsonOptions);
            return errorResponse ?? ServiceResponse<bool>.Failure("Failed to add products to library");
        }
        catch (Exception ex)
        {
            return ServiceResponse<bool>.Failure($"Failed to add products to library: {ex.Message}");
        }
    }

    public async Task<ServiceResponse<string>> GetDownloadLinkAsync(ObjectId productId)
    {
        try
        {
            SetAuthorizationHeader();
            var response = await _httpClient.GetAsync(
                $"{_apiSettings.Endpoints!.Library}/download/{productId}");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<ServiceResponse<string>>(_jsonOptions)
                       ?? ServiceResponse<string>.Failure("Download link was empty");
            }

            var errorResponse = await response.Content.ReadFromJsonAsync<ServiceResponse<string>>(_jsonOptions);
            return errorResponse ?? ServiceResponse<string>.Failure("Failed to get download link");
        }
        catch (Exception ex)
        {
            return ServiceResponse<string>.Failure($"Failed to get download link: {ex.Message}");
        }
    }

    public async Task<ServiceResponse<bool>> IsInLibraryAsync(ObjectId productId)
    {
        try
        {
            SetAuthorizationHeader();
            var response = await _httpClient.GetAsync(
                $"{_apiSettings.Endpoints!.Library}/check/{productId}");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<ServiceResponse<bool>>(_jsonOptions)
                       ?? ServiceResponse<bool>.Failure("Library check response was empty");
            }

            var errorResponse = await response.Content.ReadFromJsonAsync<ServiceResponse<bool>>(_jsonOptions);
            return errorResponse ?? ServiceResponse<bool>.Failure("Failed to check library status");
        }
        catch (Exception ex)
        {
            return ServiceResponse<bool>.Failure($"Failed to check library status: {ex.Message}");
        }
    }
}