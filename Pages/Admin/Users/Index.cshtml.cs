using BookManagement.Constants;
using BookManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;


namespace BookManagement.Pages.Admin.Users;

[Authorize(Roles = UserRoles.Admin)]
public class IndexModel : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;

    public IndexModel(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public List<UserWithRoles> UsersWithRoles { get; set; } = new();

    public async Task OnGetAsync()
    {
        var users = await _userManager.Users.ToListAsync();

        // Populate the UsersWithRoles list with user data and their roles
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            UsersWithRoles.Add(new UserWithRoles
            {
                User = user,
                Roles = roles
            });
        }
    }

    public class UserWithRoles
    {
        public ApplicationUser User { get; set; } = default!;
        public IEnumerable<string> Roles { get; set; } = new List<string>();
    }
}