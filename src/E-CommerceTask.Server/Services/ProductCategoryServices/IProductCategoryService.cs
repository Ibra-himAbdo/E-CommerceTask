namespace E_CommerceTask.Server.Services.ProductCategoryServices;

public interface IProductCategoryService
{
    Task<ServiceResponse<IEnumerable<ProductCategory>>> GetAllAsync();
    Task<ServiceResponse<ProductCategory>> GetByIdAsync(ObjectId id);
    Task<ServiceResponse<ProductCategory>> CreateAsync(ProductCategory category);
    Task<ServiceResponse<bool>> UpdateAsync(ProductCategory category);
    Task<ServiceResponse<bool>> DeleteAsync(ObjectId id);
}