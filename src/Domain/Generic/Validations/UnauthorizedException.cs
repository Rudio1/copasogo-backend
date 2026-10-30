namespace Copasogo.Domain.Generic.Validations;

public sealed class UnauthorizedException : DomainException
{
    public UnauthorizedException(string message)
        : base(message)
    {
    }
}
