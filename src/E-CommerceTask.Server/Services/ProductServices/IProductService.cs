namespace E_CommerceTask.Server.Services.ProductServices;

public interface IProductService
{
    Task<ServiceResponse<IEnumerable<Product>>> GetAllAsync();
    Task<ServiceResponse<Product>> GetByIdAsync(ObjectId id);
    Task<ServiceResponse<Product>> CreateAsync(Product product);
    Task<ServiceResponse<bool>> UpdateAsync(Product product);
    Task<ServiceResponse<bool>> DeleteAsync(ObjectId id);
}