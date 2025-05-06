namespace E_CommerceTask.Blazor.Models;

public class Product : BaseEntity
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(100)]
    public string NormalizedName { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    [Required]
    [Range(0.0, 10000.0)]
    public decimal Price { get; set; }

    [MaxLength(250)]
    public string ImageUrl { get; set; } = string.Empty;

    [Required]
    [Range(0, int.MaxValue)]
    public int Quantity { get; set; } = 1;

    [Range(0, 5)]
    public double Rate { get; set; }

    public DateTime DateAdded { get; set; } = DateTime.UtcNow;

    [Required]
    public int CategoryId { get; set; }
    public ProductCategory? Category { get; set; }
}