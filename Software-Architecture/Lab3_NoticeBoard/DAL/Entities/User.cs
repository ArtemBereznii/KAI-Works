namespace DAL.Entities;

public class User : BaseEntity
{
    public string Username { get; private set; } = null!;

    private readonly List<Advertisement> _advertisements = new();
    public IReadOnlyCollection<Advertisement> Advertisements => _advertisements.AsReadOnly();

    protected User() { }

    private User(string username)
    {
        Username = username;
    }

    public static User Create(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username is required", nameof(username));

        return new User(username);
    }

    public void CorrectUsername(string newUsername)
    {
        if (string.IsNullOrWhiteSpace(newUsername))
            throw new ArgumentException("Username cannot be empty", nameof(newUsername));

        Username = newUsername;
    }
}