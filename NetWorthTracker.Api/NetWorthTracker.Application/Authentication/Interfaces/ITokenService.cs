using FluentResults;
using NetWorthTracker.Application.User.Models.Login;

namespace NetWorthTracker.Application.Authentication.Interfaces;

public interface ITokenService
{
    LoginResponse CreateLoginResponse(string userName);

    string GenerateEmailToken(string userEmail);

    Result<string?> ValidateEmailToken(string token, out string email);
}