using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RentailCarManagement.DTOs.Common;
using RentailCarManagement.Models;

namespace RentailCarManagement.Controllers;

/// <summary>
/// API Controller cho quản lý Roles (Admin only)
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize(Roles = "Admin")]
public class RolesController : ControllerBase
{
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;
    private readonly ILogger<RolesController> _logger;

    public RolesController(
        RoleManager<ApplicationRole> roleManager,
        UserManager<ApplicationUser> userManager,
        ApplicationDbContext context,
        ILogger<RolesController> logger)
    {
        _roleManager = roleManager;
        _userManager = userManager;
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Lấy danh sách tất cả roles
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<RoleDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllRoles()
    {
        var roles = await _roleManager.Roles
            .Select(r => new RoleDto
            {
                Id = r.Id,
                Name = r.Name,
                Description = r.Description,
                CreatedAt = r.CreatedAt
            })
            .ToListAsync();

        return Ok(ApiResponse<IEnumerable<RoleDto>>.SuccessResult(roles));
    }

    /// <summary>
    /// Tạo role mới
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<RoleDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateRole([FromBody] CreateRoleRequest request)
    {
        if (await _roleManager.RoleExistsAsync(request.Name))
        {
            return BadRequest(ApiResponse.FailResult("Role đã tồn tại"));
        }

        var role = new ApplicationRole(request.Name)
        {
            Description = request.Description,
            CreatedAt = DateTime.UtcNow
        };

        var result = await _roleManager.CreateAsync(role);

        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToList();
            return BadRequest(ApiResponse.FailResult("Không thể tạo role", errors));
        }

        return CreatedAtAction(nameof(GetAllRoles), null,
            ApiResponse<RoleDto>.SuccessResult(new RoleDto
            {
                Id = role.Id,
                Name = role.Name,
                Description = role.Description,
                CreatedAt = role.CreatedAt
            }, "Tạo role thành công"));
    }

    /// <summary>
    /// Xóa role
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteRole(Guid id)
    {
        var role = await _roleManager.FindByIdAsync(id.ToString());
        if (role == null)
        {
            return NotFound(ApiResponse.FailResult("Role không tồn tại"));
        }

        // Không cho xóa roles mặc định
        if (new[] { "Admin", "Customer", "Supplier", "Staff" }.Contains(role.Name))
        {
            return BadRequest(ApiResponse.FailResult("Không thể xóa role mặc định"));
        }

        var result = await _roleManager.DeleteAsync(role);
        if (!result.Succeeded)
        {
            return BadRequest(ApiResponse.FailResult("Không thể xóa role"));
        }

        return Ok(ApiResponse.SuccessResult("Xóa role thành công"));
    }

    /// <summary>
    /// Gán role cho user
    /// </summary>
    [HttpPost("assign")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AssignRoleToUser([FromBody] AssignRoleRequest request)
    {
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());
        if (user == null)
        {
            return NotFound(ApiResponse.FailResult("User không tồn tại"));
        }

        if (!await _roleManager.RoleExistsAsync(request.RoleName))
        {
            return BadRequest(ApiResponse.FailResult("Role không tồn tại"));
        }

        if (await _userManager.IsInRoleAsync(user, request.RoleName))
        {
            return BadRequest(ApiResponse.FailResult("User đã có role này"));
        }

        var result = await _userManager.AddToRoleAsync(user, request.RoleName);
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToList();
            return BadRequest(ApiResponse.FailResult("Không thể gán role", errors));
        }

        // Auto-create profile based on role
        await CreateProfileForRole(user.Id, request.RoleName);

        return Ok(ApiResponse.SuccessResult($"Đã gán role {request.RoleName} cho user"));
    }

    /// <summary>
    /// Bỏ role khỏi user
    /// </summary>
    [HttpPost("remove")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RemoveRoleFromUser([FromBody] AssignRoleRequest request)
    {
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());
        if (user == null)
        {
            return NotFound(ApiResponse.FailResult("User không tồn tại"));
        }

        if (!await _userManager.IsInRoleAsync(user, request.RoleName))
        {
            return BadRequest(ApiResponse.FailResult("User không có role này"));
        }

        var result = await _userManager.RemoveFromRoleAsync(user, request.RoleName);
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToList();
            return BadRequest(ApiResponse.FailResult("Không thể bỏ role", errors));
        }

        return Ok(ApiResponse.SuccessResult($"Đã bỏ role {request.RoleName} khỏi user"));
    }

    /// <summary>
    /// Lấy danh sách users theo role
    /// </summary>
    [HttpGet("{roleName}/users")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<UserRoleDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUsersInRole(string roleName)
    {
        if (!await _roleManager.RoleExistsAsync(roleName))
        {
            return NotFound(ApiResponse.FailResult("Role không tồn tại"));
        }

        var usersInRole = await _userManager.GetUsersInRoleAsync(roleName);
        var userDtos = usersInRole.Select(u => new UserRoleDto
        {
            UserId = u.Id,
            Email = u.Email,
            FullName = u.FullName,
            Status = u.Status
        });

        return Ok(ApiResponse<IEnumerable<UserRoleDto>>.SuccessResult(userDtos));
    }

    /// <summary>
    /// Lấy roles của user
    /// </summary>
    [HttpGet("user/{userId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<string>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUserRoles(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            return NotFound(ApiResponse.FailResult("User không tồn tại"));
        }

        var roles = await _userManager.GetRolesAsync(user);
        return Ok(ApiResponse<IEnumerable<string>>.SuccessResult(roles));
    }

    #region Helper Methods

    /// <summary>
    /// Tự động tạo profile tương ứng khi gán role
    /// </summary>
    private async Task CreateProfileForRole(Guid userId, string roleName)
    {
        try
        {
            if (roleName == "Supplier")
            {
                // Kiểm tra xem đã có Supplier profile chưa
                var existingSupplier = await _context.Suppliers
                    .FirstOrDefaultAsync(s => s.UserId == userId);

                if (existingSupplier == null)
                {
                    var supplier = new Supplier
                    {
                        SupplierId = Guid.NewGuid(),
                        UserId = userId,
                        IsVerified = false,
                        CreatedAt = DateTime.UtcNow
                    };
                    await _context.Suppliers.AddAsync(supplier);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Created Supplier profile for user {UserId}", userId);
                }
            }
            else if (roleName == "Customer")
            {
                // Kiểm tra xem đã có Customer profile chưa
                var existingCustomer = await _context.Customers
                    .FirstOrDefaultAsync(c => c.UserId == userId);

                if (existingCustomer == null)
                {
                    var customer = new Customer
                    {
                        CustomerId = Guid.NewGuid(),
                        UserId = userId,
                        CreatedAt = DateTime.UtcNow
                    };
                    await _context.Customers.AddAsync(customer);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Created Customer profile for user {UserId}", userId);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create profile for role {RoleName} and user {UserId}", roleName, userId);
        }
    }

    #endregion
}

#region DTOs

public class RoleDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public DateTime? CreatedAt { get; set; }
}

public class CreateRoleRequest
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
}

public class AssignRoleRequest
{
    public Guid UserId { get; set; }
    public string RoleName { get; set; } = null!;
}

public class UserRoleDto
{
    public Guid UserId { get; set; }
    public string? Email { get; set; }
    public string? FullName { get; set; }
    public string? Status { get; set; }
}

#endregion
