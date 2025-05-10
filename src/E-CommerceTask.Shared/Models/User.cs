namespace E_CommerceTask.Shared.Models;

[Collection("users")]
public class User : BaseEntity
{
    [Required(ErrorMessage = "Email is required")]
    [MaxLength(200)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password hash is required")]
    [MaxLength(1000)]
    public string PasswordHash { get; set; } = string.Empty;

    [Required(ErrorMessage = "Role is required")]
    [MaxLength(200)]
    public string Role { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}