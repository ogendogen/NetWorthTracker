using NetWorthTracker.Application.User.Models.Login;

namespace NetWorthTracker.Application.Interfaces;

public interface ITokenService
{
    LoginResponse CreateLoginResponse(string userName);
}