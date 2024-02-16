namespace Sample.Users.Application.DTOs;

public sealed record UserDto(string? Id, string Name , string Email, string Password, string ConfirmPassword);