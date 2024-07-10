using Microsoft.AspNetCore.Mvc;

namespace eRM_VersionHub.Result;

public class Result<T>
{
    public ProblemDetails? ProblemDetails { get; set; }
    public bool IsSuccess { get; set; }
    public List<string> ErrorMessages { get; set; } = [];
    public T? Data { get; set; }

    public static Result<T> Success(T data)
    {
        return new Result<T> { Data = data, IsSuccess = true };
    }

    public static Result<T> Failure(
        List<string> errorMessages,
        string? detail = null,
        int statusCode = 400
    )
    {
        return new Result<T>
        {
            IsSuccess = false,
            ErrorMessages = errorMessages,
            ProblemDetails = new ProblemDetails
            {
                Detail = detail ?? string.Join(", ", errorMessages),
                Status = statusCode
            }
        };
    }
}