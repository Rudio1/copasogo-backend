using Copasogo.WebApi.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Copasogo.WebApi.Controllers.Generic;

[Route(ApiRoutes.Health)]
public sealed class HealthController : ApiControllerBase
{
    [HttpGet]
    public IActionResult Get() => OkResponse(new { status = ApiMessages.HealthStatus });
}
