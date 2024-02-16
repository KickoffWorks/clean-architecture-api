using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NSubstitute;
using Sample.Core.Domain.Settings;
using Sample.Core.Infrastructure;
using Sample.Users.Application.DTOs;
using Sample.Users.Application.Services;
using Testcontainers.PostgreSql;

namespace Sample.Users.Tests.Integration.Integration;

public class UserServiceIntegrationTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
        .WithImage("postgres:15-alpine")
        .WithDatabase("test")
        .WithUsername("test")
        .WithPassword("test")
        .Build();
    
    private readonly Fixture _fixture = new();
    private UserManager<Core.Domain.Entities.ApplicationUser> _userManager;
    
    [Fact]
    public async Task CreateAccount_Returns_SuccessfulResponse_When_UserDoesNotExist()
    {
        // Arrange        
        var config = Substitute.For<IOptions<ApplicationSettings>>();
        
        var userService = new UserService(_userManager, config);
        
        var userDTO = _fixture.Create<UserDto>();

        // Act
        var response = await userService.CreateAccount(userDTO);

        // Assert
        response.Should().NotBeNull();
        response.Succeeded.Should().BeTrue();
        response.Message.Should().Be("Account has been created.");
    }
    
    [Fact]
    public async Task LoginAccount_Returns_SuccessfulResponse_When_UserLogins()
    {
        // Arrange  
        
        _fixture.Register(() =>
        {
            var applicationSettings = _fixture.Create<ApplicationSettings>(); // Create an instance of ApplicationSettings
            
            return Options.Create(applicationSettings); // Return the IOptions<ApplicationSettings> instance
        });
        
        var options = _fixture.Create<IOptions<ApplicationSettings>>();
        
        var userService = new UserService(_userManager, options);
        
        var userDTO = _fixture.Create<UserDto>();
        
        await userService.CreateAccount(userDTO);

        var loginDto = new LoginDto(userDTO.Email, userDTO.Password);

        // Act
        var response = await userService.LoginAccount(loginDto, "test");

        // Assert
        response.Should().NotBeNull();
        response.Succeeded.Should().BeTrue();
        response.Message.Should().Be("Login successful.");
    }
    
    
    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();

        // Set up the DbContext using the connection string provided by the PostgreSQL container
        var connectionString = _postgres.GetConnectionString();
        var services = new ServiceCollection();

        services.AddDbContext<Sample.Core.Infrastructure.ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString));
        
        services.AddIdentityCore<Core.Domain.Entities.ApplicationUser>()
            .AddEntityFrameworkStores<ApplicationDbContext>();
        
        var serviceProvider = services.BuildServiceProvider();
        _userManager = serviceProvider.GetRequiredService<UserManager<Core.Domain.Entities.ApplicationUser>>();
        
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await dbContext.Database.MigrateAsync();
    }
    
    public Task DisposeAsync()
    {
        return _postgres.DisposeAsync().AsTask();
    }
}