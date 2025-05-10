namespace E_CommerceTask.Shared.Models;

[Collection("products")]
public class Product : BaseEntity
{
    [Required(ErrorMessage = "Product name is required")]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Normalized name is required for search")]
    [MaxLength(100)]
    public string NormalizedName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Description is required")]
    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Price is required")]
    [Range(0.0, 10000.0)]
    public decimal Price { get; set; }

    [MaxLength(250)]
    public string ImageUrl { get; set; } = string.Empty;

    [Required(ErrorMessage = "Quantity is required")]
    [Range(0, int.MaxValue)]
    public int Quantity { get; set; } = 1;

    [Range(0, 5)]
    public double Rate { get; set; }

    public DateTime DateAdded { get; set; } = DateTime.UtcNow;

    [Required]
    public ObjectId CategoryId { get; set; }

    [BsonIgnoreIfNull]
    public ProductCategory? Category { get; set; }
}