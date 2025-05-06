namespace E_CommerceTask.Blazor.Models;

public class User
{
    [Key]
    [MaxLength(500)]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [MaxLength(200)]
    public string Email { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string PasswordHash { get; set; } = string.Empty;

    [MaxLength(200)]
    public string Role { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}