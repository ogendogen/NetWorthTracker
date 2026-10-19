namespace NetWorthTracker.Api.Services;

public interface IExceptionResponseMapper
{
    ExceptionResponse Map(Exception exception);
}
