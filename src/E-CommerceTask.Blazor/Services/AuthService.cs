namespace E_CommerceTask.Blazor.Services;

public class AuthService(
    ApplicationDbContext db,
    IHttpContextAccessor httpContextAccessor,
    IJSRuntime jsRuntime) : AuthenticationStateProvider
{
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

    public async Task<bool> RegisterAsync(string email, string password)
    {
        if (db.Users.Any(u => u.Email == email))
            return false;

        var user = new User
        {
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            Role = "User"
        };

        db.Users.Add(user);
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<string?> LoginAsync(string email, string password)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Email.ToUpper() == email);
        if (user is null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            return null;

        const string secretKey = "YourSuperSecretKeyWithAtLeast16Characters";
        var keyBytes = Encoding.UTF8.GetBytes(secretKey);

        // Create claims
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        };

        // Create token
        var token = new JwtSecurityToken(
            issuer: "https://localhost:7128",
            audience: "https://localhost:7128",
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(keyBytes),
                SecurityAlgorithms.HmacSha256)
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task Logout()
    {
        await jsRuntime.InvokeVoidAsync("removeAuthCookie", BlazorConstants.AuthCookieName);
        NotifyAuthenticationStateChanged(
            Task.FromResult(new AuthenticationState(new ClaimsPrincipal())));
    }
}