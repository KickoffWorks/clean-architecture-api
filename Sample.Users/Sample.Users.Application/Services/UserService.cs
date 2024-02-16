using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Sample.Core.Domain.Entities;
using Sample.Users.Application.Contracts;
using Sample.Users.Application.DTOs;

namespace Sample.Users.Application.Services;

public class UserService : IUserService
{
    private readonly UserManager<Sample.Core.Domain.Entities.ApplicationUser> _userManager;
    private readonly IOptions<Sample.Core.Domain.Settings.ApplicationSettings> _config;
    
    public UserService(UserManager<ApplicationUser> userManager, IOptions<Sample.Core.Domain.Settings.ApplicationSettings> config)
    {
        _userManager = userManager;
        _config = config;
    }
    
    public async Task<GeneralResponse> CreateAccount(UserDto userDTO)
    {
        var newUser = new Sample.Core.Domain.Entities.ApplicationUser()
        {
            Email = userDTO.Email,
            UserName = userDTO.Email,
            PasswordHash = userDTO.Password,
        };

        var user = await _userManager.FindByEmailAsync(newUser.Email);
        
        if (user is not null) return new GeneralResponse(false, "User already exists.");

        var createUser = await _userManager.CreateAsync(newUser, userDTO.Password);
        
        if (!createUser.Succeeded) return new GeneralResponse(false, "Error occured, try again.");
        
        return new GeneralResponse(true, "Account has been created.");
    }

    public async Task<LoginResponse> LoginAccount(LoginDto loginDTO, string audience)
    {
        var existingUser = await _userManager.FindByEmailAsync(loginDTO.Email);
        
        if (existingUser is null) return new LoginResponse(false, null!, "User doesn't exist.");

        var checkUserPassword = await _userManager.CheckPasswordAsync(existingUser, loginDTO.Password);
        
        if (!checkUserPassword) return new LoginResponse(false, null!, "Invalid email/password.");

        var userSession = new UserSessionDto(existingUser.Id, existingUser.Email);
        
        var token = GenerateToken(userSession, audience);
        
        return new LoginResponse(true, token, "Login successful.");
    }
    
    private string GenerateToken(UserSessionDto user, string audience)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.Value.AuthenticationSettings.IssuerSigningKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var userClaims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id!),
            new Claim(ClaimTypes.Email, user.Email!)
        };

        var token = new JwtSecurityToken(
            issuer: _config.Value.AuthenticationSettings.Issuer,
            audience: audience,
            claims: userClaims,
            expires: DateTime.Now.AddDays(1),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}