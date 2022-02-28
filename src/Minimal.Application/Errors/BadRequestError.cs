namespace Minimal.Application.Errors;

public class BadRequestError : Error
{
    public const string ErrorCode = "400";

    public BadRequestError(string message)
        : base(message)
    {
    }
}
