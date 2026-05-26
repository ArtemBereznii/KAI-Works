namespace DAL.Entities;

public class Tag : BaseEntity
{
    public string Name { get; private set; } = null!;
    
    private readonly List<Advertisement> _advertisements = new();
    public IReadOnlyCollection<Advertisement> Advertisements => _advertisements.AsReadOnly();

    protected Tag() { }

    private Tag(string name)
    {
        Name = name;
    }

    public static Tag Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Tag name is required", nameof(name));

        return new Tag(name);
    }
}