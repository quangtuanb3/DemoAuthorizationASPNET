namespace BookManagement.Authorization;

public record CurrentUser(
    string Id,
    string Email,
    IEnumerable<string> Roles,
    string? Nationality,
    DateOnly? DateOfBirth,
    string? Status
    )
{
    public bool IsInRole(string role) => Roles.Contains(role);
}
