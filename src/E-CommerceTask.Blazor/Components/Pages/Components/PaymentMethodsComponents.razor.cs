using E_CommerceTask.Shared.Models;

namespace E_CommerceTask.Blazor.Components.Pages.Components;

public partial class PaymentMethodsComponents : ComponentBase
{
    private string? _userId;
    private PaymentMethod _selectedPayment = PaymentMethod.CreditCard;
    private Card _card = new();

    private async Task OnValidSubmit(EditContext context)
    {
        // Handle payment submission
        await OpenDialogAsync();
        StateHasChanged();
    }

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthStateProvider.GetAuthenticationStateAsync();
        _userId = authState.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (_userId != null)
        {
            // Use the ID for your logic
            Console.WriteLine($"User ID: {_userId}");
        }
    }

    private async Task OnPaymentComplete()
    {
        var items = await CartService.GetCartItems();
        if (_userId != null)
        {
            await LibraryService.AddToLibraryAsync(ObjectId.Parse(_userId), items.Select(p => p.Id)
                .ToList());
            await CartService.ClearCart();
            StateHasChanged();
        }

    }

    private async Task OpenDialogAsync()
    {
        var options = new DialogOptions { CloseOnEscapeKey = true };
        await OnPaymentComplete();

        await DialogService.ShowAsync<PaymentProcessingDialog>("Payment Process", options);
    }

}