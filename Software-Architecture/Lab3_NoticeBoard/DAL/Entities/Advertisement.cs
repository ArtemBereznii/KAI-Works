using DAL.Enums;

namespace DAL.Entities;

public class Advertisement : BaseEntity
{
    public string Title { get; private set; } = null!;
    public string Content { get; private set; } = null!;
    public AdStatus Status { get; private set; }

    public Guid CategoryId { get; private set; }
    public Category Category { get; private set; } = null!;

    public Guid UserId { get; private set; }
    public User User { get; private set; } = null!;

    private readonly List<Tag> _tags = new();
    public IReadOnlyCollection<Tag> Tags => _tags.AsReadOnly();

    protected Advertisement() { }

    private Advertisement(string title, string content, Category category, User user)
    {
        Title = title;
        Content = content;

        Category = category;
        CategoryId = category.Id;

        User = user;
        UserId = user.Id;

        Status = AdStatus.Active;
    }

    public static Advertisement Create(string title, string content, Category category, User user)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title is required", nameof(title));

        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Content is required", nameof(content));

        if (category == null)
            throw new ArgumentNullException(nameof(category));

        if (user == null)
            throw new ArgumentNullException(nameof(user));

        return new Advertisement(title, content, category, user);
    }

    public void AddTag(Tag tag)
    {
        if (tag == null) throw new ArgumentNullException(nameof(tag));

        if (!_tags.Contains(tag))
        {
            _tags.Add(tag);
        }
    }

    public void Deactivate(Guid requestingUserId)
    {
        if (UserId != requestingUserId)
            throw new InvalidOperationException("Only the user who added this advertisement can deactivate it.");

        Status = AdStatus.Deactivated;
    }

    public void CorrectTitle(string newTitle)
    {
        if (string.IsNullOrWhiteSpace(newTitle))
            throw new ArgumentException("Title cannot be empty", nameof(newTitle));

        Title = newTitle;
    }

    public void CorrectContent(string newContent)
    {
        if (string.IsNullOrWhiteSpace(newContent))
            throw new ArgumentException("Content cannot be empty", nameof(newContent));

        Content = newContent;
    }
}