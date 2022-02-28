namespace Minimal.Application.Errors;

public class NotFoundError : Error
{
    public const string ErrorCode = "404";

    public NotFoundError(string message = "Not found")
        : base(message)
    {
    }
}
