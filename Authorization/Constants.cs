namespace BookManagement.Authorization;

public static class PolicyNames
{
    public const string HasNationality = "HasNationality";
    public const string AtLeast18 = "AtLeast18";
    public const string NonBlocking = "NonBlocking";
}

public static class AppClaimTypes
{
    public const string Nationality = "Nationality";
    public const string DateOfBirth = "DateOfBirth";
    public const string UserStatus = "Status";
}



