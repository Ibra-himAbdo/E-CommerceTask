namespace E_CommerceTask.Blazor.Services.LibraryServices;

public interface ILibraryService
{
    Task<List<Product>> GetUserLibraryAsync(string userId);
    Task<bool> AddToLibraryAsync(string userId, int productId);
    Task<bool> AddToLibraryAsync(string userId, List<int> productIds);
    Task<string> GetDownloadLinkAsync(int productId, string userId);
    Task<bool> IsInLibraryAsync(int productId, string userId);
}