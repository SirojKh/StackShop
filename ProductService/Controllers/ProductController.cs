using Microsoft.AspNetCore.Mvc;
using ProductService.DTOs;
using ProductService.Interfaces;

namespace ProductService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _service;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(IProductService service, ILogger<ProductsController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProductReadDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        _logger.LogInformation("Fetching all products");
        return Ok(await _service.GetAllAsync());
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ProductReadDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        _logger.LogInformation("Fetching product with ID: {ProductId}", id);
        var product = await _service.GetByIdAsync(id);
        return product == null ? NotFound() : Ok(product);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ProductReadDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] ProductCreateDto dto)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid model state for Create");
            return BadRequest(ModelState);
        }

        var product = await _service.CreateAsync(dto);
        _logger.LogInformation("Product created with ID: {ProductId}", product.Id);
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ProductReadDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] ProductUpdateDto dto)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid model state for Update");
            return BadRequest(ModelState);
        }

        var updated = await _service.UpdateAsync(id, dto);
        if (updated == null)
        {
            _logger.LogWarning("Product with ID {ProductId} not found for update", id);
            return NotFound();
        }

        _logger.LogInformation("Product with ID {ProductId} updated", id);
        return Ok(updated);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var success = await _service.DeleteAsync(id);
        if (!success)
        {
            _logger.LogWarning("Product with ID {ProductId} not found for deletion", id);
            return NotFound();
        }

        _logger.LogInformation("Product with ID {ProductId} deleted", id);
        return NoContent();
    }
}
