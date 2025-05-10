namespace E_CommerceTask.Server.Services.ProductCategoryServices;

public class ProductCategoryService(ECommerceDbContext dbContext) : IProductCategoryService
{
    public async Task<ServiceResponse<IEnumerable<ProductCategory>>> GetAllAsync()
    {
        var categories = await dbContext.Categories.AsNoTracking().ToListAsync();
        return ServiceResponse<IEnumerable<ProductCategory>>.Success(categories);
    }

    public async Task<ServiceResponse<ProductCategory>> GetByIdAsync(ObjectId id)
    {
        var category = await dbContext.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
        if (category == null)
        {
            return ServiceResponse<ProductCategory>.Failure("Category not found.");
        }
        return ServiceResponse<ProductCategory>.Success(category);
    }

    public async Task<ServiceResponse<ProductCategory>> CreateAsync(ProductCategory category)
    {
        try
        {
            dbContext.Categories.Add(category);
            await dbContext.SaveChangesAsync();
            return ServiceResponse<ProductCategory>.Success(category, "Category created successfully.");
        }
        catch (Exception ex)
        {
            return ServiceResponse<ProductCategory>.Failure($"Failed to create category: {ex.Message}");
        }
    }

    public async Task<ServiceResponse<bool>> UpdateAsync(ProductCategory category)
    {
        var existing = await dbContext.Categories.FirstOrDefaultAsync(c => c.Id == category.Id);
        if (existing == null)
        {
            return ServiceResponse<bool>.Failure("Category not found.");
        }

        try
        {
            existing.Name = category.Name;
            dbContext.Categories.Update(existing);
            await dbContext.SaveChangesAsync();
            return ServiceResponse<bool>.Success(true, "Category updated successfully.");
        }
        catch (Exception ex)
        {
            return ServiceResponse<bool>.Failure($"Failed to update category: {ex.Message}");
        }
    }

    public async Task<ServiceResponse<bool>> DeleteAsync(ObjectId id)
    {
        var category = await dbContext.Categories.FirstOrDefaultAsync(c => c.Id == id);
        if (category == null)
        {
            return ServiceResponse<bool>.Failure("Category not found.");
        }

        try
        {
            dbContext.Categories.Remove(category);
            await dbContext.SaveChangesAsync();
            return ServiceResponse<bool>.Success(true, "Category deleted successfully.");
        }
        catch (Exception ex)
        {
            return ServiceResponse<bool>.Failure($"Failed to delete category: {ex.Message}");
        }
    }
}