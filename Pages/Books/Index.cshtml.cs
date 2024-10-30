using BookManagement.Authorization;
using BookManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BookManagement.Pages.Books;

//[Authorize(Policy = PolicyNames.NonBlocking)]
public class IndexModel : PageModel
{
    private readonly BookManagement.Data.ApplicationDbContext _context;

    public IndexModel(BookManagement.Data.ApplicationDbContext context)
    {
        _context = context;
    }

    public IList<Book> Book { get; set; } = default!;

    public async Task OnGetAsync()
    {
        Book = await _context.Books.ToListAsync();
    }
}
