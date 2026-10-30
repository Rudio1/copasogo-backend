namespace Copasogo.Domain.Generic.Validations;

public class DomainException : Exception
{
    public DomainException(string message)
        : base(message)
    {
    }
}
