namespace E_CommerceTask.Blazor.Models;

public class Purchase
{
    [Key]
    public int Id { get; set; }
    public string UserId { get; set; }
    public int ProductId { get; set; }
    public DateTime PurchaseDate { get; set; }
    public bool IsPaid { get; set; }

    public Product Product { get; set; }
}