using System.Security.Claims;
using Copasogo.Application.Generic.Services.Interfaces;

namespace Copasogo.WebApi.Infrastructure;

public sealed class CurrentUserContext(IHttpContextAccessor httpContextAccessor) : ICurrentUserContext
{
    public Guid? UserId
    {
        get
        {
            var value = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? httpContextAccessor.HttpContext?.User.FindFirstValue("sub");

            return Guid.TryParse(value, out var userId) ? userId : null;
        }
    }

    public bool IsAuthenticated =>
        httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated == true;

    public IReadOnlyCollection<string> Roles
    {
        get
        {
            var user = httpContextAccessor.HttpContext?.User;
            if (user is null)
            {
                return [];
            }

            return user.FindAll("role")
                .Concat(user.FindAll(ClaimTypes.Role))
                .Select(claim => claim.Value)
                .Distinct()
                .ToArray();
        }
    }
}
