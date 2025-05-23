namespace E_CommerceTask.Shared.DTOs;

public class LoginModel
{

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [MinLength(8)]
    public string Password { get; set; }

    public LoginModel(string email, string password)
    {
        Email = email;
        Password = password;
    }
    public LoginModel()
    {
    }
}