namespace E_CommerceTask.Blazor.Components.Pages.Components;

public partial class PaymentMethodsComponents : ComponentBase
{
    private PaymentMethod _selectedPayment = PaymentMethod.CreditCard;
    private Card _card = new();

    private void OnValidSubmit(EditContext context)
    {
        // Handle payment submission
        StateHasChanged();
    }

}