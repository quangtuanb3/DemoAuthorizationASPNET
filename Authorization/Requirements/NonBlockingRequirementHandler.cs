using Microsoft.AspNetCore.Authorization;

namespace BookManagement.Authorization.Requirements;

internal class NonBlockingRequirementHandler(ILogger<NonBlockingRequirementHandler> logger,
    IUserContext userContext) : AuthorizationHandler<NonBlockingRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
        NonBlockingRequirement requirement)
    {
        var currentUser = userContext.GetCurrentUser();
        Console.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>handling...");
        var userStatus = context.User.FindFirst(AppClaimTypes.UserStatus)?.Value;
        logger.LogInformation("User: {Email}, status {Status} - Handling NonBlockingRequirement",

            currentUser!.Email,
            currentUser.Status);

        if (string.IsNullOrEmpty(userStatus))
        {
            logger.LogWarning("User status is null");
            Console.WriteLine("User status is null");
            context.Fail();
            return Task.CompletedTask;
        }

        if (requirement.AllowStatuses.Contains(userStatus, StringComparer.OrdinalIgnoreCase))
        {
            logger.LogInformation("Authorization succeeded");
            context.Succeed(requirement);
        }
        else
        {
            Console.WriteLine("Authorization faild status");
            context.Fail();
        }

        return Task.CompletedTask;
    }
}
