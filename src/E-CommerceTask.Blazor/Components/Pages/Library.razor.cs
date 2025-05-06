namespace E_CommerceTask.Blazor.Components.Pages;

public partial class Library : ComponentBase
{
    private List<Product>? _library;
    private int? _downloadingGameId;
    private string? userId;

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthStateProvider.GetAuthenticationStateAsync();
        userId = authState.User.FindFirst(ClaimTypes.NameIdentifier)
            ?.Value;

        if (userId != null)
        {
            _library = await LibraryService.GetUserLibraryAsync(userId);
        }
    }

    private async Task DownloadGame(int gameId)
    {
        _downloadingGameId = gameId;
        StateHasChanged();

        try
        {
            Snackbar.Add("Download started", Severity.Success);
            // var downloadUrl = await LibraryService.GetDownloadLinkAsync(gameId, userId!);
            // // In a real app, you would trigger the download here
            // await JsRuntime.InvokeVoidAsync("open", downloadUrl, "_blank");
        }
        finally
        {
            _downloadingGameId = null;
            StateHasChanged();
        }
    }

    private static string ImageUrl(string url)
        => string.IsNullOrEmpty(url) ? "https://fakeimg.pl/600x400" : url;
}