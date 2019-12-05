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
        /// Retornar uma lista de todas as pessoas por paginação. 
        /// Informar um número de página (iniciar com 1), definir a quantidade de registros que você quer que retorne (máximo 100). 
        /// Além disso, você opcionalmente pode solicitar a ordenacao dos dados por um ou mais atributos do JSON (se não informado será ordenado por Id).
        /// Caso precise na descriminar o tipo de ordenação (ascendente ou descendente), acrescente na frente do atributo ':asc' ou ':desc'.
        /// 
        /// URL de Exemplo: 
        /// http://meuservidor/api/pessoas?pagina=1&tamanhoPagina=50&nomecompleto=MARC&tipo=J&ordenacao=nomecompleto:asc,cadastradoem:desc
        /// 
        /// Cabeçalho da Request de Exemplo: 
        /// 
        /// Body de Exemplo: 
        /// [não se aplica]
        /// </summary>
        /// <remarks>
        /// Retorna uma lista de todas as pessoas do ERP Acrux Web.
        /// </remarks>       
        [SwaggerResponse(System.Net.HttpStatusCode.OK, "Requisição processada com sucesso", typeof(PessoaPaginado))]
        [SwaggerResponse(System.Net.HttpStatusCode.NotFound, "Não foram encontrados registros para a página informada.")]
        [SwaggerResponse(System.Net.HttpStatusCode.Unauthorized, "Sem permissão para executar método")]
        #endregion
        [Route("")]
        public IDisposable Get([FromUri] Pessoa filtro, int pagina, int tamanhoPagina, string ordenacao)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.BadRequest, "Formato de requisição inválido.");
            }

            List<string> erros = _pService.ValidarRequisicao(ordenacao);
            if (erros.Count > 0)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.BadRequest, erros);
            }

            var p = _pService.Obter(pagina, tamanhoPagina, filtro, ordenacao);

            if (p != null)
            {
                //ver como implementar o versionamento da API aqui no cabeçalho da Response
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, p);
            }
            else
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.NotFound);
            }
        }

        #region anotacoes Swagger para documentacao da api
        /// <summary>
        /// Retornar uma pessoa 
        /// </summary>
        /// <remarks>
        /// Retorna uma pessoa a partir do seu id.
        /// </remarks>       
        [SwaggerResponse(System.Net.HttpStatusCode.OK, "Requisição processada com sucesso", typeof(Pessoa))]
        [SwaggerResponse(System.Net.HttpStatusCode.NotFound, "Não encontrou registro de pessoa com o id informado.")]
        [SwaggerResponse(System.Net.HttpStatusCode.Unauthorized, "Sem permissão para executar método")]
        #endregion        
        [HttpGet]
        [Route("{id}")]
        public IHttpActionResult ObterPessoa(long id)
        {
            if (id == 0)
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

        /*
        #region anotacoes Swagger para documentacao da api        
        /// <summary>
        /// Retornar uma lista de todas as pessoas de um tipo específico (física ou jurídica) por paginação. 
        /// Informar um número de página (iniciar com 1), definir a quantidade de registros que você quer que retorne (máximo 100). 
        /// Além disso, você opcionalmente pode solicitar a ordenacao dos dados por um ou mais atributos do JSON (se não informado será ordenado por Id).
        /// Caso precise na descriminar o tipo de ordenação (ascendente ou descendente), acrescente na frente do atributo ':asc' ou ':desc'.
        /// Por fim, na URI do endpoint você deve informar todas/tipo/fisica ou todas/tipo/juridica
        /// 
        /// URL de Exemplos: 
        /// http://meuservidor/api/v1?todas/tipo/fisica?pagina=1&tamanhoPagina=50&ordenacao=nomecompleto:asc,cadastradoem:desc
        /// http://meuservidor/api/v1?todas/tipo/juridica?pagina=1&tamanhoPagina=50&ordenacao=nomereduzido
        /// 
        /// Cabeçalho da Request de Exemplo: 
        /// 
        /// Body de Exemplo: 
        /// [não se aplica]
        /// </summary>
        /// <remarks>
        /// Retorna uma lista de todas as pessoas do ERP Acrux Web.
        /// </remarks>      
        [SwaggerResponse(System.Net.HttpStatusCode.OK, "Requisição processada com sucesso", typeof(PessoaPaginado))]
        [SwaggerResponse(System.Net.HttpStatusCode.NotFound, "Não foram encontrados registros para a página informada.")]
        [SwaggerResponse(System.Net.HttpStatusCode.Unauthorized, "Sem permissão para executar método")]
        #endregion
        [HttpGet]
        [Route("api/v1/todas/tipo/{tipo}")]
        public IHttpActionResult ObterPessoasPorTipo(int pagina, int tamanhoPagina, string PessoaFiltro, string ordenacao, [FromUri] string tipo)
        {
            //Criar aqui o Cancellation Token

            bool reqOk = true;
            if (string.IsNullOrEmpty(tipo) || string.IsNullOrWhiteSpace(tipo))
            {
                reqOk = false;
            }
            else
            {
                if (!tipo.ToLower().Equals("fisica") && !tipo.ToLower().Equals("juridica")) {
                    reqOk = false;
                }
            }                    
                
            if (!reqOk)
            {
                return BadRequest("Formato de requisição inválido. Tipo requerido: 'fisica' ou 'juridica'");
            }

            var p = _pService.Obter(pagina, tamanhoPagina, filtro, ordenacao, tipo);
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
        */
    }
}
