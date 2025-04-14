namespace ProductService.DTOs;

public class ProductUpdateDto : ProductCreateDto
{
    public Guid Id { get; set; }
}