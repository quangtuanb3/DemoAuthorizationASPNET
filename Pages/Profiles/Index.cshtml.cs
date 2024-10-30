
using Microsoft.AspNetCore.Mvc.RazorPages;

using BookManagement.Authorization;

namespace BookManagement.Pages.Profiles;

public class IndexModel : PageModel
{
    private readonly IUserContext _userContext;

    public CurrentUser CurrentUser { get; set; }

    public IndexModel(IUserContext userContext)
    {
        _userContext = userContext;
    }

    public void OnGet()
    {
        CurrentUser = _userContext.GetCurrentUser();
        Console.WriteLine(CurrentUser);
    }
}