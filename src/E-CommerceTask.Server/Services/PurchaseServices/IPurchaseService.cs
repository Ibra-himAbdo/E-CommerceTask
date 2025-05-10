namespace E_CommerceTask.Server.Services.PurchaseServices;

public interface IPurchaseService
{
    Task<ServiceResponse<IEnumerable<Purchase>>> GetAllAsync();
    Task<ServiceResponse<Purchase>> GetByIdAsync(ObjectId id);
    Task<ServiceResponse<Purchase>> CreateAsync(Purchase purchase);
    Task<ServiceResponse<bool>> UpdateAsync(Purchase purchase);
    Task<ServiceResponse<bool>> DeleteAsync(ObjectId id);
}