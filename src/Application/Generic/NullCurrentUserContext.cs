using Copasogo.Application.Generic.Services.Interfaces;

namespace Copasogo.Application.Generic;

public sealed class NullCurrentUserContext : ICurrentUserContext
{
    public Guid? UserId => null;

    public bool IsAuthenticated => false;

    public IReadOnlyCollection<string> Roles => [];
}
