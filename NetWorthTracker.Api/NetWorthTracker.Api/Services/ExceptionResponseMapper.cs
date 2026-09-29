namespace NetWorthTracker.Api.Services;

public sealed class ExceptionResponseMapper : IExceptionResponseMapper
{
    private const string UnexpectedErrorMessage =
        "An unexpected error occurred while processing the request.";

    public ExceptionResponse Map(Exception exception)
    {
        return exception switch
        {
            _ => new ExceptionResponse(
                StatusCodes.Status500InternalServerError,
                UnexpectedErrorMessage,
                UnexpectedErrorMessage)
        };
    }
}