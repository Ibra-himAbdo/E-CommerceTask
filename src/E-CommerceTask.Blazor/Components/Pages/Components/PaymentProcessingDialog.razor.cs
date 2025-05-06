namespace E_CommerceTask.Blazor.Components.Pages.Components;

public partial class PaymentProcessingDialog : ComponentBase
{
    [CascadingParameter]
    private IMudDialogInstance MudDialog { get; set; }

    private bool _paymentCompleted = false;
    private Timer? _paymentTimer;
    private readonly TimeSpan _paymentDelay = TimeSpan.FromSeconds(5);

    protected override void OnInitialized()
    {
        _paymentTimer = new Timer(_ =>
        {
            InvokeAsync(() =>
            {
                _paymentCompleted = true;
                StateHasChanged();
            });
        }, null, _paymentDelay, Timeout.InfiniteTimeSpan);
    }

    private void Cancel() => MudDialog.Cancel();

    private void ContinueShopping()
    {
        MudDialog.Close();
        NavigationManager.NavigateTo("/");
    }

    private void GoToLibrary()
    {
        MudDialog.Close();
        NavigationManager.NavigateTo("/Library");
    }

    public void Dispose()
    {
        _paymentTimer?.Dispose();
    }
}