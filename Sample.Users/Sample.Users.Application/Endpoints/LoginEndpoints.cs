using Sample.Users.Application.Contracts;
using Sample.Users.Application.DTOs;

namespace Sample.Users.Application.Endpoints;

public static class EndpointExtensions
{
    public static void MapUserEndpoints(this IEndpointRouteBuilder group)
    {
        group.MapPost("/register", async (RegisterDto registerDto, IUserService userService) =>
        {
            var userDto = new UserDto(null, registerDto.Email, registerDto.Email, registerDto.Password,
                registerDto.ConfirmPassword);
            
            var response = await userService.CreateAccount(userDto);

            return response.Succeeded ? Results.Ok(response) : Results.BadRequest();
        });
        
        group.MapPost("/login", async (LoginDto loginDto, IUserService userService) =>
        {
            var response = await userService.LoginAccount(loginDto, "sample");

            return response.Succeeded ? Results.Ok(response) : Results.BadRequest();
        });
    }
}