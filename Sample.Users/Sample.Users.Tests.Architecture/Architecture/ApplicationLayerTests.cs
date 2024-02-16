using NetArchTest.Rules;

namespace Sample.Users.Tests.Architecture.Architecture;

public class ApplicationLayerTests
{
    [Fact]
    public void ApplicationLayer_ShouldNot_AccessInfrastructureLayer()
    {
        // Arrange
        var applicationLayer = typeof(Sample.Users.Application.Services.UserService).Assembly;

        // Act
        var result = Types.InAssembly(applicationLayer)
            .ShouldNot()
            .HaveDependencyOn("Sample.Core.Infrastructure")
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful);
    }

    [Fact]
    public void ApplicationLayer_ShouldNot_AccessPresentationLayer()
    {
        // Arrange
        var applicationLayer = typeof(Sample.Users.Application.Services.UserService).Assembly;

        // Act
        var result = Types.InAssembly(applicationLayer)
            .ShouldNot()
            .HaveDependencyOn("Sample.Users.Api")
            .GetResult();

        // Assert
        Assert.True(result.IsSuccessful);
    }
}