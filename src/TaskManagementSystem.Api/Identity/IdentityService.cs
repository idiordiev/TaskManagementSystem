using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TaskManagementSystem.BLL.Exceptions;
using TaskManagementSystem.BLL.Interfaces;

namespace TaskManagementSystem.Api.Identity;

public class IdentityService : IIdentityService
{
    private readonly JwtSettings _jwtSettings;
    private readonly UserManager<Account> _userManager;

    private static readonly TimeSpan TokenLifetime = TimeSpan.FromMinutes(20);

    public IdentityService(UserManager<Account> userManager, IOptions<JwtSettings> jwtSettings)
    {
        _userManager = userManager;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task CreateAccountAsync(string email, string password, int userId)
    {
        var account = new Account
        {
            UserName = email,
            Email = email,
            UserId = userId,
        };

        await _userManager.CreateAsync(account);
        await _userManager.AddPasswordAsync(account, password);
    }

    public async Task<string> GetTokenAsync(string email, string password)
    {
        var user = await _userManager.Users.Where(x => x.Email == email).FirstOrDefaultAsync();

        if (user is null)
        {
            throw new NotFoundException($"User with email {email} has not been found");
        }
        
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_jwtSettings.SecretKey);

        var claims = new List<Claim>()
        {
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Sub, user.Email),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(IdentityClaims.UserIdClaim, user.UserId.ToString()),
            new Claim(IdentityClaims.IsAdminClaim, (await _userManager.IsInRoleAsync(user, "Admin")).ToString())
        };

        foreach (var role in await _userManager.GetRolesAsync(user))
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var tokenDescriptor = new SecurityTokenDescriptor()
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.Add(TokenLifetime),
            Issuer = _jwtSettings.ValidIssuer,
            Audience = _jwtSettings.ValidAudience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);

        var jwt = tokenHandler.WriteToken(token);

        return jwt;
    }

    public async Task DeleteAccountsForUserAsync(int userId, CancellationToken cancellationToken = default)
    {
        var accountsToDelete = await _userManager.Users.Where(x => x.UserId == userId).ToListAsync(cancellationToken);

        foreach (var account in accountsToDelete)
        {
            await _userManager.DeleteAsync(account);
        }
    }
}