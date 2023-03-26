using FlyShoes.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FlyShoes.API.MiddleWareHandler
{
    public class HttpResponseFilter : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
            if(context.Exception != null)
            {
                if(context.Exception is FSException fSException)
                {
                    object result = fSException.Data.Count == 0 ? fSException.Message : fSException.Data;
                    context.Result = new ObjectResult(result)
                    {
                        StatusCode = 400,
                    };

                    context.ExceptionHandled = true;
                }
                else
                {
                    var result = new
                    {
                        userMsg = "Có lỗi xảy ra vui lòng liên hệ kỹ thuật viên để được giúp đỡ",
                        devMsg = context.Exception.Message,
                        errorMsg = "",
                    };

                    context.Result = new ObjectResult(result)
                    {
                        StatusCode = 500
                    };

                    context.ExceptionHandled = true;
                }
            }
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {

        }
    }
}
