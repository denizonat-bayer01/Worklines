using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using wixi.Core.Exceptions;

namespace wixi.WebAPI.Filters
{
    public sealed class ModelValidationFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var errors = context.ModelState
                    .Where(kv => kv.Value?.Errors.Count > 0)
                    .ToDictionary(
                        kv => kv.Key,
                        kv => kv.Value!.Errors.Select(e => string.IsNullOrWhiteSpace(e.ErrorMessage) ? e.Exception?.Message : e.ErrorMessage).Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s!).ToArray()
                    );

                throw new ValidationException("Doğrulama hatası", errors);
            }
        }

        public void OnActionExecuted(ActionExecutedContext context) { }
    }
}


