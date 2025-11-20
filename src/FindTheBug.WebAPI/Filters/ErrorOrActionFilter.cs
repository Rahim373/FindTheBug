using ErrorOr;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FindTheBug.WebAPI.Filters;

public class ErrorOrActionFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        // Nothing to do before action executes
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Result is ObjectResult objectResult && objectResult.Value is not null)
        {
            var resultType = objectResult.Value.GetType();
            
            // Check if the result implements IErrorOr
            if (resultType.IsGenericType)
            {
                var genericTypeDef = resultType.GetGenericTypeDefinition();
                
                // Check if it's ErrorOr<T>
                if (genericTypeDef.FullName?.StartsWith("ErrorOr.ErrorOr") == true)
                {
                    // Get the IsError property using reflection
                    var isErrorProperty = resultType.GetProperty("IsError");
                    if (isErrorProperty is not null)
                    {
                        var isError = (bool)isErrorProperty.GetValue(objectResult.Value)!;
                        
                        if (isError)
                        {
                            // Handle error case
                            var errorsProperty = resultType.GetProperty("Errors");
                            if (errorsProperty is not null)
                            {
                                var errorsValue = errorsProperty.GetValue(objectResult.Value);
                                if (errorsValue is System.Collections.IEnumerable enumerable)
                                {
                                    var errorsList = new List<Error>();
                                    foreach (var item in enumerable)
                                    {
                                        if (item is Error error)
                                        {
                                            errorsList.Add(error);
                                        }
                                    }

                                    if (errorsList.Count > 0)
                                    {
                                        var firstError = errorsList[0];
                                        var statusCode = firstError.Type switch
                                        {
                                            ErrorType.NotFound => StatusCodes.Status404NotFound,
                                            ErrorType.Validation => StatusCodes.Status400BadRequest,
                                            ErrorType.Conflict => StatusCodes.Status409Conflict,
                                            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
                                            ErrorType.Forbidden => StatusCodes.Status403Forbidden,
                                            _ => StatusCodes.Status500InternalServerError
                                        };

                                        var problemDetails = new ProblemDetails
                                        {
                                            Type = firstError.Type.ToString(),
                                            Title = firstError.Description,
                                            Status = statusCode
                                        };
                                        
                                        problemDetails.Extensions["errors"] = errorsList.Select(e => new { e.Code, e.Description }).ToList();
                                        problemDetails.Extensions["traceId"] = context.HttpContext.TraceIdentifier;

                                        context.Result = new ObjectResult(problemDetails)
                                        {
                                            StatusCode = statusCode
                                        };
                                    }
                                }
                            }
                        }
                        else
                        {
                            // Handle success case - extract the value
                            var valueProperty = resultType.GetProperty("Value");
                            if (valueProperty is not null)
                            {
                                var value = valueProperty.GetValue(objectResult.Value);
                                context.Result = new ObjectResult(value)
                                {
                                    StatusCode = objectResult.StatusCode ?? StatusCodes.Status200OK
                                };
                            }
                        }
                    }
                }
            }
        }
    }
}
