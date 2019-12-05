using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Web.Http;
using System.Net.Http;
using Consinco.WebApi.Models.Pessoas;
using Consinco.WebApi.Services;
using System;

namespace Consinco.WebApi.Controllers.v1
{
    //Extremamente importante quando criar um Controller Novo estender o ConsincoBaseController
    [RoutePrefix("api/pessoas/v1")]
    public class PessoasController : ApiController
    {
        // é boa prática criar uma classe de serviço para encapsular regras de consistência 
        // ou binds de dados para evitar que os controladores fiquem inchados
        private readonly PessoaService _pService;

        public PessoasController()
        {
            _pService = new PessoaService();
        }

        #region anotacoes Swagger para documentacao da api
        /// <summary>
        /// Retorna uma lista de pessoas.        
        /// </summary>
        /// <remarks>
        /// Para acionar esse método é necessário seguir os seguintes passos para montagem da requisição:
        /// 1. Cabeçalho do Http Request: 
        /// 
        /// 
        /// 2. Query string (url):
        ///    2.1. Informar um número de página (iniciar com 1) e definir o tamanho da página (quantidade de registros que você quer que retorne (máximo 100));
        ///    2.2. Opcionalmente, informar um ou mais dos atributos, caso desejar filtrar os dados;
        ///    2.3. Opcionalmente, informar ordenacao dos dados retornador, por um ou mais atributos;
        ///         Obs: Se precisar informar o tipo de ordenação (ascendente ou descendente), acrescente na frente do atributo desejado ':asc' ou ':desc'.
        /// 
        /// 3. Body de Exemplo
        ///    [não se aplica]
        ///    
        /// 4. Exemplos de Requisições: 
        /// protocolo://meuservidor/api/pessoas?pagina=1&tamanhoPagina=50&nomecompleto=MARC&tipo=J&ordenacao=nomecompleto:asc,cadastradoem:desc
        /// protocolo://meuservidor/api/pessoas?pagina=1&tamanhoPagina=50&ordenacao=nomecompleto
        /// protocolo://meuservidor/api/pessoas?pagina=1&tamanhoPagina=50
        /// </remarks>       
        [SwaggerResponse(System.Net.HttpStatusCode.OK, "Requisição processada com sucesso.", typeof(PessoaPaginado))]
        [SwaggerResponse(System.Net.HttpStatusCode.BadRequest, "Formato de requisição inválido ou parametrização errada.", typeof(PessoaPaginado))]
        [SwaggerResponse(System.Net.HttpStatusCode.NotFound, "Não foram encontrados registros para a página informada.")]
        [SwaggerResponse(System.Net.HttpStatusCode.Unauthorized, "Sem permissão para executar método.")]
        #endregion
        [Route("")]
        public IDisposable Get([FromUri] PessoaFiltro filtro)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.BadRequest, "Formato de requisição inválido.");
            }

            List<string> erros = _pService.ValidarRequisicao(filtro);
            if (erros.Count > 0)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.BadRequest, erros);
            }

            //ver como implementar o versionamento da API aqui no cabeçalho da Response

            var p = _pService.Obter(filtro);
            if (p.Pessoas.Count > 0) {                    
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, p);
            }
            else
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.NotFound);
            }                            
        }

        #region anotacoes Swagger para documentacao da api
        /// <summary>
        /// Retorna uma pessoa 
        /// </summary>
        /// <remarks>
        /// Retorna uma pessoa a partir do seu id.
        /// </remarks>               
        [SwaggerResponse(System.Net.HttpStatusCode.BadRequest, "Id inválido.")]
        [SwaggerResponse(System.Net.HttpStatusCode.NotFound, "Não encontrou registro de pessoa com o id informado.")]
        [SwaggerResponse(System.Net.HttpStatusCode.Unauthorized, "Sem permissão para executar método")]
        [SwaggerResponse(System.Net.HttpStatusCode.OK, "Requisição processada com sucesso", typeof(Pessoa))]
        #endregion        
        [HttpGet]
        [Route("{id}")]
        public IHttpActionResult ObterPessoa(long id)
        {
            if (id <= 0)
            {
                return BadRequest("Id inválido.");
            }

            var p = _pService.Obter(id);
            //ver como implementar o versionamento da API aqui no cabeçalho da Response
            if (p != null)
            {
                return Ok(p);
            }
            else
            {
                return NotFound();
            }
        }               
    }
}
