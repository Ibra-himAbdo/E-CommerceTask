namespace E_CommerceTask.Shared.Models;
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ProductFilters
{
    All,
    TopRated,
    Newest,
    Oldest,
    PriceLowToHigh,
    PriceHighToLow,
}