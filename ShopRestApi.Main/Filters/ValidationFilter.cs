using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ShopRestApi.Application.Common.Models;

namespace ShopRestApi.Api.Filters;

public class ValidationFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            var errors = context.ModelState
                .Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            var response = new ApiResponse<object>
            {
                Success = false,
                Message = "Validation failed",
                Errors = errors
            };

            context.Result = new BadRequestObjectResult(response);
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
    }
}