using AspNetCoreFeatureWithMonitor.Extension;
using AspNetCoreFeatureWithMonitor.Models.Enum;
using AspNetCoreFeatureWithMonitor.Models.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AspNetCoreFeatureWithMonitor.ActionFilter;

public class ValidationModelActionFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            var apiResponse = new ApiResponse<object>(ApiResponseStatus.ModelValidError)
            {
                Errors = context.ModelState.SelectMany(item => item.Value.Errors.Select(item2 => item2.ErrorMessage)).ToList(),
                Data = null
            };
            context.Result = new ObjectResult(apiResponse);
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        
    }
}