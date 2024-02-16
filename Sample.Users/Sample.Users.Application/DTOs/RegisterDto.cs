namespace Sample.Users.Application.DTOs;

public sealed record RegisterDto(string Email, string Password, string ConfirmPassword);