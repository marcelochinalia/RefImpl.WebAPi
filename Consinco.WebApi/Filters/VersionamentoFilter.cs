using System.Web.Http.ExceptionHandling;
using System.Web.Http.Filters;

namespace Consinco.WebApi.Filters
{
    public class VersionamentoFilter : ExceptionFilterAttribute
    {
        //  UnsupportedApiVersionException

        public override void OnException(HttpActionExecutedContext context)
        {
         
            var teste = context.Exception.ToString();

            //context.Result = new JsonResult(apiError);

            base.OnException(context);
        }
    }
}