namespace E_CommerceTask.Server.Services.AuthServices;

public interface IAuthService
{
    Task<ServiceResponse<AuthResponseDto>> RegisterAsync(RegisterModel registerDto);
    Task<ServiceResponse<AuthResponseDto>> LoginAsync(LoginModel loginDto);
}