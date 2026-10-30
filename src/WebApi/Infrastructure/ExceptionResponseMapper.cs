using System.Net;
using Copasogo.Domain.Generic.Validations;
using DomainValidationException = Copasogo.Domain.Generic.Validations.ValidationException;

namespace Copasogo.WebApi.Infrastructure;

public static class ExceptionResponseMapper
{
    public static (int StatusCode, ApiResponse<object?> Response) Map(Exception exception)
    {
        return exception switch
        {
            DomainValidationException domainValidation => (
                StatusCodes.Status400BadRequest,
                ApiResponse<object?>.Fail(new ApiError(
                    ApiErrorCodes.ValidationError,
                    domainValidation.Message,
                    domainValidation.Errors?
                        .SelectMany(pair => pair.Value.Select(message => new FieldError(pair.Key, message)))
                        .ToArray()))),

            FluentValidation.ValidationException fluentValidation => (
                StatusCodes.Status400BadRequest,
                ApiResponse<object?>.Fail(new ApiError(
                    ApiErrorCodes.ValidationError,
                    "Falha de validacao.",
                    fluentValidation.Errors
                        .Select(error => new FieldError(error.PropertyName, error.ErrorMessage))
                        .ToArray()))),

            BusinessException business => (
                StatusCodes.Status422UnprocessableEntity,
                ApiResponse<object?>.Fail(new ApiError(ApiErrorCodes.BusinessError, business.Message))),

            NotFoundException notFound => (
                StatusCodes.Status404NotFound,
                ApiResponse<object?>.Fail(new ApiError(ApiErrorCodes.NotFound, notFound.Message))),

            UnauthorizedException unauthorized => (
                StatusCodes.Status403Forbidden,
                ApiResponse<object?>.Fail(new ApiError(ApiErrorCodes.Unauthorized, unauthorized.Message))),

            DomainException domain => (
                StatusCodes.Status422UnprocessableEntity,
                ApiResponse<object?>.Fail(new ApiError(ApiErrorCodes.BusinessError, domain.Message))),

            _ => (
                StatusCodes.Status500InternalServerError,
                ApiResponse<object?>.Fail(new ApiError(
                    ApiErrorCodes.InternalError,
                    "Houve um erro ao processar sua requisicao. Entre em contato com o suporte")))
        };
    }
}
