using System.Collections;
using System.Web.Http;
using System.Web.Http.Routing;
using Consinco.WebApi.Helpers;
using Microsoft.Web.Http;
using Microsoft.Web.Http.Routing;
using Microsoft.Web.Http.Versioning;

namespace Consinco.WebApi
{
    public static class WebApiConfig
    {   public static void Register(HttpConfiguration config)
        {
            // Remove a opção da WebApi de receber e retornar dados em formato XML
            config.Formatters.Remove(config.Formatters.XmlFormatter);

            // Usado para caso de chamada de protocolo http 1.0 para verbos do http 1.1
            config.MessageHandlers.Add(new MethodOverrideHandler());

            config.AddApiVersioning(o =>
            {
                o.ReportApiVersions = true;                
                o.DefaultApiVersion = new ApiVersion(1,0);
                o.AssumeDefaultVersionWhenUnspecified = true;
                o.ApiVersionReader = new HeaderApiVersionReader("api-version");                
                o.ApiVersionSelector = new CurrentImplementationApiVersionSelector(o);
            });

            // Configuração de Rotas da Web Api
            config.MapHttpAttributeRoutes();
            
            // Ativando mecanismo de versionamento dos Endpoints
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
