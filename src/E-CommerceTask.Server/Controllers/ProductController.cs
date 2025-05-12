namespace E_CommerceTask.Server.Controllers;

public class ProductController(IProductService productService) : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var response = await productService.GetAllAsync();
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        if (!ObjectId.TryParse(id, out var objectId))
        {
            return BadRequest("Invalid ID format.");
        }

        var response = await productService.GetByIdAsync(objectId);
        return Ok(response);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] AddProductDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var product = new Product()
        {
            Name = request.Name,
            NormalizedName = request.Name.ToUpper(),
            Description = request.Description,
            Price = request.Price,
            Quantity = request.Quantity,
            ImageUrl = request.ImageUrl,
            CategoryId = request.CategoryId,
            DateAdded = DateTime.UtcNow,
        };

        var response = await productService.CreateAsync(product);
        return Ok(response);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] Product product)
    {
        if (!ObjectId.TryParse(id, out var objectId) || product.Id != objectId)
        {
            return BadRequest("Invalid ID.");
        }

        var response = await productService.UpdateAsync(product);
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

        var response = await productService.DeleteAsync(objectId);
        return Ok(response);
    }
}