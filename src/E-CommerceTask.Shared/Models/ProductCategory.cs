namespace E_CommerceTask.Shared.Models;

[Collection("product_categories")]
public class ProductCategory : BaseEntity
{
    [Required(ErrorMessage = "Category name is required")]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    public ICollection<Product> Products { get; set; } = new HashSet<Product>();
}