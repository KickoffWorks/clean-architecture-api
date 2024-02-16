using Sample.Users.Application.DTOs;

namespace Sample.Users.Application.Contracts;

public interface IUserService
{
    Task<GeneralResponse> CreateAccount(UserDto userDTO);

    Task<LoginResponse> LoginAccount(LoginDto loginDTO, string audience);
}