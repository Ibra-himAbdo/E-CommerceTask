namespace E_CommerceTask.Server.Services.AuthServices;

public class AuthService(
    IUserService userService,
    ITokenService tokenService) : IAuthService
{
    public async Task<ServiceResponse<AuthResponseDto>> RegisterAsync(RegisterModel registerDto)
    {
        var existingUserResponse = await userService.GetByEmailAsync(registerDto.Email);
        if (existingUserResponse.IsSuccess)
        {
            return ServiceResponse<AuthResponseDto>.Failure("User already exists.");
        }

        var user = new User
        {
            Email = registerDto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
            Role = "User",
            CreatedAt = DateTime.UtcNow
        };

        var createResponse = await userService.CreateAsync(user);
        if (!createResponse.IsSuccess)
        {
            return ServiceResponse<AuthResponseDto>.Failure(createResponse.Message);
        }

        var token = tokenService.GenerateToken(user);
        return ServiceResponse<AuthResponseDto>.Success(
            new AuthResponseDto(token, user.Id.ToString(), user.Email, user.Role),
            "Registration successful");
    }

    public async Task<ServiceResponse<AuthResponseDto>> LoginAsync(LoginModel loginDto)
    {
        var userResponse = await userService.GetByEmailAsync(loginDto.Email);
        if (!userResponse.IsSuccess ||
            !BCrypt.Net.BCrypt.Verify(loginDto.Password, userResponse.Data!.PasswordHash))
        {
            return ServiceResponse<AuthResponseDto>.Failure("Invalid credentials");
        }

        var token = tokenService.GenerateToken(userResponse.Data);
        return ServiceResponse<AuthResponseDto>.Success(
            new AuthResponseDto(token, userResponse.Data.Id.ToString(),
                userResponse.Data.Email, userResponse.Data.Role),
            "Login successful");
    }
}