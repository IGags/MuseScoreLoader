using Api.Constants;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Api.Filters
{
    /// <summary>
    /// Фильтр, чтобы не дублировать логику установки имени файла
    /// </summary>
    public class OutputFileNameFilterAttribute : Attribute, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            context.ActionArguments[ControllerConstants.NameQuery] ??=
                context.ActionArguments[ControllerConstants.RouteIdPostfix];
            await next();
        }
    }
}
