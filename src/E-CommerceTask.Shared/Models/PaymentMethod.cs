namespace E_CommerceTask.Shared.Models;
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PaymentMethod
{
    CreditCard,
    PayPal,
    GooglePay,
    ApplePay
}