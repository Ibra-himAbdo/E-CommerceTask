namespace E_CommerceTask.Blazor.Helpers.ApiHelpers;

public record ApiSettings
{
    public string ApiName { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = string.Empty;
    public ApiEndpoints? Endpoints { get; set; }
}