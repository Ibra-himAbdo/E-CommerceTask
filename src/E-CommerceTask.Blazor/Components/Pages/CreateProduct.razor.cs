namespace E_CommerceTask.Blazor.Components.Pages;

public partial class CreateProduct : ComponentBase
{
    [Parameter] public string Id { get; set; }
    private Product model = new();
    private IEnumerable<ProductCategory> categories = [];
    private string _baseUrl;
    private string? _imageUrl;
    private string? _imageBase64; // New field for base64 display
    private IBrowserFile? uploadedFile;
    private const long MaxFileSize = 15 * 1024 * 1024;
    private bool IsEditMode => !string.IsNullOrEmpty(Id);
    private bool _isUploading;
    private bool _isProcessing;
    private bool success;
    private string uploadError;

    protected override async Task OnInitializedAsync()
    {
        _baseUrl = NavigationManager.BaseUri;
        var categoryResponse = await ProductService.GetCategoriesAsync();
        if (categoryResponse.IsSuccess)
            categories = categoryResponse.Data ?? [];

        if (IsEditMode)
        {
            var response = await ProductService.GetByIdAsync(ObjectId.Parse(Id));
            if (response.IsSuccess)
            {
                model = response.Data ?? new Product();
                _imageUrl = response.Data?.ImageUrl;
                // Convert existing image to base64 for display
                if (!string.IsNullOrEmpty(_imageUrl))
                {
                    await LoadImageAsBase64(_imageUrl);
                }
            }
            else
                Snackbar.Add($"Error loading product: {response.Message}", Severity.Error);
        }
    }

    private string GetFullImageUrl(string url)
    {
        if (string.IsNullOrEmpty(url))
            return "https://fakeimg.pl/600x400?text=Product+Image";

        // If already a full URL or data URI, return as-is
        if (url.StartsWith("http") || url.StartsWith("data:image"))
            return url;

        // Handle local paths
        if (url.StartsWith("/"))
            return $"{NavigationManager.BaseUri.TrimEnd('/')}{url}";

        return url;
    }

    private async Task LoadImageAsBase64(string imageUrl)
    {
        try
        {
            var fullUrl = GetFullImageUrl(imageUrl);

            // Skip if already a base64 image
            if (fullUrl.StartsWith("data:image"))
            {
                _imageBase64 = fullUrl.Split(',')[1];
                return;
            }

            // Handle local files
            if (!fullUrl.StartsWith("http"))
            {
                var filePath = Path.Combine(Environment.WebRootPath, imageUrl.TrimStart('/'));
                if (File.Exists(filePath))
                {
                    var readAllBytesAsync = await File.ReadAllBytesAsync(filePath);
                    _imageBase64 = Convert.ToBase64String(readAllBytesAsync);
                }

                return;
            }

            // Handle remote URLs (only if absolutely necessary)
            using var httpClient = new HttpClient();
            var bytes = await httpClient.GetByteArrayAsync(fullUrl);
            _imageBase64 = $"data:image/jpeg;base64,{Convert.ToBase64String(bytes)}";
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Error loading image for preview: {ex.Message}", Severity.Error);
            // Fall back to original URL
            _imageBase64 = null;
        }
    }

    private string GetImageDisplay()
    {
        // Show base64 preview if available (for newly uploaded files)
        if (!string.IsNullOrEmpty(_imageBase64))
        {
            return _imageBase64;
        }

        // Show saved image URL if available
        if (!string.IsNullOrEmpty(model.ImageUrl))
        {
            return GetFullImageUrl(model.ImageUrl);
        }

        // Default image
        return "https://fakeimg.pl/600x400?text=Product+Image";
    }

    private async Task OnValidSubmit(EditContext context)
    {
        _isProcessing = true;
        success = false;
        uploadError = string.Empty;

        try
        {
            model.NormalizedName = model.Name?.ToUpper()!;
            model.DateAdded = IsEditMode ? model.DateAdded : DateTime.UtcNow;
            model.ImageUrl = uploadedFile != null
                ? Path.Combine("uploads", "products", uploadedFile.Name)
                : model.ImageUrl;

            ServiceResponse<bool> serviceResponse;

            if (IsEditMode)
            {
                serviceResponse = await ProductService.UpdateAsync(model);
            }
            else
            {
                serviceResponse = await ProductService.CreateAsync(model);
            }

            if (!serviceResponse.IsSuccess)
            {
                Snackbar.Add($"Error saving product: {serviceResponse.Message}", Severity.Error);
                return;
            }

            success = true;

            // Reset form if not in edit mode
            if (!IsEditMode)
            {
                model = new Product();
                uploadedFile = null;
                _imageBase64 = null;
                context.NotifyValidationStateChanged();
            }
        }
        catch (Exception ex)
        {
            uploadError = $"Error saving product: {ex.Message}";
        }
        finally
        {
            _isProcessing = false;
            StateHasChanged();
        }
    }

    private async Task UploadFiles(IBrowserFile? file)
    {
        if (file == null)
        {
            Snackbar.Add("No file selected.", Severity.Error);
            return;
        }

        if (file.Size > MaxFileSize)
        {
            Snackbar.Add("File size exceeds the 15MB limit.", Severity.Error);
            return;
        }

        _isUploading = true;
        uploadedFile = file;
        StateHasChanged();

        try
        {
            // First read the file into memory
            await using var stream = file.OpenReadStream(MaxFileSize);
            using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            var bytes = memoryStream.ToArray();

            // Create base64 for preview
            _imageBase64 = $"data:{file.ContentType};base64,{Convert.ToBase64String(bytes)}";

            // Save to file system
            var folderPath = Path.Combine(Environment.WebRootPath, "uploads", "products");
            Directory.CreateDirectory(folderPath);
            var filePath = Path.Combine(folderPath, file.Name);

            await File.WriteAllBytesAsync(filePath, bytes);

            model.ImageUrl = $"/uploads/products/{file.Name}";

            Snackbar.Add("File uploaded successfully!", Severity.Success);
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Error uploading file: {ex.Message}", Severity.Error);
        }
        finally
        {
            _isUploading = false;
            StateHasChanged();
        }
    }
}