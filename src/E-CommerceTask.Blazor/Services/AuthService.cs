namespace E_CommerceTask.Blazor.Services;

public class AuthService(
    IHttpContextAccessor httpContextAccessor,
    IJSRuntime jsRuntime,
    IHttpClientFactory clientFactory,
    IOptions<ApiSettings> apiSettings) : AuthenticationStateProvider
{
    private readonly HttpClient _httpClient = clientFactory.CreateClient(apiSettings.Value.ApiName);
    private readonly ApiSettings _apiSettings = apiSettings.Value;

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            if (!httpContextAccessor.HttpContext!.Request.Cookies.ContainsKey(BlazorConstants.AuthCookieName))
                await MakeAsUnauthenticated();

            var token = httpContextAccessor.HttpContext.Request.Cookies[BlazorConstants.AuthCookieName];
            if (string.IsNullOrEmpty(token))
                return await MakeAsUnauthenticated();

            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadJwtToken(token);
            var claims = jsonToken.Claims.Select(claim => new Claim(claim.Type, claim.Value))
                .ToList();
            var identity = new ClaimsIdentity(claims, "jwt");
            var user = new ClaimsPrincipal(identity);
            return await Task.FromResult(new AuthenticationState(user));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return await MakeAsUnauthenticated();
        }
    }

    private Task<AuthenticationState> MakeAsUnauthenticated()
    {
        try
        {
            var state = new AuthenticationState(new ClaimsPrincipal());
            NotifyAuthenticationStateChanged(Task.FromResult(state));
            return Task.FromResult(state);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));
        }
    }

    public async Task<ServiceResponse<bool>> RegisterAsync(string email, string password, string confirmPassword)
    {
        var registerDto = new RegisterModel(email, password, confirmPassword);
        var response = await _httpClient.PostAsJsonAsync(_apiSettings.Endpoints?.Register, registerDto);

        if (response.IsSuccessStatusCode) return ServiceResponse<bool>.Success(true, "Registration successful");

        var errorResponse = await response.Content.ReadFromJsonAsync<ServiceResponse<AuthResponseDto>>();
        return ServiceResponse<bool>.Failure(errorResponse?.Message ?? "Registration failed");

    }

    public async Task<ServiceResponse<AuthResponseDto>> LoginAsync(string email, string password)
    {
        var loginDto = new LoginModel(email, password);
        var response = await _httpClient.PostAsJsonAsync(_apiSettings.Endpoints?.Login, loginDto);

        if (!response.IsSuccessStatusCode)
        {
            var errorResponse = await response.Content.ReadFromJsonAsync<ServiceResponse<AuthResponseDto>>();
            return errorResponse ?? ServiceResponse<AuthResponseDto>.Failure("Login failed");
        }

        var authResponse = await response.Content.ReadFromJsonAsync<ServiceResponse<AuthResponseDto>>();

        if (authResponse?.Data != null)
        {
            await jsRuntime.InvokeVoidAsync("setAuthCookie",
                BlazorConstants.AuthCookieName,
                authResponse.Data.Token,
                7); // 7 days expiration
        }

        return authResponse ?? ServiceResponse<AuthResponseDto>.Failure("Invalid response from server");
    }

    public async Task Logout()
    {
        await jsRuntime.InvokeVoidAsync("removeAuthCookie", BlazorConstants.AuthCookieName);
        NotifyAuthenticationStateChanged(
            Task.FromResult(new AuthenticationState(new ClaimsPrincipal())));
    }
}