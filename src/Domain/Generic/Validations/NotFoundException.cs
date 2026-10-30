namespace Copasogo.Domain.Generic.Validations;

public sealed class NotFoundException : DomainException
{
    public NotFoundException(string message)
        : base(message)
    {
    }
}
