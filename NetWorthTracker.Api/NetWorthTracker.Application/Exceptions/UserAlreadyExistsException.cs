namespace NetWorthTracker.Application.Exceptions;

public class UserAlreadyExistsException() : Exception("User with provided credentials already exists.")
{
}