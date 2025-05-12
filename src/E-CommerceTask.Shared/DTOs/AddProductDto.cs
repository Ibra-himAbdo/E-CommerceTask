namespace E_CommerceTask.Shared.DTOs;

public record AddProductDto
{
    [Required(ErrorMessage = "Product name is required")]
    [MaxLength(100)]
    public string Name { get; init; } = string.Empty;

    [Required(ErrorMessage = "Description is required")]
    [MaxLength(500)]
    public string Description { get; init; } = string.Empty;

    [Required(ErrorMessage = "Price is required")]
    [Range(0.0, 10000.0)]
    public decimal Price { get; init; }

    [MaxLength(250)]
    public string ImageUrl { get; init; } = string.Empty;

    [Required(ErrorMessage = "Quantity is required")]
    [Range(0, int.MaxValue)]
    public int Quantity { get; init; } = 1;

    [Range(0, 5)]
    public double Rate { get; init; }

    [Required]
    [BsonRepresentation(BsonType.ObjectId)]
    public ObjectId CategoryId { get; init; }
}