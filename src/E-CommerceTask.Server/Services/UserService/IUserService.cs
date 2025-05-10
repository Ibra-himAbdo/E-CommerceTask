namespace E_CommerceTask.Server.Services.UserService;

public interface IUserService
{
    Task<ServiceResponse<IEnumerable<User>>> GetAllAsync();
    Task<ServiceResponse<User>> GetByIdAsync(ObjectId id);
    Task<ServiceResponse<User>> GetByEmailAsync(string email);
    Task<ServiceResponse<User>> CreateAsync(User user);
    Task<ServiceResponse<bool>> UpdateAsync(User user);
    Task<ServiceResponse<bool>> DeleteAsync(ObjectId id);
}