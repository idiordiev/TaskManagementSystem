using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;
using TaskManagementSystem.Application.Exceptions;

namespace TaskManagementSystem.Api.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="next"></param>
    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    /// <param name="logger"></param>
    /// <returns></returns>
    public async Task Invoke(HttpContext context, ILogger<ExceptionMiddleware> logger)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException ex)
        {
            logger.LogError(ex, "Bad Request Exception");
            await CreateErrorAsync(context, HttpStatusCode.BadRequest, new { error = ex.Message });
        }
        catch (NotFoundException ex)
        {
            logger.LogError(ex, "Not Found Exception");
            await CreateErrorAsync(context, HttpStatusCode.NotFound, new { error = ex.Message });
        }
        catch (ForbiddenException ex)
        {
            logger.LogError(ex, "Forbidden Exception");
            await CreateErrorAsync(context, HttpStatusCode.Forbidden, new { error = ex.Message });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Internal Server Error");
            await CreateErrorAsync(context, HttpStatusCode.InternalServerError, new { error = ex.Message });
        }
    }

    private async Task CreateErrorAsync(
        HttpContext context,
        HttpStatusCode statusCode = HttpStatusCode.InternalServerError,
        object? errorBody = null)
    {
        _ = errorBody ?? new { error = "Unknown error has occured" };
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;
        await context.Response.WriteAsync(JsonSerializer.Serialize(errorBody));
    }
}