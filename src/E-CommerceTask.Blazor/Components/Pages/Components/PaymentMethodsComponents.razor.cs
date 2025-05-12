namespace E_CommerceTask.Blazor.Components.Pages.Components;

public partial class PaymentMethodsComponents : ComponentBase
{
    private bool _isAuthenticated;
    private PaymentMethod _selectedPayment = PaymentMethod.CreditCard;
    private Card _card = new();

    protected override async Task OnInitializedAsync()
    {
        await LoadAuthState();
    }

    private async Task LoadAuthState()
    {
        var authState = await AuthStateProvider.GetAuthenticationStateAsync();
        _isAuthenticated = authState.User.Identity?.IsAuthenticated ?? false;
    }

    private async Task OnValidSubmit(EditContext context)
    {
        if (context.Validate())
        {
            await OpenDialogAsync();
            StateHasChanged();
        }
    }

    private async Task OnPaymentComplete()
    {
        var items = await CartService.GetCartItems();
        if (_isAuthenticated && items.Count != 0)
        {
            await LibraryService.AddToLibraryAsync(items.Select(p => p.Id)
                .ToList());
            await CartService.ClearCart();
            StateHasChanged();
        }
    }

    private async Task OpenDialogAsync()
    {
        var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall };
        await OnPaymentComplete();
        await DialogService.ShowAsync<PaymentProcessingDialog>("Payment Process", options);
    }
}