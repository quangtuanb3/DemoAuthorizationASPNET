using BookManagement.Authorization;
using BookManagement.Constants;
using BookManagement.Models;
using Microsoft.Extensions.Logging;


namespace BookManagement.Authorization.Services;

public class BookAuthorizationService(ILogger<BookAuthorizationService> logger,
    IUserContext userContext)
{
    public bool Authorize(Book book, ResourceOperation resourceOperation)
    {
        var user = userContext.GetCurrentUser();

        logger.LogInformation("Authorizing user {UserEmail}, to {Operation} for book {BookTitle}",
            user.Email,
            resourceOperation,
            book.Title);

        if (resourceOperation == ResourceOperation.Read || resourceOperation == ResourceOperation.Create)
        {
            logger.LogInformation("Create/read operation - successful authorization");
            return true;
        }

        if (resourceOperation == ResourceOperation.Delete && user.IsInRole(UserRoles.Admin))
        {
            logger.LogInformation("Admin User, delete operation - successful authorization");
            return true;
        }

        if (resourceOperation == ResourceOperation.Update && user.IsInRole(UserRoles.Manager))
        {
            logger.LogInformation("Manager User - successful authorization");
            return true;
        }

        return false;
    }
}
