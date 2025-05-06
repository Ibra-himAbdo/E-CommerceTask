namespace E_CommerceTask.Blazor.Models;

public class ProductCategory : BaseEntity
{
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    public ICollection<Product> Products { get; set; } = new HashSet<Product>();
}