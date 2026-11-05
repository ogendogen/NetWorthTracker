namespace NetWorthTracker.Application.User.Models.ResetPassword;

public record ResetPasswordRequest(string Token, string Password, string ConfirmPassword);
