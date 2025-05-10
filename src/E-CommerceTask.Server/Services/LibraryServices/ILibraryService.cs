namespace E_CommerceTask.Server.Services.LibraryServices;

public interface ILibraryService
{
    Task<ServiceResponse<IEnumerable<Product>>> GetUserLibraryAsync(ObjectId userId);
    Task<ServiceResponse<bool>> AddToLibraryAsync(ObjectId userId, ObjectId productId);
    Task<ServiceResponse<bool>> AddToLibraryAsync(ObjectId userId, List<ObjectId> productIds);
    Task<ServiceResponse<string>> GetDownloadLinkAsync(ObjectId productId, ObjectId userId);
    Task<ServiceResponse<bool>> IsInLibraryAsync(ObjectId productId, ObjectId userId);
}