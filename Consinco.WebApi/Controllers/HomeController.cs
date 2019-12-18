using Consinco.WebApi.Helpers;
using Microsoft.Web.Http;
using Swashbuckle.Swagger.Annotations;
using System.Collections;
using System.Web.Http;

namespace Consinco.WebApi.Controllers
{
    // AdvertiseApiVersions se declarado com Deprecated = true, faz com que o cliente que consome o endpoint receba um HttpStatusCode = BadRequest
    // o que significa que ele não receberá mais o retorno esperado.
    [AdvertiseApiVersions("1", Deprecated = true)]
    [RoutePrefix("api/home")]
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
            
            Hashtable dadosVersao = VersionamentoHelper.ObterVersaoAtualApi();
            string apiNome = dadosVersao[VersionamentoHelper.ApiNome].ToString();
            string apiVersao = dadosVersao[VersionamentoHelper.ApiVersao].ToString();
            string apiBuild = dadosVersao[VersionamentoHelper.ApiBuild].ToString();

            string texto = "[Get v1 - deprecated]: " + apiNome + " versão [" + apiVersao + "." + apiBuild + "] está rodando... ";

            return Ok(texto);
        }
    }

    // Se ApiVersion declarado com Deprecated = true, faz com que o cliente que consome o endpoint receba um HttpStatusCode = 200
    // porém, ele receberá o atributo api-deprecated-versions, indicando que esse método já está obsoleto.
    [ApiVersion("2",Deprecated = true)]
    [Route("api/home")]
    public class HomeV2Controller : ApiController
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

    // sempre que alterar a versão de um controlador, todos os demais devem ser versionado
    [ApiVersion("3")]
    [Route("api/home")]
    public class HomeV3Controller : ApiController
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
