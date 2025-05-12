namespace E_CommerceTask.Blazor.Services.ProductServices;

public interface IProductService
{
    Task<ServiceResponse<IEnumerable<Product>>> GetAllAsync();
    Task<ServiceResponse<Product>> GetByIdAsync(ObjectId id);
    Task<ServiceResponse<bool>> CreateAsync(Product product);
    Task<ServiceResponse<bool>> UpdateAsync(Product product);
    Task<ServiceResponse<bool>> DeleteAsync(ObjectId id);
    Task<ServiceResponse<IEnumerable<ProductCategory>>> GetCategoriesAsync();
}