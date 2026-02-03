using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using RentailCarManagement.DTOs.Auth;
using RentailCarManagement.Models;
using RentailCarManagement.Services.Interfaces;

namespace RentailCarManagement.Services.Implementations;

/// <summary>
/// Authentication Service Implementation using ASP.NET Core Identity
/// </summary>
public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        RoleManager<ApplicationRole> roleManager,
        ApplicationDbContext context,
        IConfiguration configuration,
        ILogger<AuthService> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _context = context;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        // Check if email exists
        var existingUser = await _userManager.FindByEmailAsync(request.Email);

        if (existingUser != null)
        {
            return new AuthResponse
            {
                Success = false,
                Message = "Email đã được sử dụng"
            };
        }

        // Create new user
        var user = new ApplicationUser
        {
            Email = request.Email,
            UserName = request.Email,
            FullName = request.FullName,
            PhoneNumber = request.Phone,
            EmailConfirmed = true,
            Status = "Active",
            CreatedAt = DateTime.UtcNow
        };

        // Create user with password hashing by Identity
        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToList();
            return new AuthResponse
            {
                Success = false,
                Message = string.Join(", ", errors)
            };
        }

        // Always assign Customer role by default
        // Only Admin can change user roles later
        const string defaultRole = "Customer";
        
        // Ensure role exists
        if (!await _roleManager.RoleExistsAsync(defaultRole))
        {
            await _roleManager.CreateAsync(new ApplicationRole(defaultRole));
        }
        
        await _userManager.AddToRoleAsync(user, defaultRole);

        // Create customer profile
        var customer = new Customer
        {
            CustomerId = Guid.NewGuid(),
            UserId = user.Id,
            CreatedAt = DateTime.UtcNow
        };
        await _context.Customers.AddAsync(customer);

        await _context.SaveChangesAsync();

        // Generate tokens
        var roles = await _userManager.GetRolesAsync(user);
        var accessToken = GenerateJwtToken(user, roles);
        var refreshToken = GenerateRefreshToken();

        return new AuthResponse
        {
            Success = true,
            Message = "Đăng ký thành công",
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(GetTokenExpiryMinutes()),
            User = new UserInfo
            {
                UserId = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                Phone = user.PhoneNumber,
                Roles = roles.ToList()
            }
        };
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null)
        {
            return new AuthResponse
            {
                Success = false,
                Message = "Email hoặc mật khẩu không đúng"
            };
        }

        // Check if user is active
        if (user.Status != "Active")
        {
            return new AuthResponse
            {
                Success = false,
                Message = "Tài khoản đã bị vô hiệu hóa"
            };
        }

        // Verify password with Identity
        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);

        if (result.IsLockedOut)
        {
            return new AuthResponse
            {
                Success = false,
                Message = "Tài khoản đã bị khóa do đăng nhập sai quá nhiều lần. Vui lòng thử lại sau."
            };
        }

        if (!result.Succeeded)
        {
            return new AuthResponse
            {
                Success = false,
                Message = "Email hoặc mật khẩu không đúng"
            };
        }

        var roles = await _userManager.GetRolesAsync(user);
        var accessToken = GenerateJwtToken(user, roles);
        var refreshToken = GenerateRefreshToken();

        return new AuthResponse
        {
            Success = true,
            Message = "Đăng nhập thành công",
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(GetTokenExpiryMinutes()),
            User = new UserInfo
            {
                UserId = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                Phone = user.PhoneNumber,
                Roles = roles.ToList()
            }
        };
    }

    public async Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request)
    {
        // Validate existing token
        var principal = GetPrincipalFromExpiredToken(request.AccessToken);
        if (principal == null)
        {
            return new AuthResponse
            {
                Success = false,
                Message = "Token không hợp lệ"
            };
        }

        var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
        {
            return new AuthResponse
            {
                Success = false,
                Message = "Token không hợp lệ"
            };
        }

        var userId = Guid.Parse(userIdClaim.Value);
        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user == null)
        {
            return new AuthResponse
            {
                Success = false,
                Message = "Người dùng không tồn tại"
            };
        }

        var roles = await _userManager.GetRolesAsync(user);
        var accessToken = GenerateJwtToken(user, roles);
        var refreshToken = GenerateRefreshToken();

        return new AuthResponse
        {
            Success = true,
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(GetTokenExpiryMinutes()),
            User = new UserInfo
            {
                UserId = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                Phone = user.PhoneNumber,
                Roles = roles.ToList()
            }
        };
    }

    public async Task<bool> ForgotPasswordAsync(ForgotPasswordRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
            return true; // Don't reveal if email exists

        // Generate password reset token
        var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
        _logger.LogInformation("Password reset token for {Email}: {Token}", request.Email, resetToken);

        // In real app, send this token via email
        return true;
    }

    public async Task<bool> ResetPasswordAsync(ResetPasswordRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
            return false;

        var result = await _userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);
        return result.Succeeded;
    }

    public async Task<bool> LogoutAsync(Guid userId)
    {
        await _signInManager.SignOutAsync();
        // In real app, invalidate refresh tokens here
        return true;
    }

    public async Task<bool> ConfirmEmailAsync(string email, string token)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
            return false;

        var result = await _userManager.ConfirmEmailAsync(user, token);
        return result.Succeeded;
    }

    #region Private Methods

    private string GenerateJwtToken(ApplicationUser user, IList<string> roles)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"] ?? "YourSuperSecretKeyHere12345678901234567890";
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email ?? ""),
            new(ClaimTypes.Name, user.FullName ?? ""),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        // Add role claims
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"] ?? "RentalCarManagement",
            audience: jwtSettings["Audience"] ?? "RentalCarManagement",
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(GetTokenExpiryMinutes()),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string GenerateRefreshToken()
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }

    private ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"] ?? "YourSuperSecretKeyHere12345678901234567890";

        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = false, // Don't validate lifetime for refresh
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"] ?? "RentalCarManagement",
            ValidAudience = jwtSettings["Audience"] ?? "RentalCarManagement",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                return null;
            }
            return principal;
        }
        catch
        {
            return null;
        }
    }

    private int GetTokenExpiryMinutes()
    {
        var expiry = _configuration.GetSection("JwtSettings")["ExpiryInMinutes"];
        return int.TryParse(expiry, out var minutes) ? minutes : 60;
    }

    #endregion
}
