using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EventReceiverApi.Web.Filters
{
    public class ValidateEventParametersAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.ActionArguments.TryGetValue("offset", out object? offsetValue) && offsetValue is int offset)
            {
                if (offset < 0 || offset > 23)
                {
                    context.ModelState.AddModelError("offset", "The field offset must be between 0 and 23.");
                }
            }

            if (context.ActionArguments.TryGetValue("startTime", out object? startTimeValue) && startTimeValue is DateTime startTime &&
                context.ActionArguments.TryGetValue("endTime", out object? endTimeValue) && endTimeValue is DateTime endTime)
            {
                if (startTime >= endTime)
                {
                    context.ModelState.AddModelError("startTime", "The startTime must be less than endTime.");
                }
            }

            if (!context.ModelState.IsValid)
            {
                context.Result = new BadRequestObjectResult(context.ModelState);
            }

            base.OnActionExecuting(context);
        }
    }
}
