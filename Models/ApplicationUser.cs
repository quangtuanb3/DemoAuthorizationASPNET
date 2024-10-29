using Microsoft.AspNetCore.Identity;

namespace BookManagement.Models;

public class ApplicationUser : IdentityUser
{
    public DateOnly? DateOfBirth { get; set; }
    public string? Nationality { get; set; }
    public string Status { get; set; } = "Activate";
}
