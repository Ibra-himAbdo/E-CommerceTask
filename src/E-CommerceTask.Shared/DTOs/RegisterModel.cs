namespace E_CommerceTask.Shared.DTOs;

public class RegisterModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [MinLength(6)]
    public string Password { get; set; }

    [Required]
    public string ConfirmPassword { get; set; }
}