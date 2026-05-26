namespace DAL.Entities;

public class Category : BaseEntity
{
    public string Name { get; private set; } = null!;

    public Guid? ParentCategoryId { get; private set; }
    public Category? ParentCategory { get; private set; }

    private readonly List<Category> _subcategories = new();
    public IReadOnlyCollection<Category> Subcategories => _subcategories.AsReadOnly();

    private readonly List<Advertisement> _advertisements = new();
    public IReadOnlyCollection<Advertisement> Advertisements => _advertisements.AsReadOnly();

    protected Category() { }

    private Category(string name, Guid? parentCategoryId)
    {
        Name = name;
        ParentCategoryId = parentCategoryId;
    }

    public static Category Create(string name, Guid? parentCategoryId = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Category name is required", nameof(name));

        return new Category(name, parentCategoryId);
    }
}