using Microsoft.AspNetCore.Identity;

namespace Copasogo.Infra.Data.Auth.Identity;

public sealed class ApplicationUser : IdentityUser<Guid>
{
    public string Nome { get; set; } = string.Empty;
}
