namespace E_CommerceTask.Blazor.Components.Pages;

public partial class Library : ComponentBase
{
    private IEnumerable<Product>? _library;
    private ObjectId? _downloadingGameId;
    private bool _isAuthenticated;

    protected override async Task OnInitializedAsync()
    {
        await LoadAuthState();

        if (_isAuthenticated)
        {
            var response = await LibraryService.GetUserLibraryAsync();
            _library = response.Data ?? [];
        }
    }

    private async Task LoadAuthState()
    {
        var authState = await AuthStateProvider.GetAuthenticationStateAsync();
        _isAuthenticated = authState.User.Identity?.IsAuthenticated ?? false;
    }

    private Task DownloadGame(ObjectId gameId)
    {
        _downloadingGameId = gameId;
        StateHasChanged();

        try
        {
            Snackbar.Add("Download started", Severity.Success);
        }
        finally
        {
            _downloadingGameId = null;
            StateHasChanged();
        }

        return Task.CompletedTask;
    }

    private static string ImageUrl(string url)
        => string.IsNullOrEmpty(url) ? "https://fakeimg.pl/600x400" : url;
}