namespace E_CommerceTask.Server.Services.ProductServices;

public class ProductService(ECommerceDbContext dbContext) : IProductService
{
    public async Task<ServiceResponse<IEnumerable<Product>>> GetAllAsync()
    {
        var products = await dbContext.Products.Include(c => c.Category)
            .AsNoTracking()
            .ToListAsync();
        return ServiceResponse<IEnumerable<Product>>.Success(products);
    }

    public async Task<ServiceResponse<Product>> GetByIdAsync(ObjectId id)
    {
        var product = await dbContext.Products.Include(c => c.Category).AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);
        return product is null
            ? ServiceResponse<Product>.Failure("Product not found.")
            : ServiceResponse<Product>.Success(product);
    }

    public async Task<ServiceResponse<Product>> CreateAsync(Product product, ObjectId categoryId)
    {
        try
        {
            var category = await dbContext.Categories.FindAsync(categoryId);
            if (category is null)
                return ServiceResponse<Product>.Failure("Category not found.");
            product.NormalizedName = product.Name.ToUpperInvariant();
            dbContext.Products.Add(product);
            await dbContext.SaveChangesAsync();
            return ServiceResponse<Product>.Success(product, "Product created successfully.");
        }
        catch (Exception ex)
        {
            return ServiceResponse<Product>.Failure($"Failed to create product: {ex.Message}");
        }
    }

    public async Task<ServiceResponse<bool>> UpdateAsync(Product product)
    {
        var existing = await dbContext.Products.FirstOrDefaultAsync(p => p.Id == product.Id);
        if (existing is null)
            return ServiceResponse<bool>.Failure("Product not found.");

        try
        {
            existing.Name = product.Name;
            existing.NormalizedName = product.Name.ToUpperInvariant();
            existing.Description = product.Description;
            existing.Price = product.Price;
            existing.ImageUrl = product.ImageUrl;
            existing.Quantity = product.Quantity;
            existing.Rate = product.Rate;
            existing.CategoryId = product.CategoryId;

            dbContext.Products.Update(existing);
            await dbContext.SaveChangesAsync();
            return ServiceResponse<bool>.Success(true, "Product updated successfully.");
        }
        catch (Exception ex)
        {
            return ServiceResponse<bool>.Failure($"Failed to update product: {ex.Message}");
        }
    }

    public async Task<ServiceResponse<bool>> DeleteAsync(ObjectId id)
    {
        var product = await dbContext.Products.FirstOrDefaultAsync(p => p.Id == id);
        if (product == null)
        {
            return ServiceResponse<bool>.Failure("Product not found.");
        }

        try
        {
            dbContext.Products.Remove(product);
            await dbContext.SaveChangesAsync();
            return ServiceResponse<bool>.Success(true, "Product deleted successfully.");
        }
        catch (Exception ex)
        {
            return ServiceResponse<bool>.Failure($"Failed to delete product: {ex.Message}");
        }
    }
}