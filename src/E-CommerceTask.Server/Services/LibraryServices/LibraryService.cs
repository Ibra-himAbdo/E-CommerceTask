namespace E_CommerceTask.Server.Services.LibraryServices;

public class LibraryService(ECommerceDbContext dbContext) : ILibraryService
{
    public async Task<ServiceResponse<IEnumerable<Product>>> GetUserLibraryAsync(ObjectId userId)
    {
        try
        {
            var products = await dbContext.Purchases
                .Where(p => p.UserId == userId && p.IsPaid)
                .Include(p => p.Product)
                .ThenInclude(p => p.Category)
                .Select(p => p.Product)
                .ToListAsync();
                
            return ServiceResponse<IEnumerable<Product>>.Success(products);
        }
        catch (Exception ex)
        {
            return ServiceResponse<IEnumerable<Product>>.Failure($"Failed to get user library: {ex.Message}");
        }
    }

    public async Task<ServiceResponse<bool>> AddToLibraryAsync(ObjectId userId, ObjectId productId)
    {
        try
        {
            var existing = await dbContext.Purchases
                .FirstOrDefaultAsync(p => p.UserId == userId && p.ProductId == productId);

            if (existing != null) 
                return ServiceResponse<bool>.Success(true, "Product already in library");

            dbContext.Purchases.Add(new Purchase
            {
                UserId = userId,
                ProductId = productId,
                PurchaseDate = DateTime.UtcNow,
                IsPaid = true
            });
            
            await dbContext.SaveChangesAsync();
            return ServiceResponse<bool>.Success(true, "Product added to library");
        }
        catch (Exception ex)
        {
            return ServiceResponse<bool>.Failure($"Failed to add to library: {ex.Message}");
        }
    }

    public async Task<ServiceResponse<bool>> AddToLibraryAsync(ObjectId userId, List<ObjectId> productIds)
    {
        try
        {
            var existingProductIds = await dbContext.Purchases
                .Where(p => p.UserId == userId && productIds.Contains(p.ProductId))
                .Select(p => p.ProductId)
                .ToListAsync();

            var newProductIds = productIds.Except(existingProductIds).ToList();

            if (!newProductIds.Any())
                return ServiceResponse<bool>.Success(true, "All products already in library");

            var newPurchases = newProductIds.Select(productId => new Purchase
            {
                UserId = userId,
                ProductId = productId,
                PurchaseDate = DateTime.UtcNow,
                IsPaid = true
            }).ToList();

            dbContext.Purchases.AddRange(newPurchases);
            await dbContext.SaveChangesAsync();

            return ServiceResponse<bool>.Success(true, "Products added to library");
        }
        catch (Exception ex)
        {
            return ServiceResponse<bool>.Failure($"Failed to add products to library: {ex.Message}");
        }
    }

    public async Task<ServiceResponse<bool>> IsInLibraryAsync(ObjectId productId, ObjectId userId)
    {
        try
        {
            var exists = await dbContext.Purchases
                .AnyAsync(p => p.UserId == userId && p.ProductId == productId);
                
            return ServiceResponse<bool>.Success(exists);
        }
        catch (Exception ex)
        {
            return ServiceResponse<bool>.Failure($"Failed to check library status: {ex.Message}");
        }
    }

    public async Task<ServiceResponse<string>> GetDownloadLinkAsync(ObjectId productId, ObjectId userId)
    {
        try
        {
            var hasAccess = await dbContext.Purchases
                .AnyAsync(p => p.UserId == userId && p.ProductId == productId);
                
            if (!hasAccess)
                return ServiceResponse<string>.Failure("You do not have access to this product");

            var product = await dbContext.Products.FindAsync(productId);
            var downloadLink = $"/download/{productId}/{product?.Name.Replace(" ", "-")}";
            
            return ServiceResponse<string>.Success(downloadLink);
        }
        catch (Exception ex)
        {
            return ServiceResponse<string>.Failure($"Failed to get download link: {ex.Message}");
        }
    }
}