using Microsoft.AspNetCore.Authorization;

namespace BookManagement.Authorization.Requirements;


public class NonBlockingRequirement(List<string> allowStatuses) : IAuthorizationRequirement
{
    public List<string> AllowStatuses { get; } = allowStatuses ?? throw new ArgumentNullException(nameof(allowStatuses));
}