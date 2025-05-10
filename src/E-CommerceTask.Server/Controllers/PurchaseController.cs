using System.Security.Claims;
using E_CommerceTask.Server.Services.PurchaseServices;

namespace E_CommerceTask.Server.Controllers;

public class PurchaseController(IPurchaseService purchaseService) : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var response = await purchaseService.GetAllAsync();
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        if (!ObjectId.TryParse(id, out var objectId))
        {
            return BadRequest("Invalid ID format.");
        }

        var response = await purchaseService.GetByIdAsync(objectId);
        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Purchase purchase)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!ObjectId.TryParse(userIdString, out var userId))
        {
            return Unauthorized("Invalid user ID in token.");
        }

        purchase.UserId = userId;
        var response = await purchaseService.CreateAsync(purchase);
        return Ok(response);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] Purchase purchase)
    {
        if (!ObjectId.TryParse(id, out var objectId) || purchase.Id != objectId)
        {
            return BadRequest("Invalid ID.");
        }

        var response = await purchaseService.UpdateAsync(purchase);
        return Ok(response);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        if (!ObjectId.TryParse(id, out var objectId))
        {
            return BadRequest("Invalid ID format.");
        }

        var response = await purchaseService.DeleteAsync(objectId);
        return Ok(response);
    }
}