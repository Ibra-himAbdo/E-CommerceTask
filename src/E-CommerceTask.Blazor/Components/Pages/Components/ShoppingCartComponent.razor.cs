namespace E_CommerceTask.Blazor.Components.Pages.Components;

public partial class ShoppingCartComponent : ComponentBase
{
    protected override void OnInitialized()
    {
        base.OnInitialized();
        CartService.OnChange += StateHasChanged;
    }

    public void Dispose()
        => CartService.OnChange -= StateHasChanged;

    private void GoToSoppingCartPage()
        => NavigationManager.NavigateTo("/ShoppingCart");
}