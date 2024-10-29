using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BookManagement.Data;
using BookManagement.Models;
using BookManagement.Authorization;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Threading;
using System.Security.Claims;

namespace BookManagement.Pages.Profiles;

public class EditModel : PageModel
{
    private readonly IUserContext _userContext;
    private readonly IUserStore<ApplicationUser> _userStore;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    public EditModel(IUserContext userContext, IUserStore<ApplicationUser> userStore, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
    {
        _userContext = userContext;
        _userStore = userStore;
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [BindProperty]
    public EditProfileInputModel Input { get; set; } = new();

    public IActionResult OnGet()
    {
        var user = _userContext.GetCurrentUser();
        if (user == null) return NotFound("User not found.");

        Input = new EditProfileInputModel
        {
            Nationality = user.Nationality,
            DateOfBirth = user.DateOfBirth
        };

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var user = _userContext.GetCurrentUser();
        if (user == null) return NotFound("User not found.");

        var dbUser = await _userStore.FindByIdAsync(user!.Id, CancellationToken.None);

        dbUser.Nationality = Input.Nationality;
        dbUser.DateOfBirth = Input.DateOfBirth;

        var result = await _userStore.UpdateAsync(dbUser, CancellationToken.None);
        if (result.Succeeded)
        {
            await UpdateUserClaims(dbUser);

            await _signInManager.RefreshSignInAsync(dbUser);

            TempData["SuccessMessage"] = "Profile updated successfully.";

            return RedirectToPage("Index");
        }

        foreach (var error in result.Errors)
            ModelState.AddModelError(string.Empty, error.Description);

        return Page();
    }

    public class EditProfileInputModel
    {
        [Required(ErrorMessage = "Please enter your nationality.")]
        public string Nationality { get; set; } = default!;

        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateOnly? DateOfBirth { get; set; }
    }

    public async Task UpdateUserClaims(ApplicationUser user)
    {
        // Assuming you have access to UserManager<ApplicationUser> here
        var userClaims = await _userManager.GetClaimsAsync(user);

        // Remove existing claims
        await _userManager.RemoveClaimsAsync(user, userClaims);

        // Create new claims
        var nationalityClaim = new Claim("Nationality", user.Nationality);
        var dateOfBirthClaim = new Claim("DateOfBirth", user.DateOfBirth?.ToString("yyyy-MM-dd"));

        // Add updated claims
        await _userManager.AddClaimAsync(user, nationalityClaim);
        await _userManager.AddClaimAsync(user, dateOfBirthClaim);
    }
}