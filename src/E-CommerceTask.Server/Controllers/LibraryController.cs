namespace E_CommerceTask.Server.Controllers;

[Authorize]
public class LibraryController(ILibraryService libraryService) : BaseApiController
{
    private ActionResult? ValidateUserId()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdClaim))
            return Unauthorized(ServiceResponse<bool>.Failure("User ID not found in token"));

        return !ObjectId.TryParse(userIdClaim, out _) ? Unauthorized(ServiceResponse<bool>.Failure("Invalid user ID format")) : null;
    }

    private ObjectId GetUserIdFromToken()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return ObjectId.Parse(userIdClaim);
    }

    [HttpGet]
    public async Task<IActionResult> GetUserLibrary()
    {
        var validationResult = ValidateUserId();
        if (validationResult is not null) return validationResult;

        var userId = GetUserIdFromToken();
        var response = await libraryService.GetUserLibraryAsync(userId);
        return Ok(response);
    }

    [HttpPost("add/{id}")]
    public async Task<IActionResult> AddToLibrary(string id)
    {
        if (!ObjectId.TryParse(id, out var productId))
            return BadRequest("Invalid ID format.");

        var validationResult = ValidateUserId();
        if (validationResult is not null) return validationResult;

        var userId = GetUserIdFromToken();
        var response = await libraryService.AddToLibraryAsync(userId, productId);
        return Ok(response);
    }

    [HttpPost("add-multiple")]
    public async Task<IActionResult> AddMultipleToLibrary(List<ObjectId> productIds)
    {
        var validationResult = ValidateUserId();
        if (validationResult is not null) return validationResult;

        var userId = GetUserIdFromToken();
        var response = await libraryService.AddToLibraryAsync(userId, productIds);
        return Ok(response);
    }

    [HttpGet("check/{id}")]
    public async Task<IActionResult> IsInLibrary(string id)
    {
        if (!ObjectId.TryParse(id, out var productId))
            return BadRequest("Invalid ID format.");

        var validationResult = ValidateUserId();
        if (validationResult is not null) return validationResult;

        var userId = GetUserIdFromToken();
        var response = await libraryService.IsInLibraryAsync(productId, userId);
        return Ok(response);
    }

    [HttpGet("download/{id}")]
    public async Task<IActionResult> GetDownloadLink(string id)
    {
        if (!ObjectId.TryParse(id, out var productId))
            return BadRequest("Invalid ID format.");

        var validationResult = ValidateUserId();
        if (validationResult is not null) return validationResult;

        var userId = GetUserIdFromToken();
        var response = await libraryService.GetDownloadLinkAsync(productId, userId);
        return Ok(response);
    }
}