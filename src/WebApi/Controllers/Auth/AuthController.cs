using Copasogo.Application.Auth.Commands.Login;
using Copasogo.Application.Generic;
using Copasogo.WebApi.Controllers.Generic;
using Copasogo.WebApi.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Copasogo.WebApi.Controllers.Auth;

[Route("api/auth")]
public sealed class AuthController(ILoginHandler loginHandler) : ApiControllerBase
{
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand command, CancellationToken cancellationToken)
    {
        var result = await loginHandler.HandleAsync(command, cancellationToken);
        if (!result.Success || result.Value is null)
        {
            return Unauthorized(ApiResponse<LoginResponse>.Fail(new ApiError(ApiErrorCodes.Unauthorized, result.Error ?? "E-mail ou senha invalidos.")));
        }

        return OkResponse(result.Value);
    }
}
