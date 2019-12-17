using Consinco.WebApi.Models.Errors;
using Consinco.WebApi.Models.Pessoas;
using Consinco.WebApi.Services;
using Swashbuckle.Swagger.Annotations;
using Microsoft.Web.Http;
using System.Web.Http;
using System.Net.Http;
using System;
using Consinco.WebApi.Filters;

namespace Consinco.WebApi.Controllers.v1
{
    [AdvertiseApiVersions("1", Deprecated = true)]
    [ApiVersion("2", Deprecated = true)]
    [ApiVersion("3")]
    [VersionamentoFilter]
    [RoutePrefix("api/pessoas")]
    public class PessoasController : ApiController
    {
        // é boa prática criar uma classe de serviço para encapsular regras de consistência 
        // ou binds de dados para evitar que os controladores fiquem inchados
        private const int _TOO_MANY_REQUEST = 429;
        private readonly PessoaService _pService = new PessoaService();
        private readonly NLog.Logger _log = Logs.PessoasLog.Instace.ObterLogger();
        
        #region anotacoes Swagger para documentacao da api
        ///<summary>
        /// Retorna uma lista de pessoas.        
        ///</summary>
        ///<remarks>
        ///Para acionar esse método é necessário seguir os seguintes passos para montagem da requisição:
        ///###1. Cabeçalho (Header) do Http Request###
        ///Informe através da variável `api-version` a versão do endpoint que você está usando.
        ///
        ///**Atenção:** Se `api-version` não for informada, a Web Api executará sua versão mais nova, podendo causar uma problema em sua aplicação de consumo.
        ///Verifique sempre em sua `Http Response` se a Web Api Consinco retorna uma varíavel `api-deprecated-versions`.
        ///Isso significa que a versão do endpoint está obselta e a mesma poderá deixar de funcionar em versões futuras do ERP (você receberá response com `HttpStatusCode = 400 (Bad Request)`.
        /// 
        ///###2. Query string (URL)###
        ///     2.1. Informar um número da página (iniciar com 1) a ser consultada e definir o tamanho da página, 
        ///          que é quantidade de registros que você quer que retorne na página (máximo 100).
        ///     2.2. Opcionalmente, informar um ou mais dos atributos, caso desejar filtrar os dados.
        ///     2.3. Opcionalmente, informar ordenacao dos dados retornador, por um ou mais atributos.
        ///     
        ///**Obs:** Se precisar informar o tipo de ordenação (ascendente ou descendente), acrescente na frente do atributo desejado `:asc` ou `:desc`.
        /// 
        ///###3. Corpo (Body) do Http Request###
        ///[não se aplica]
        ///    
        ///###4. Detalhes da Resposta (Http Response)###
        ///Com o objetivo de evitar que clients com erro ou maliciosos façam muitas requisições simultaneamente, causando lentidão, falha no sistema ou na infra que o suporta, está Web Api possui uma política de acesso simultâneo totalmente configurável para sua necessidade. Uma vez definida a política, o Web Api sempre retornará ao client que estiver consumindo o endpoint as varáveis abaixo em seu cabeçalho:
        /// 
        ///     X-Rate-Limit-Limit: exibe a quantidade de tempo que resta para aceitar requisições simultâneas no WebApi.
        ///     X-Rate-Limit-Remaining: exibe a quantidade de requisições que ainda falta para exceder a quantidade de acessos simultâneos.
        ///     X-Rate-Limit-Reset: indica em que data/hora o Web Api reiniciará o número total de requisições simultâneas.
        ///    
        ///**Detalhe importante sobre a política de acesso simultâneo:**
        ///
        ///Caso o limite de acessos da política tenha sido atingido, o Web Api retornará client o `HttpStatusCode 429 (Too Many Requests)` e no cabeçalho da response virá a variável abaixo:
        ///
        ///     After-Retry: possui o tempo de espera que o cliente precisa aguardar para enviar uma nova requisição.
        ///    
        /// ###5. Exemplos de Requisições###
        ///     a. <![CDATA[http://meuservidor/api/pessoas?pagina=1&tamanhoPagina=50&nomecompleto=marc&tipo=J&ordenacao=nomecompleto:asc,cadastradoem:desc]]>
        ///     b. <![CDATA[http://meuservidor/api/pessoas?pagina=1&tamanhoPagina=50&ordenacao=nomecompleto]]>
        ///     c. <![CDATA[http://meuservidor/api/pessoas?pagina=1&tamanhoPagina=50]]>       
        ///</remarks>
        [SwaggerResponse(_TOO_MANY_REQUEST, "Número de requisições simultâneas suportadas esgotado.)")]
        [SwaggerResponse(System.Net.HttpStatusCode.BadRequest, "Parâmetros enviados incorretamente.", typeof(Erro))]
        [SwaggerResponse(System.Net.HttpStatusCode.NotFound, "Não foram encontrados registros para a página informada.")]
        [SwaggerResponse(System.Net.HttpStatusCode.Unauthorized, "Sem permissão para executar método.")]
        [SwaggerResponse(System.Net.HttpStatusCode.OK, "Requisição processada com sucesso.", typeof(PessoaPaginado))]
        #endregion
        [HttpGet]
        [Route("")]
        public HttpResponseMessage ObterPessoa([FromUri] PessoaFiltro filtro)
        {
            _log.Info("[ObterPessoa] Iniciando...");
            _log.Debug("[ObterPessoa] Filtro: " + filtro.toJson());
            try {
                _log.Info("[ObterPessoa] Validando ModelState...");
                if (!ModelState.IsValid)
                {
                    return Request.CreateResponse(System.Net.HttpStatusCode.BadRequest, _pService.GerarErro());
                }

                _log.Info("[ObterPessoa] Validando Requisicao...");
                var erros = _pService.ValidarRequisicao(filtro);
                if (erros.Count > 0)
                {
                    return Request.CreateResponse(System.Net.HttpStatusCode.BadRequest, erros);
                }

                _log.Info("[ObterPessoa] Consultando dados...");
                var p = _pService.Obter(filtro);
                if (p.Pessoas.Count > 0) {
                    _log.Info("[ObterPessoa] Retornando dados...");
                    return Request.CreateResponse(System.Net.HttpStatusCode.OK, p);
                }
                else
                {
                    _log.Info("[ObterPessoa] Dados não encontrados...");
                    return Request.CreateResponse(System.Net.HttpStatusCode.NotFound);
                }
            }
            catch (Exception e) {
                _log.Error(e);

                return Request.CreateResponse(System.Net.HttpStatusCode.InternalServerError);
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
        public HttpResponseMessage ObterPessoa(long id)
        {            
            try
            {
                if (id <= 0)
                {
                    return Request.CreateResponse(System.Net.HttpStatusCode.BadRequest, _pService.GerarErro());
                }

                var p = _pService.Obter(id);
                if (p != null)
                {
                    return Request.CreateResponse(System.Net.HttpStatusCode.OK, p);
                }
                else
                {
                    return Request.CreateResponse(System.Net.HttpStatusCode.NotFound);
                }
            }
            catch (Exception)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.InternalServerError);
            }
        }

