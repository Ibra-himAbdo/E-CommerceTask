namespace E_CommerceTask.Blazor.Models;

public class Product
{
    public int Id { get; init; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; init; }
    public string ImageUrl { get; set; } = string.Empty;
    public int Quantity { get; set; } = 1;
}