using Consinco.WebApi.Helpers;
using Microsoft.Web.Http;
using Swashbuckle.Swagger.Annotations;
using System.Collections;
using System.Web.Http;

namespace Consinco.WebApi.Controllers
{
    // Sempre criar este Controlador para ajudar no HotDeploy da Web Api no servidor IIS do cliente
    //[ApiVersion("1", Deprecated = true)]
    [AdvertiseApiVersions("1", Deprecated = true)]
    [Route("api/home")]
    public class HomeController : ConsincoBaseController
    {
        #region anotacoes Swagger para documentacao da api
        /// <summary>
        /// Avisa se Web Api está rodando.        
        /// </summary>
        [SwaggerResponse(System.Net.HttpStatusCode.OK, "Requisição processada com sucesso.", typeof(string))]
        #endregion
        public IHttpActionResult Get()
        {
            
            Hashtable dadosVersao = VersionamentoHelper.ObterVersaoAtualApi();
            string apiNome = dadosVersao[VersionamentoHelper.ApiNome].ToString();
            string apiVersao = dadosVersao[VersionamentoHelper.ApiVersao].ToString();
            string apiBuild = dadosVersao[VersionamentoHelper.ApiBuild].ToString();

            string texto = "[Get v1 - deprecated]: " + apiNome + " versão [" + apiVersao + "." + apiBuild + "] está rodando... ";

            return Ok(texto);
        }
    }

    [ApiVersion("2")]
    [Route("api/home")]
    public class HomeV2Controller : ConsincoBaseController
    {
        #region anotacoes Swagger para documentacao da api
        /// <summary>
        /// Avisa se Web Api está rodando.        
        /// </summary>
        [SwaggerResponse(System.Net.HttpStatusCode.OK, "Requisição processada com sucesso.", typeof(string))]
        #endregion
        public IHttpActionResult Get()
        {
            Hashtable dadosVersao = VersionamentoHelper.ObterVersaoAtualApi();
            string apiNome = dadosVersao[VersionamentoHelper.ApiNome].ToString();
            string apiVersao = dadosVersao[VersionamentoHelper.ApiVersao].ToString();
            string apiBuild = dadosVersao[VersionamentoHelper.ApiBuild].ToString();

            string texto = "[Get v2]: " + apiNome + " versão [" + apiVersao + "." + apiBuild + "] está rodando... ";

            return Ok(texto);
        }
    }

    [ApiVersion("3")]
    [Route("api/home")]
    public class HomeV3Controller : ConsincoBaseController
    {
        #region anotacoes Swagger para documentacao da api
        /// <summary>
        /// Avisa se Web Api está rodando.        
        /// </summary>
        [SwaggerResponse(System.Net.HttpStatusCode.OK, "Requisição processada com sucesso.", typeof(string))]
        #endregion
        public IHttpActionResult Get()
        {
            Hashtable dadosVersao = VersionamentoHelper.ObterVersaoAtualApi();
            string apiNome = dadosVersao[VersionamentoHelper.ApiNome].ToString();
            string apiVersao = dadosVersao[VersionamentoHelper.ApiVersao].ToString();
            string apiBuild = dadosVersao[VersionamentoHelper.ApiBuild].ToString();

            string texto = "[Get v3]: " + apiNome + " versão [" + apiVersao + "." + apiBuild + "] está rodando... ";

            return Ok(texto);
        }
    }
}
