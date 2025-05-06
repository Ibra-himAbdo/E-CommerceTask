namespace E_CommerceTask.Blazor.Services.CartServices;

public class CartService : ICartService
{
    public event Action? OnChange;
    private List<Product> _cartItems = [];
    private decimal _shoppingFee = 0.0m;
    private static readonly Random Random = new Random();

    public int TotalItems => _cartItems.Sum(p => p.Quantity);
    public decimal TotalPrice => _cartItems.Sum(p => p.Price * p.Quantity) + _shoppingFee;

    public decimal ShoppingFee => _shoppingFee;

    public Task AddToCart(Product product)
    {
        var existingProduct =
            _cartItems.FirstOrDefault(p => p.Id == product.Id);

        if (existingProduct is not null)
            existingProduct.Quantity += product.Quantity;
        else
            _cartItems.Add(product);

        NotifyStateChanged();
        return Task.CompletedTask;
    }

    public Task AddItemToCart(Product product, int quantity)
    {
        var existingProduct = _cartItems
            .FirstOrDefault(p => p.Id == product.Id);

        if (existingProduct is not null)
        {
            existingProduct.Quantity += quantity;
        }
        else
        {
            product.Quantity = quantity;
            _cartItems.Add(product);
        }

        NotifyStateChanged();
        return Task.CompletedTask;
    }


    public Task RemoveFromCart(int productId)
    {
        var existingProduct =
            _cartItems.FirstOrDefault(p => p.Id == productId);

        if (existingProduct is not null)
        {
            if (existingProduct.Quantity > 1)
                existingProduct.Quantity--;
            else
                _cartItems.Remove(existingProduct);
        }

        NotifyStateChanged();
        return Task.CompletedTask;
    }

    public Task DeleteFromCart(int productId)
    {
        var existingProduct =
            _cartItems.FirstOrDefault(p => p.Id == productId);

        if (existingProduct is not null)
            _cartItems.Remove(existingProduct);

        NotifyStateChanged();
        return Task.CompletedTask;
    }

    public Task ClearCart()
    {
        _cartItems.Clear();
        NotifyStateChanged();
        return Task.CompletedTask;
    }

    public Task<List<Product>> GetCartItems()
    {
        return Task.FromResult(_cartItems);
    }

    public Task<decimal> GetTotalPrice()
    {
        var totalPrice = _cartItems.Sum(p => p.Price * p.Quantity);
        return Task.FromResult(totalPrice);
    }

    public Task<int> GetTotalItems()
    {
        var totalItems = _cartItems.Sum(p => p.Quantity);
        return Task.FromResult(totalItems);
    }

    public Task<bool> IsProductInCart(Product product)
    {
        var isInCart = _cartItems.Any(p => p.Id == product.Id);
        return Task.FromResult(isInCart);
    }

    public Task AddShoppingFee(ShippingMethod shippingMethod)
    {
        var fee = shippingMethod switch
        {
            ShippingMethod.Standard => 5.00m,
            ShippingMethod.Express => 10.00m,
            ShippingMethod.NextDay => 20.00m,
            _ => 0.00m
        };
        _shoppingFee = fee;
        NotifyStateChanged();
        return Task.CompletedTask;
    }

    public Task UsePromoCode(string code)
    {
        switch (code)
        {
            case "DISCOUNT10":
            default:
                _shoppingFee -= 10.00m;
                break;
            case "FREESHIP":
                _shoppingFee = 0.00m;
                break;
        }

        NotifyStateChanged();
        return Task.CompletedTask;
    }

    private void NotifyStateChanged() => OnChange?.Invoke();

    public static List<Product> GetRandomProducts(int count)
    {
        var products = new List<Product>();
        for (var i = 0; i < count; i++)
        {
            products.Add(new Product
            {
                Id = i + 1,
                Name = $"Product {i + 1}",
                Description = $"Description for Product {i + 1}",
                Price = Math.Round((decimal)(Random.NextDouble() * 100), 2),
                ImageUrl = $"https://fakeimg.pl/{600 + i * 120}x{400 + i * 50}",
                Quantity = Random.Next(1, 10)
            });
        }

        return products;
    }
}