using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentailCarManagement.DTOs.Common;
using RentailCarManagement.Services;

namespace RentailCarManagement.Controllers;

/// <summary>
/// Admin Controller để quản lý và migrate password
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
[Produces("application/json")]
public class AdminController : ControllerBase
{
    private readonly ILogger<AdminController> _logger;

    public AdminController(
        ILogger<AdminController> logger)
    {
        _logger = logger;
    }
}

#region DTOs

public class PasswordMigrationResult
{
    public int MigratedCount { get; set; }
    public string DefaultPassword { get; set; } = null!;
    public string Message { get; set; } = null!;
}

public class HealthCheckResult
{
    public string Status { get; set; } = null!;
    public DateTime Timestamp { get; set; }
    public string Version { get; set; } = null!;
}

#endregion
