using System.Collections;
using System.Web.Http;
using System.Web.Http.Routing;
using Consinco.WebApi.Helpers;
using Microsoft.Web.Http;
using Microsoft.Web.Http.Routing;
using Microsoft.Web.Http.Versioning;
using WebApiThrottle;

namespace Consinco.WebApi
{
    public static class WebApiConfig
    {   public static void Register(HttpConfiguration config)
        {
            // Remove a opção da WebApi de receber e retornar dados em formato XML
            config.Formatters.Remove(config.Formatters.XmlFormatter);

            // Usado para caso de segurança em Web Api, para evitar que clients com erro
            // ou maliciosos façam muitas requisições simultaneamente, causando lentidão
            // ou falha no sistema ou na infra que o suporta.
            // Esse exemplo usamos a política em hardcode, mas há como usar um arquivo
            // de web.config para deixar os parâmetros fora do código.
            // A arquitetura está em um projeto de criar um catálogo de web apis para 
            // realizar vários controles, incluindo um deles, a política de uso do WebApi
            config.MessageHandlers.Add(new ThrottlingHandler()
            {
                Policy = new ThrottlePolicy(perSecond: 1, perMinute: 30)
                {
                    IpThrottling = false,
                    ClientThrottling = false,
                    EndpointThrottling = true,
                    StackBlockedRequests = true
                },
                Repository = new CacheRepository()
            });

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
