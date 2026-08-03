namespace Support.Auth.Id.Exceptions;

public sealed class BusinessValidationException : Exception
{
    public BusinessValidationException(string message)
        : base(message) { }
}
