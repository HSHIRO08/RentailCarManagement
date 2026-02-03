namespace RentailCarManagement.Constants;

/// <summary>
/// Car status constants
/// </summary>
public static class CarStatus
{
    public const string Available = nameof(Available);
    public const string Rented = nameof(Rented);
    public const string Maintenance = nameof(Maintenance);
    public const string Retired = nameof(Retired);
    public const string PendingApproval = nameof(PendingApproval);

    public static readonly string[] All = { Available, Rented, Maintenance, Retired, PendingApproval };

    public static bool IsValid(string status)
    {
        return All.Contains(status, StringComparer.OrdinalIgnoreCase);
    }
}

/// <summary>
/// Rental status constants
/// </summary>
public static class RentalStatus
{
    public const string Pending = nameof(Pending);
    public const string Confirmed = nameof(Confirmed);
    public const string Active = nameof(Active);
    public const string Completed = nameof(Completed);
    public const string Cancelled = nameof(Cancelled);

    public static readonly string[] All = { Pending, Confirmed, Active, Completed, Cancelled };

    public static readonly string[] ActiveStates = { Confirmed, Active };

    public static bool IsValid(string status)
    {
        return All.Contains(status, StringComparer.OrdinalIgnoreCase);
    }

    public static bool IsActive(string status)
    {
        return ActiveStates.Contains(status, StringComparer.OrdinalIgnoreCase);
    }
}

/// <summary>
/// Payment status constants
/// </summary>
public static class PaymentStatus
{
    public const string Pending = nameof(Pending);
    public const string Completed = nameof(Completed);
    public const string Failed = nameof(Failed);
    public const string Refunded = nameof(Refunded);

    public static readonly string[] All = { Pending, Completed, Failed, Refunded };

    public static bool IsValid(string status)
    {
        return All.Contains(status, StringComparer.OrdinalIgnoreCase);
    }
}
