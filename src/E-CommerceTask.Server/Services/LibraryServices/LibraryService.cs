namespace E_CommerceTask.Server.Services.LibraryServices;

public class LibraryService(ECommerceDbContext dbContext) : ILibraryService
{
    public async Task<ServiceResponse<IEnumerable<Product>>> GetUserLibraryAsync(ObjectId userId)
    {
        try
        {
            // Get all purchased product IDs for the user
            var purchasedProductIds = await dbContext.Purchases
                .Where(p => p.UserId == userId && p.IsPaid)
                .Select(p => p.ProductId)
                .ToListAsync();

            // Get all products that match the purchased IDs
            var products = await dbContext.Products
                .Where(p => purchasedProductIds.Contains(p.Id))
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
            // Check if product exists
            var productExists = await dbContext.Products.AnyAsync(p => p.Id == productId);
            if (!productExists)
                return ServiceResponse<bool>.Failure("Product not found");

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
            // Verify all products exist
            var existingProductsCount = await dbContext.Products
                .CountAsync(p => productIds.Contains(p.Id));

            if (existingProductsCount != productIds.Count)
                return ServiceResponse<bool>.Failure("One or more products not found");

            var existingProductIds = await dbContext.Purchases
                .Where(p => p.UserId == userId && productIds.Contains(p.ProductId))
                .Select(p => p.ProductId)
                .ToListAsync();

            var newProductIds = productIds.Except(existingProductIds)
                .ToList();

            if (!newProductIds.Any())
                return ServiceResponse<bool>.Success(true, "All products already in library");

            var newPurchases = newProductIds.Select(productId => new Purchase
                {
                    UserId = userId,
                    ProductId = productId,
                    PurchaseDate = DateTime.UtcNow,
                    IsPaid = true
                })
                .ToList();

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
            if (product == null)
                return ServiceResponse<string>.Failure("Product not found");

            var downloadLink = $"/download/{productId}/{product.Name.Replace(" ", "-")}";

            return ServiceResponse<string>.Success(downloadLink);
        }
        catch (Exception ex)
        {
            return ServiceResponse<string>.Failure($"Failed to get download link: {ex.Message}");
        }
    }
}