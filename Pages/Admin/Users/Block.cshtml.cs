using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using BookManagement.Models;

namespace BookManagement.Pages.Admin.Users
{
    public class BlockModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public BlockModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [BindProperty(SupportsGet = true)]
        public string id { get; set; }

        public ApplicationUser UserToBlock { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound("User ID not provided.");
            }

            UserToBlock = await _userManager.FindByIdAsync(id);
            if (UserToBlock == null)
            {
                return NotFound("User not found.");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound("User ID not provided.");
            }

            UserToBlock = await _userManager.FindByIdAsync(id);
            if (UserToBlock == null)
            {
                return NotFound("User not found.");
            }

            UserToBlock.Status = UserToBlock.Status == "Blocked" ? "Activate" : "Blocked";
            var result = await _userManager.UpdateAsync(UserToBlock);

            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Failed to block the user.");
                return Page();
            }

            return RedirectToPage("/Admin/Users/Index");
        }
    }
}
