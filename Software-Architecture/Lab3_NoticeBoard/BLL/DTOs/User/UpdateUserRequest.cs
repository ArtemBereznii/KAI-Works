namespace BLL.DTOs.User;

public record UpdateUserRequest(
    Guid Id,
    string? Username
);