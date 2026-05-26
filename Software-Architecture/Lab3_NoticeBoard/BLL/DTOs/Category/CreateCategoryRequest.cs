namespace BLL.DTOs.Category;

public record CreateCategoryRequest(
    string Name,
    Guid? ParentCategoryId
);