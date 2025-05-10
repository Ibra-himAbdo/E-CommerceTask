namespace E_CommerceTask.Server.Services.UserService;

public class UserService(ECommerceDbContext dbContext) : IUserService
{
    public async Task<ServiceResponse<IEnumerable<User>>> GetAllAsync()
    {
        var users = await dbContext.Users.AsNoTracking().ToListAsync();
        return ServiceResponse<IEnumerable<User>>.Success(users);
    }

    public async Task<ServiceResponse<User>> GetByIdAsync(ObjectId id)
    {
        var user = await dbContext.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
        if (user == null)
        {
            return ServiceResponse<User>.Failure("User not found.");
        }
        return ServiceResponse<User>.Success(user);
    }

    public async Task<ServiceResponse<User>> GetByEmailAsync(string email)
    {
        var user = await dbContext.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email);
        if (user == null)
        {
            return ServiceResponse<User>.Failure("User not found.");
        }
        return ServiceResponse<User>.Success(user);
    }

    public async Task<ServiceResponse<User>> CreateAsync(User user)
    {
        try
        {
            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();
            return ServiceResponse<User>.Success(user, "User created successfully.");
        }
        catch (Exception ex)
        {
            return ServiceResponse<User>.Failure($"Failed to create user: {ex.Message}");
        }
    }

    public async Task<ServiceResponse<bool>> UpdateAsync(User user)
    {
        var existing = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == user.Id);
        if (existing == null)
        {
            return ServiceResponse<bool>.Failure("User not found.");
        }

        try
        {
            existing.Email = user.Email;
            existing.PasswordHash = user.PasswordHash;
            existing.Role = user.Role;

            dbContext.Users.Update(existing);
            await dbContext.SaveChangesAsync();
            return ServiceResponse<bool>.Success(true, "User updated successfully.");
        }
        catch (Exception ex)
        {
            return ServiceResponse<bool>.Failure($"Failed to update user: {ex.Message}");
        }
    }

    public async Task<ServiceResponse<bool>> DeleteAsync(ObjectId id)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (user == null)
        {
            return ServiceResponse<bool>.Failure("User not found.");
        }

        try
        {
            dbContext.Users.Remove(user);
            await dbContext.SaveChangesAsync();
            return ServiceResponse<bool>.Success(true, "User deleted successfully.");
        }
        catch (Exception ex)
        {
            return ServiceResponse<bool>.Failure($"Failed to delete user: {ex.Message}");
        }
    }
}