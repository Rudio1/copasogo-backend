using Copasogo.Application.Generic;
using Copasogo.WebApi.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Copasogo.WebApi.Controllers.Generic;

[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    protected IActionResult OkResponse<T>(T data) =>
        Ok(ApiResponse<T>.Ok(data));

    protected IActionResult CreatedResponse<T>(string uri, T data) =>
        Created(uri, ApiResponse<T>.Ok(data));

    protected IActionResult NoContentResponse() =>
        NoContent();

    protected IActionResult FromResult(Result result)
    {
        if (result.Success)
        {
            return NoContentResponse();
        }

        return FailResult(result.Error);
    }

    protected IActionResult FromResult<T>(Result<T> result)
    {
        if (result.Success && result.Value is not null)
        {
            return OkResponse(result.Value);
        }

        return FailResult(result.Error);
    }

    private IActionResult FailResult(string? error)
    {
        if (IsNotFound(error))
        {
            return NotFound(ApiResponse<object?>.Fail(new ApiError(ApiErrorCodes.NotFound, error ?? ApiMessages.RequisicaoInvalida)));
        }

        return BadRequest(ApiResponse<object?>.Fail(new ApiError(ApiErrorCodes.BusinessError, error ?? ApiMessages.RequisicaoInvalida)));
    }

    private static bool IsNotFound(string? error) =>
        error?.Contains(ApiMessages.MarcadorNaoEncontrado, StringComparison.OrdinalIgnoreCase) == true;
}
