namespace Copasogo.WebApi.Infrastructure;

public sealed record ApiError(string Code, string Message, IReadOnlyList<FieldError>? Fields = null);

public sealed record FieldError(string Field, string Message);

public sealed record ApiResponse<T>(bool Success, T? Data, ApiError? Error)
{
    public static ApiResponse<T> Ok(T data) => new(true, data, null);

    public static ApiResponse<T> Fail(ApiError error) => new(false, default, error);
}

public sealed record PagedResponse<T>(
    IReadOnlyList<T> Items,
    int Page,
    int PageSize,
    int TotalCount);

public static class ApiErrorCodes
{
    public const string ValidationError = "VALIDATION_ERROR";
    public const string BusinessError = "BUSINESS_ERROR";
    public const string NotFound = "NOT_FOUND";
    public const string Unauthorized = "UNAUTHORIZED";
    public const string InternalError = "INTERNAL_ERROR";
}
