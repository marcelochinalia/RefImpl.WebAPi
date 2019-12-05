using System.Web.Http;

namespace Consinco.WebApi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Remove a opção da WebApi de receber e retornar dados em formato XML
            config.Formatters.Remove(config.Formatters.XmlFormatter);

            // Usado para caso de chamada de protocolo http 1.0 para verbos do http 1.1
            config.MessageHandlers.Add(new MethodOverrideHandler());

            // Configuração de Rotas da Web Api
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
