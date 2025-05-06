namespace E_CommerceTask.Blazor.Services.LibraryServices;

public class LibraryService : ILibraryService
{
    private readonly ApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public LibraryService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<List<Product>> GetUserLibraryAsync(string userId)
    {
        return await _context.Purchases
            .Where(p => p.UserId == userId && p.IsPaid)
            .Include(p => p.Product)
            .ThenInclude(p => p.Category)
            .Select(p => p.Product)
            .ToListAsync();
    }

    public async Task<bool> AddToLibraryAsync(string userId, int productId)
    {
        // In a real app, this would be called after successful payment
        var existing = await _context.Purchases
            .FirstOrDefaultAsync(p => p.UserId == userId && p.ProductId == productId);

        if (existing != null) return true;
        _context.Purchases.Add(new Purchase
        {
            UserId = userId,
            ProductId = productId,
            PurchaseDate = DateTime.UtcNow,
            IsPaid = true
        });
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> AddToLibraryAsync(string userId, List<int> productIds)
    {
        // Get existing purchases to avoid duplicates
        var existingProductIds = await _context.Purchases
            .Where(p => p.UserId == userId && productIds.Contains(p.ProductId))
            .Select(p => p.ProductId)
            .ToListAsync();

        // Filter out products already in library
        var newProductIds = productIds.Except(existingProductIds).ToList();

        if (!newProductIds.Any())
        {
            return true; // All products already in library
        }

        // Add new purchases
        var newPurchases = newProductIds.Select(productId => new Purchase
        {
            UserId = userId,
            ProductId = productId,
            PurchaseDate = DateTime.UtcNow,
            IsPaid = true
        }).ToList();

        _context.Purchases.AddRange(newPurchases);
        await _context.SaveChangesAsync();

        return true;
    }

    public Task<bool> IsInLibraryAsync(int productId, string userId)
    {
        return _context.Purchases
            .AnyAsync(p => p.UserId == userId && p.ProductId == productId);
    }

    public async Task<string> GetDownloadLinkAsync(int productId, string userId)
    {
        var isUserHaveProduct = await _context.Purchases
            .AnyAsync(p => p.UserId == userId && p.ProductId == productId);
        if (!isUserHaveProduct)
        {
            throw new UnauthorizedAccessException("You do not have access to this product.");
        }
        var product = await _context.Products.FindAsync(productId);
        return $"/download/{productId}/{product?.Name.Replace(" ", "-")}";
    }


}