        #region anotacoes Swagger para documentacao da api
        /// <summary>
        /// Exclui uma pessoa 
        /// </summary>
        /// <remarks>
        /// Exclui uma pessoa da base de dados a partir do seu id.
        /// </remarks>               
        [SwaggerResponse(System.Net.HttpStatusCode.BadRequest, "Parâmetro inválido.", typeof(Erro))]
        [SwaggerResponse(System.Net.HttpStatusCode.InternalServerError, "Erro ao tentar excluir pessoa.")]
        [SwaggerResponse(System.Net.HttpStatusCode.OK, "Requisição processada com sucesso.")]
        [SwaggerResponse(System.Net.HttpStatusCode.Unauthorized, "Sem permissão para executar método.")]        
        #endregion
        [HttpDelete]
        [Route("{id}")]
        public HttpResponseMessage Excluir(long id)
        {
            try
            {
                if (id <= 0)
                {
                    return Request.CreateResponse(System.Net.HttpStatusCode.BadRequest, _pService.GerarErro());
                }

                _pService.Excluir(id);
                return Request.CreateResponse(System.Net.HttpStatusCode.OK);
            }
            catch(Exception)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.InternalServerError);
            }
        }

        #region anotacoes Swagger para documentacao da api
        /// <summary>
        /// Incluir uma pessoa nova
        /// </summary>
        /// <remarks>
        /// Inclui uma pessoa da base de dados.
        /// </remarks>               
        [SwaggerResponse(System.Net.HttpStatusCode.BadRequest, "Parâmetros inválidos.", typeof(Erro))]
        [SwaggerResponse(System.Net.HttpStatusCode.InternalServerError, "Erro ao incluir uma nova pessoa.")]
        [SwaggerResponse(System.Net.HttpStatusCode.Created, "Requisição processada com sucesso.", typeof(Pessoa))]
        [SwaggerResponse(System.Net.HttpStatusCode.Unauthorized, "Sem permissão para executar método.")]
        #endregion
        [HttpPost]
        [Route("")]
        public HttpResponseMessage Incluir([FromBody] Pessoa pessoa)
        {
            try
            {
                if (!ModelState.IsValid || pessoa == null)
                {
                    return Request.CreateResponse(System.Net.HttpStatusCode.BadRequest, _pService.GerarErro());
                }

                var erros = _pService.ValidarRequisicao(pessoa, true);
                if (erros.Count > 0)
                {
                    return Request.CreateResponse(System.Net.HttpStatusCode.BadRequest, erros);
                }

                var p = _pService.Novo(pessoa);

                return Request.CreateResponse(System.Net.HttpStatusCode.OK, p);                
            }
            catch (Exception)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.InternalServerError);
            }
        }

        #region anotacoes Swagger para documentacao da api
        /// <summary>
        /// Atualizar todos os dados de uma pessoa.
        /// </summary>
        /// <remarks>
        /// Atualizar todos os dados de uma pessoa já existente na base de dados.
        /// </remarks>               
        [SwaggerResponse(System.Net.HttpStatusCode.BadRequest, "Parâmetros inválidos.", typeof(Erro))]
        [SwaggerResponse(System.Net.HttpStatusCode.InternalServerError, "Erro ao incluir uma nova pessoa.")]
        [SwaggerResponse(System.Net.HttpStatusCode.OK, "Requisição processada com sucesso.", typeof(Pessoa))]
        [SwaggerResponse(System.Net.HttpStatusCode.Unauthorized, "Sem permissão para executar método.")]
        #endregion
        [HttpPut]
        [Route("")]
        public HttpResponseMessage Atualizar([FromBody] Pessoa pessoa)
        {
            try
            {
                if (!ModelState.IsValid ||  pessoa == null)
                {
                    return Request.CreateResponse(System.Net.HttpStatusCode.BadRequest, _pService.GerarErro());
                }

                if (_pService.PessoaExiste(pessoa.Id))
                {
                    var erros = _pService.ValidarRequisicao(pessoa);
                    if (erros.Count > 0)
                    {
                        return Request.CreateResponse(System.Net.HttpStatusCode.BadRequest, erros);
                    }
                    _pService.Atualizar(pessoa);

                    return Request.CreateResponse(System.Net.HttpStatusCode.OK);
                }
                else
                {
                    return Request.CreateResponse(System.Net.HttpStatusCode.NotFound);
                }
            }
            catch (Exception)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.InternalServerError);
            }
        }

        #region anotacoes Swagger para documentacao da api
        /// <summary>
        /// Atualizar 1 ou mais dados de uma pessoa.
        /// </summary>
        /// <remarks>
        /// Atualizar 1 ouo mais os dados de uma pessoa já existente na base de dados.
        /// </remarks>               
        [SwaggerResponse(System.Net.HttpStatusCode.BadRequest, "Parâmetros inválidos.", typeof(Erro))]
        [SwaggerResponse(System.Net.HttpStatusCode.InternalServerError, "Erro ao incluir uma nova pessoa.")]
        [SwaggerResponse(System.Net.HttpStatusCode.OK, "Requisição processada com sucesso.")]
        [SwaggerResponse(System.Net.HttpStatusCode.Unauthorized, "Sem permissão para executar método.")]
        #endregion
        [HttpPatch]
        [Route("{id}")]
        public HttpResponseMessage Atualizar(long id, [FromBody] PessoaPatch pessoa)
        {
            try
            {
                if (!ModelState.IsValid || pessoa == null)
                {
                    return Request.CreateResponse(System.Net.HttpStatusCode.BadRequest, _pService.GerarErro());
                }

                if (!_pService.PessoaExiste(id))
                {
                    return Request.CreateResponse(System.Net.HttpStatusCode.NotFound);
                }

                var erros = _pService.ValidarRequisicao(id, pessoa);
                if (erros.Count > 0)
                {
                    return Request.CreateResponse(System.Net.HttpStatusCode.BadRequest, erros);
                }

                _pService.Atualizar(id, pessoa);

                return Request.CreateResponse(System.Net.HttpStatusCode.OK);                
            }
            catch (Exception)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.InternalServerError);
            }
        }
    }
}
