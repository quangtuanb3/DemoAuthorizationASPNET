using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using BookManagement.Data;
using BookManagement.Models;
using Microsoft.AspNetCore.Authorization;
using BookManagement.Constants;
using BookManagement.Authorization;

namespace BookManagement.Pages.Books
{
    //[Authorize(Policy = PolicyNames.HasNationality)]
    public class CreateModel : PageModel
    {
        private readonly BookManagement.Data.ApplicationDbContext _context;

        public CreateModel(BookManagement.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Book Book { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Books.Add(Book);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
