using System.ComponentModel.DataAnnotations;

namespace E_CommerceTask.Shared.Models;

public class Card
{
    [Required(ErrorMessage = "Card number is required.")]
    [RegularExpression(@"^\d{16}$", ErrorMessage = "Card number must be 16 digits.")]
    public string CardNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Expiration date is required.")]
    [RegularExpression(@"^(0[1-9]|1[0-2])\/\d{2}$", ErrorMessage = "Expiration date must be in MM/YY format.")]
    public string ExpirationDate { get; set; } = string.Empty;

    [Required(ErrorMessage = "CVV is required.")]
    [RegularExpression(@"^\d{3,4}$", ErrorMessage = "CVV must be 3 or 4 digits.")]
    public string CVV { get; set; } = string.Empty;
}
