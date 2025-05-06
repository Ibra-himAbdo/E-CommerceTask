namespace E_CommerceTask.Blazor.Components.Pages;

public partial class Checkout : ComponentBase
{
    string _address = string.Empty;
    private ShippingMethod _selectedShipping = ShippingMethod.Standard;

    protected override async Task OnInitializedAsync()
        => await CartService.AddShoppingFee(_selectedShipping);

    private async Task OnShippingChanged(ShippingMethod method)
        => await CartService.AddShoppingFee(method);
}