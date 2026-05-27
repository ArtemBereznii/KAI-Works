namespace BLL.DTOs.Category;

public record CategoryResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public Guid? ParentCategoryId { get; init; }

    public List<CategoryResponse> Subcategories { get; set; } = new List<CategoryResponse>();
}