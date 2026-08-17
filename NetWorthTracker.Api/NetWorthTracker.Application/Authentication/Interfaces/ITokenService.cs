using NetWorthTracker.Application.User.Models.Login;

namespace NetWorthTracker.Application.Authentication.Interfaces;

public interface ITokenService
{
    LoginResponse CreateLoginResponse(string userName);
}