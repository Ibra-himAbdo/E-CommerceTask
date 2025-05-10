using E_CommerceTask.Shared.Models;

namespace E_CommerceTask.Blazor.Security;

public class JwtAuthHandler : AuthenticationHandler<JwtAuthHandlerOptions>
{
    public JwtAuthHandler(
        IOptionsMonitor<JwtAuthHandlerOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock) : base(options, logger, encoder, clock)
    {
    }

    public JwtAuthHandler(
        IOptionsMonitor<JwtAuthHandlerOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder) :
        base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        try
        {
            var token = Request.Cookies[BlazorConstants.AuthCookieName];
            if (string.IsNullOrEmpty(token))
                return Task.FromResult(AuthenticateResult.NoResult());

            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadJwtToken(token);
            var claims = jsonToken.Claims.Select(claim => new Claim(claim.Type, claim.Value))
                .ToList();
            var identity = new ClaimsIdentity(claims, "jwt");
            var user = new ClaimsPrincipal(identity);

            var ticket = new AuthenticationTicket(user, Scheme.Name);
            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return Task.FromResult(AuthenticateResult.NoResult());
        }
    }
    protected override Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        Response.Redirect("/login");
        return Task.CompletedTask;
    }

    protected override Task HandleForbiddenAsync(AuthenticationProperties properties)
    {
        Response.Redirect("/access-denied");
        return Task.CompletedTask;
    }
}

public class JwtAuthHandlerOptions : AuthenticationSchemeOptions;