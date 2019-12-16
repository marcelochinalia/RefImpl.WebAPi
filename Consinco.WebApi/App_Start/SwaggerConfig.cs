using System.Web.Http;
using WebActivatorEx;
using WebApiReferencia;
using Swashbuckle.Application;
using System;

[assembly: PreApplicationStartMethod(typeof(SwaggerConfig), "Register")]

namespace WebApiReferencia
{
    public class SwaggerConfig
    {
        public static void Register()
        {
            var thisAssembly = typeof(SwaggerConfig).Assembly;

            GlobalConfiguration.Configuration
                .EnableSwagger(c =>
                {
                    c.SingleApiVersion("v1", "Consinco Web Api - Implementação de Referência");
                    c.IncludeXmlComments($@"{AppDomain.CurrentDomain.BaseDirectory}\bin\Consinco.WebApi.xml");                    
                    //c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
                })

                .EnableSwaggerUi(c =>
                {
                    c.DocumentTitle("Consinco.WebApi Reference");
                    c.DocExpansion(DocExpansion.List);
                });
        }
    }
}