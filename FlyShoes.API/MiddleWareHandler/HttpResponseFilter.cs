using Microsoft.AspNetCore.Mvc.Filters;

namespace FlyShoes.API.MiddleWareHandler
{
    public class HttpResponseFilter : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
            if(context.Exception != null)
            {
                
            }
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {

        }
    }
}
