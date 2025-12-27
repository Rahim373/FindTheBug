namespace FindTheBug.Domain.Contracts;

public static class ModuleConstants
{
    public const string Laboratory = "Laboratory";
    public const string Doctors = "Doctors";
    public const string Dispensary = "Dispensary";
    public const string Reception = "Reception";
    public const string UserManagement = "UserManagement";
    public const string Patient = "Patient";
    public const string Accounts = "Accounts";

    public static IEnumerable<string> GetModules() =>
    [
        Accounts,
        Reception,
        Doctors, 
        Dispensary, 
        Laboratory, 
        UserManagement
    ];
}
