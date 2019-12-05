using System.Web.Http;
using WebActivatorEx;
using Consinco.WebApi;
using Swashbuckle.Application;

[assembly: PreApplicationStartMethod(typeof(SwaggerConfig), "Register")]

namespace Consinco.WebApi
{
    public class SwaggerConfig
    {
        public static void Register()
        {
            
        }
    }
}
