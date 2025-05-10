namespace E_CommerceTask.Shared.Models;

[Collection("purchases")]
public class Purchase : BaseEntity
{
    [Required(ErrorMessage = "User ID is required")]
    public ObjectId UserId { get; set; }

    [Required]
    public ObjectId ProductId { get; set; }

    [Required(ErrorMessage = "Purchase date is required")]
    public DateTime PurchaseDate { get; set; } = DateTime.UtcNow;

    public bool IsPaid { get; set; }

    [BsonIgnoreIfNull]
    public Product? Product { get; set; }

    [BsonIgnoreIfNull]
    public User? User { get; set; }
}