namespace E_CommerceTask.Blazor.Components.Pages.Components;

public partial class DeleteProductDialog : ComponentBase
{
    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; }
    [Parameter] public ObjectId Id { get; set; }

    private async Task Submit()
    {
        var response = await ProductService.DeleteAsync(Id);
        if (!response.IsSuccess)
        {
            Snackbar.Add(response.Message, Severity.Error);
            MudDialog.Close(DialogResult.Ok(true));
        }

        Snackbar.Add(response.Message, Severity.Success);
        MudDialog.Close(DialogResult.Ok(true));
        StateHasChanged();
    }

    private void Cancel() => MudDialog.Cancel();
}