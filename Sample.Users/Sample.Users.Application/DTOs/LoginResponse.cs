namespace Sample.Users.Application.DTOs;

public record LoginResponse(bool Succeeded, string Token, string Message);