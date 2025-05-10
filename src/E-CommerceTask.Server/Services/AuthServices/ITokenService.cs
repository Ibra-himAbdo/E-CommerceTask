namespace E_CommerceTask.Server.Services.AuthServices;

public interface ITokenService
{
    string GenerateToken(User user);
}