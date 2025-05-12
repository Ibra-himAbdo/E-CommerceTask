using E_CommerceTask.Shared.Models;

namespace E_CommerceTask.Blazor.Services.LibraryServices;

public interface ILibraryService
{
    Task<ServiceResponse<IEnumerable<Product>>> GetUserLibraryAsync();
    Task<ServiceResponse<bool>> AddToLibraryAsync(ObjectId productId);
    Task<ServiceResponse<bool>> AddToLibraryAsync(List<ObjectId> productIds);
    Task<ServiceResponse<string>> GetDownloadLinkAsync(ObjectId productId);
    Task<ServiceResponse<bool>> IsInLibraryAsync(ObjectId productId);
}