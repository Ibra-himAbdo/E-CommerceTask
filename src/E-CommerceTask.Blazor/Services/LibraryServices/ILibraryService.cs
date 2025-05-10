using E_CommerceTask.Shared.Models;

namespace E_CommerceTask.Blazor.Services.LibraryServices;

public interface ILibraryService
{
    Task<List<Product>> GetUserLibraryAsync(ObjectId userId);
    Task<bool> AddToLibraryAsync(ObjectId userId, ObjectId productId);
    Task<bool> AddToLibraryAsync(ObjectId userId, List<ObjectId> productIds);
    Task<string> GetDownloadLinkAsync(ObjectId productId, ObjectId userId);
    Task<bool> IsInLibraryAsync(ObjectId productId, ObjectId userId);
}