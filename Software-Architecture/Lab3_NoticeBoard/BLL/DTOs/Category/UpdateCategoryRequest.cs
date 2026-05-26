namespace BLL.DTOs.Category;

public record UpdateCategoryRequest(
    Guid Id,
    string? Name,
    Guid? ParentCategoryId
);