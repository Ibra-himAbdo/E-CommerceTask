
using E_CommerceTask.Shared.DTOs;

namespace E_CommerceTask.Server.Controllers;

public class AuthController : BaseApiController
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<ServiceResponse<AuthResponseDto>>> Register(
        [FromBody] RegisterModel registerDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ServiceResponse<AuthResponseDto>.Failure("Invalid data"));
        }

        var response = await _authService.RegisterAsync(registerDto);

        return response.IsSuccess
            ? CreatedAtAction(nameof(Register), response)
            : BadRequest(response);
    }

    [HttpPost("login")]
    public async Task<ActionResult<ServiceResponse<AuthResponseDto>>> Login(
        [FromBody] LoginModel loginDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ServiceResponse<AuthResponseDto>.Failure("Invalid data"));
        }

        var response = await _authService.LoginAsync(loginDto);
        return response.IsSuccess ? Ok(response) : Unauthorized(response);
    }
}