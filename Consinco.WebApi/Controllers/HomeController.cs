using Swashbuckle.Swagger.Annotations;
using System.Reflection;
using System.Web.Http;

namespace Consinco.WebApi.Controllers
{
    // Sempre criar este Controlador para ajudar no HotDeploy da Web Api no servidor IIS do cliente
    public class HomeController : ApiController
    {
        #region anotacoes Swagger para documentacao da api
        /// <summary>
        /// Avisa se Web Api está rodando.        
        /// </summary>
        [SwaggerResponse(System.Net.HttpStatusCode.OK, "Requisição processada com sucesso.", typeof(string))]
        #endregion
        public IHttpActionResult Get()
        {
            string apinName = Assembly.GetExecutingAssembly().GetName().Name.ToString();
            string apiVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();

            string texto = apinName + " [" + apiVersion + "] está rodando... ";

            return Ok(texto);
        }
    }
}
