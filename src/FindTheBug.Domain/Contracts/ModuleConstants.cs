namespace FindTheBug.Domain.Contracts;

public static class ModuleConstants
{
    public const string Laboratory = "Laboratory";
    public const string Doctors = "Doctors";
    public const string Dispensary = "Dispensary";
    public const string Billing = "Billing";
    public const string UserManagement = "UserManagement";
    public const string Patient = "Patient";

    public static IEnumerable<string> GetModules() =>
    [
        Laboratory, Doctors, Dispensary, Billing, UserManagement
    ];
}
