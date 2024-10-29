using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookManagement.Models;
using BookManagement.Authorization;

namespace BookManagement.Pages.Admin.Users
{
    [Authorize(Roles = "Admin")]
    public class EditModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUserContext _userContext;

        public EditModel(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IUserContext userContext)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _userContext = userContext;
        }

        [BindProperty]
        public EditUserRoleInputModel Input { get; set; } = new();

        public IList<string> AvailableRoles { get; set; } = new List<string>();

        public async Task<IActionResult> OnGetAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound("User not found.");

            Input.UserId = user.Id;
            Input.Email = user.Email;
            Input.SelectedRoles = (List<string>)await _userManager.GetRolesAsync(user);

            // Get all available roles
            AvailableRoles = await _roleManager.Roles.Select(r => r.Name).ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.FindByIdAsync(Input.UserId);
            if (user == null) return NotFound("User not found.");

            var currentRoles = await _userManager.GetRolesAsync(user);
            var result = await _userManager.RemoveFromRolesAsync(user, currentRoles);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
                return Page();
            }

            // Add the selected roles
            result = await _userManager.AddToRolesAsync(user, Input.SelectedRoles);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "User roles updated successfully.";
                return RedirectToPage("./Index");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return Page();
        }

        public class EditUserRoleInputModel
        {
            public string UserId { get; set; } = default!;
            public string Email { get; set; } = default!;
            public List<string> SelectedRoles { get; set; } = new();
        }
    }
}
