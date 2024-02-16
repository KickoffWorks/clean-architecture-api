using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using NSubstitute;
using Sample.Core.Domain.Entities;
using Sample.Core.Domain.Settings;
using Sample.Users.Application.Contracts;
using Sample.Users.Application.DTOs;
using Sample.Users.Application.Services;

namespace Sample.Users.Tests.Unit;

public class UserServiceTests
{
    private readonly IUserService _userService;
    private readonly UserManager<Core.Domain.Entities.ApplicationUser> _userManager;
    private readonly IFixture _fixture = new Fixture();

    public UserServiceTests()
    {
        _userManager = Substitute.For<UserManager<Core.Domain.Entities.ApplicationUser>>(
            Substitute.For<IUserStore<Core.Domain.Entities.ApplicationUser>>(),
            null, null, null, null, null, null, null, null);
        
        _fixture.Register(() =>
        {
            var applicationSettings = _fixture.Create<ApplicationSettings>(); // Create an instance of ApplicationSettings
            
            return Options.Create(applicationSettings); // Return the IOptions<ApplicationSettings> instance
        });
        
        var options = _fixture.Create<IOptions<ApplicationSettings>>();

        _userService = new UserService(_userManager, options);
    }
    
    [Fact]
    public async Task CreateAccount_ShouldReturnSuccessResponse_WhenUserCreationSucceeds()
    {
        // Arrange
        var userDto = _fixture.Create<UserDto>();
        _userManager.FindByEmailAsync(Arg.Any<string>()).Returns((Core.Domain.Entities.ApplicationUser?)null);
        _userManager.CreateAsync(Arg.Any<Core.Domain.Entities.ApplicationUser>(), Arg.Any<string>()).Returns(IdentityResult.Success);

        // Act
        var response = await _userService.CreateAccount(userDto);

        // Assert
        response.Should().NotBeNull();
        response.Succeeded.Should().BeTrue();
        response.Message.Should().Be("Account has been created.");
    }
    
    [Fact]
    public async Task LoginAccount_ShouldReturnSuccessResponse_WhenUserExists()
    {
        // Arrange
        var userDto = _fixture.Create<LoginDto>();
        
        var appUser = new ApplicationUser()
        {
            Email = userDto.Email,
            PasswordHash = userDto.Password
        };

        _userManager.FindByEmailAsync(userDto.Email).Returns(appUser);
        
        _userManager.CheckPasswordAsync(appUser, userDto.Password).Returns(true);
        
        // Act
        var response = await _userService.LoginAccount(userDto, "sample");

        // Assert
        response.Should().NotBeNull();
        response.Succeeded.Should().BeTrue();
        response.Message.Should().Be("Login successful.");
    }
    
}