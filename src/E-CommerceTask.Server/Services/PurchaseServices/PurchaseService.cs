namespace E_CommerceTask.Server.Services.PurchaseServices;

public class PurchaseService(ECommerceDbContext dbContext) : IPurchaseService
{
    public async Task<ServiceResponse<IEnumerable<Purchase>>> GetAllAsync()
    {
        var purchases = await dbContext.Purchases.AsNoTracking().ToListAsync();
        return ServiceResponse<IEnumerable<Purchase>>.Success(purchases);
    }

    public async Task<ServiceResponse<Purchase>> GetByIdAsync(ObjectId id)
    {
        var purchase = await dbContext.Purchases.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
        if (purchase == null)
        {
            return ServiceResponse<Purchase>.Failure("Purchase not found.");
        }
        return ServiceResponse<Purchase>.Success(purchase);
    }

    public async Task<ServiceResponse<Purchase>> CreateAsync(Purchase purchase)
    {
        try
        {
            var product = await dbContext.Products.FirstOrDefaultAsync(p => p.Id == purchase.ProductId);
            if (product == null)
            {
                return ServiceResponse<Purchase>.Failure("Product not found.");
            }
            if (product.Quantity < 1)
            {
                return ServiceResponse<Purchase>.Failure("Product is out of stock.");
            }

            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == purchase.UserId);
            if (user == null)
            {
                return ServiceResponse<Purchase>.Failure("User not found.");
            }

            product.Quantity--;
            dbContext.Products.Update(product);
            dbContext.Purchases.Add(purchase);
            await dbContext.SaveChangesAsync();
            return ServiceResponse<Purchase>.Success(purchase, "Purchase created successfully.");
        }
        catch (Exception ex)
        {
            return ServiceResponse<Purchase>.Failure($"Failed to create purchase: {ex.Message}");
        }
    }

    public async Task<ServiceResponse<bool>> UpdateAsync(Purchase purchase)
    {
        var existing = await dbContext.Purchases.FirstOrDefaultAsync(p => p.Id == purchase.Id);
        if (existing == null)
        {
            return ServiceResponse<bool>.Failure("Purchase not found.");
        }

        try
        {
            existing.UserId = purchase.UserId;
            existing.ProductId = purchase.ProductId;
            existing.PurchaseDate = purchase.PurchaseDate;
            existing.IsPaid = purchase.IsPaid;

            dbContext.Purchases.Update(existing);
            await dbContext.SaveChangesAsync();
            return ServiceResponse<bool>.Success(true, "Purchase updated successfully.");
        }
        catch (Exception ex)
        {
            return ServiceResponse<bool>.Failure($"Failed to update purchase: {ex.Message}");
        }
    }

    public async Task<ServiceResponse<bool>> DeleteAsync(ObjectId id)
    {
        var purchase = await dbContext.Purchases.FirstOrDefaultAsync(p => p.Id == id);
        if (purchase == null)
        {
            return ServiceResponse<bool>.Failure("Purchase not found.");
        }

        try
        {
            var product = await dbContext.Products.FirstOrDefaultAsync(p => p.Id == purchase.ProductId);
            if (product != null)
            {
                product.Quantity++;
                dbContext.Products.Update(product);
            }

            dbContext.Purchases.Remove(purchase);
            await dbContext.SaveChangesAsync();
            return ServiceResponse<bool>.Success(true, "Purchase deleted successfully.");
        }
        catch (Exception ex)
        {
            return ServiceResponse<bool>.Failure($"Failed to delete purchase: {ex.Message}");
        }
    }
}