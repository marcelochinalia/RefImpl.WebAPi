using Consinco.WebApi.Models.Errors;
using Consinco.WebApi.Models.Pessoas;
using Consinco.WebApi.Services;
using Swashbuckle.Swagger.Annotations;
using Microsoft.Web.Http;
using System.Web.Http;
using System.Net.Http;
using System;
using Serilog.Core;
using Consinco.WebApi.Logs;

namespace Consinco.WebApi.Controllers.v1
{
    [AdvertiseApiVersions("1", Deprecated = true)]
    [ApiVersion("2", Deprecated = true)]
    [ApiVersion("3")]
    [RoutePrefix("api/pessoas")]
    public class PessoasController : ApiController
    {
        // é boa prática criar uma classe de serviço para encapsular regras de consistência 
        // ou binds de dados para evitar que os controladores fiquem inchados
        private const int _TOO_MANY_REQUEST = 429;
        private readonly PessoaService _pService = new PessoaService();
        private readonly Logger _log = Logs.PessoasLog.Instace.ObterLogger();
        
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
        ///###2. Query string (URI)###
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
        /// ###5. Exemplo de Requisição###
        ///     a. <![CDATA[http://meuservidor/api/pessoas?pagina=1&tamanhoPagina=50&nomecompleto=marc&tipo=J&ordenacao=nomecompleto:asc,cadastradoem:desc]]>
        ///     b. <![CDATA[http://meuservidor/api/pessoas?pagina=1&tamanhoPagina=50&ordenacao=nomecompleto]]>
        ///     c. <![CDATA[http://meuservidor/api/pessoas?pagina=1&tamanhoPagina=50]]>       
        ///</remarks>
        [SwaggerResponse(_TOO_MANY_REQUEST, "Número de requisições simultâneas suportadas esgotado.")]
        [SwaggerResponse(System.Net.HttpStatusCode.BadRequest, "Parâmetros enviados incorretamente.", typeof(Erro))]
        [SwaggerResponse(System.Net.HttpStatusCode.NotFound, "Não foram encontrados registros para a página informada.")]
        [SwaggerResponse(System.Net.HttpStatusCode.Unauthorized, "Sem permissão para executar método.")]
        [SwaggerResponse(System.Net.HttpStatusCode.OK, "Requisição processada com sucesso.", typeof(PessoaPaginado))]
        #endregion
        [HttpGet]
        [Route("")]
        public HttpResponseMessage ObterPessoas([FromUri] PessoaFiltro filtro)
        {            
            try {
                _log.Information("[ObterPessoas] Iniciando...");
                _log.Debug("[ObterPessoas] Filtro: \n" + filtro.ToJson());

                _log.Information("[ObterPessoas] Validando ModelState...");
                if (!ModelState.IsValid)
                {
                    return Request.CreateResponse(System.Net.HttpStatusCode.BadRequest, _pService.GerarErro());
                }

                _log.Information("[ObterPessoas] Validando requisicao...");
                var erros = _pService.ValidarRequisicao(filtro);
                if (erros.Count > 0)
                {
                    return Request.CreateResponse(System.Net.HttpStatusCode.BadRequest, erros);
                }

                _log.Information("[ObterPessoas] Consultando dados...");
                var p = _pService.Obter(filtro);
                if (p.Resultados.Count > 0) {
                    _log.Information("[ObterPessoas] Finalizado.");
                    return Request.CreateResponse(System.Net.HttpStatusCode.OK, p);
                }
                else
                {
                    _log.Warning("[ObterPessoas] Dados não encontrados...");
                    return Request.CreateResponse(System.Net.HttpStatusCode.NotFound);
                }
            }
            catch (Exception e) {
                _log.Error(e, "[ObterPessoas] Erro Interno");
                
                return Request.CreateResponse(System.Net.HttpStatusCode.InternalServerError);
            }
        }

        #region anotacoes Swagger para documentacao da api
        /// <summary>
        /// Retorna uma pessoa pelo Id
        /// </summary>
        /// <remarks>
        ///Para acionar esse método é necessário seguir os seguintes passos para montagem da requisição:
        ///###1. Cabeçalho (Header) do Http Request###
        ///Informe através da variável `api-version` a versão do endpoint que você está usando.
        ///
        ///**Atenção:** Se `api-version` não for informada, a Web Api executará sua versão mais nova, podendo causar uma problema em sua aplicação de consumo.
        ///Verifique sempre em sua `Http Response` se a Web Api Consinco retorna uma varíavel `api-deprecated-versions`.
        ///Isso significa que a versão do endpoint está obselta e a mesma poderá deixar de funcionar em versões futuras do ERP (você receberá response com `HttpStatusCode = 400 (Bad Request)`.
        /// 
        ///###2. Query string (URI)###
        ///     O Id deve ser passado no padrão REST, ou seja, logo após da API, incluir "/" e o número do Id.
        ///     **Vide seção 5: Exemplo de Requisição**
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
        /// ###5. Exemplo de Requisição###
        ///     <![CDATA[http://meuservidor/api/pessoas/124]]>
        ///     
        ///</remarks>
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
                _log.Information("[ObterPessoa] Iniciando...");

                if (id <= 0)
                {
                    _log.Debug("[ObterPessoa] Id inválido: " + id.ToString());
                    return Request.CreateResponse(System.Net.HttpStatusCode.BadRequest, _pService.GerarErro());
                }

                var p = _pService.Obter(id);
                if (p != null)
                {
                    _log.Debug("[ObterPessoa] Pessoa encontrada: \n" + p.ToJson());
                    _log.Information("[ObterPessoa] Finalizado.");
                    return Request.CreateResponse(System.Net.HttpStatusCode.OK, p);
                }
                else
                {
                    _log.Warning("[ObterPessoa] Finalizado.");
                    return Request.CreateResponse(System.Net.HttpStatusCode.NotFound);
                }
            }
            catch (Exception e)
            {
                _log.Error(e, "[ObterPessoa] Erro Interno");
                return Request.CreateResponse(System.Net.HttpStatusCode.InternalServerError);
            }
        }

        #region anotacoes Swagger para documentacao da api
        ///<summary>
        ///Exclui uma pessoa 
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
        ///###2. Query string (URI)###
        ///     O Id deve ser passado no padrão REST, ou seja, logo após da API, incluir "/" e o número do Id.
        ///     **Vide seção 5: Exemplo de Requisição**
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
        /// ###5. Exemplo de Requisição###
        ///     <![CDATA[http://meuservidor/api/pessoas/124]]>
        ///     
        ///</remarks>              
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
                _log.Information("[Excluir] Iniciando...");
                _log.Debug("[Excluir] Id: " + id.ToString());

                if (id <= 0)
                {
                    _log.Warning("[Excluir] Id <= 0.");
                    return Request.CreateResponse(System.Net.HttpStatusCode.BadRequest, _pService.GerarErro());
                }

                _log.Information("[Excluir] Excluindo...");
                _pService.Excluir(id);
                _log.Information("[Excluir] Finalizado.");
                return Request.CreateResponse(System.Net.HttpStatusCode.OK);
            }
            catch(Exception e)
            {
                _log.Information(e, "[Excluir] Erro interno");
                return Request.CreateResponse(System.Net.HttpStatusCode.InternalServerError);
            }
        }

        #region anotacoes Swagger para documentacao da api
        /// <summary>
        /// Incluir uma pessoa nova
        /// </summary>
        /// <remarks>
        ///Para acionar esse método é necessário seguir os seguintes passos para montagem da requisição:
        ///###1. Cabeçalho (Header) do Http Request###
        ///Informe através da variável `api-version` a versão do endpoint que você está usando.
        ///
        ///**Atenção:** Se `api-version` não for informada, a Web Api executará sua versão mais nova, podendo causar uma problema em sua aplicação de consumo.
        ///Verifique sempre em sua `Http Response` se a Web Api Consinco retorna uma varíavel `api-deprecated-versions`.
        ///Isso significa que a versão do endpoint está obselta e a mesma poderá deixar de funcionar em versões futuras do ERP (você receberá response com `HttpStatusCode = 400 (Bad Request)`.
        /// 
        ///###2. Query string (URI)###
        ///[não se aplica]
        /// 
        ///###3. Corpo (Body) do Http Request###
        ///Enviar a estrutura em formato JSON conforme seção abaixo: **Response Class**
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
        ///</remarks>
        [SwaggerResponseRemoveDefaults()]
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
                _log.Information("[Incluir] Iniciando...");
                _log.Debug("[Incluir] Pessoa:" + pessoa.ToJson());

                if (!ModelState.IsValid || pessoa == null)
                {
                    _log.Error("[Incluir] Formato da requisicao invalida.");
                    return Request.CreateResponse(System.Net.HttpStatusCode.BadRequest, _pService.GerarErro());
                }

                _log.Information("[Incluir] Validando pessoa...");
                var erros = _pService.ValidarRequisicao(pessoa, true);
                if (erros.Count > 0)
                {
                    _log.Warning("[Incluir] Inconsistencias: \n" + erros);
                    return Request.CreateResponse(System.Net.HttpStatusCode.BadRequest, erros);
                }

                _log.Information("[Incluir] Criando pessoa...");
                var p = _pService.Novo(pessoa);

                _log.Debug("[Incluir] Pessoa criada, Id: " + p.Id.ToString());
                _log.Information("[Incluir] Finalizado.");
                return Request.CreateResponse(System.Net.HttpStatusCode.Created, p);                
            }
            catch (Exception e)
            {
                _log.Error(e, "[Incluir] Erro interno");
                return Request.CreateResponse(System.Net.HttpStatusCode.InternalServerError);
            }
        }

        #region anotacoes Swagger para documentacao da api
        /// <summary>
        /// Atualizar todos os dados de uma pessoa.
        /// </summary>
        /// <remarks>
        ///Para acionar esse método é necessário seguir os seguintes passos para montagem da requisição:
        ///###1. Cabeçalho (Header) do Http Request###
        ///Informe através da variável `api-version` a versão do endpoint que você está usando.
        ///
        ///**Atenção:** Se `api-version` não for informada, a Web Api executará sua versão mais nova, podendo causar uma problema em sua aplicação de consumo.
        ///Verifique sempre em sua `Http Response` se a Web Api Consinco retorna uma varíavel `api-deprecated-versions`.
        ///Isso significa que a versão do endpoint está obselta e a mesma poderá deixar de funcionar em versões futuras do ERP (você receberá response com `HttpStatusCode = 400 (Bad Request)`.
        /// 
        ///###2. Query string (URI)###
        ///[não se aplica]
        /// 
        ///###3. Corpo (Body) do Http Request###
        ///Enviar a estrutura em formato JSON conforme seção abaixo: **Response Class**
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
        ///</remarks>              
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
                _log.Information("[Atualizar] Iniciando...");
                _log.Debug("[Atualizar] Pessoa: \n" + pessoa.ToJson());

                if (!ModelState.IsValid ||  pessoa == null)
                {
                    _log.Error("[Incluir] Formato da requisicao invalida.");
                    return Request.CreateResponse(System.Net.HttpStatusCode.BadRequest, _pService.GerarErro());
                }

                _log.Information("[Incluir] Validando dados...");
                _log.Debug("[Incluir] Procurando pessoa por id: " +  pessoa.Id.ToString());
                if (_pService.PessoaExiste(pessoa.Id))
                {
                    var erros = _pService.ValidarRequisicao(pessoa);
                    if (erros.Count > 0)
                    {
                        _log.Warning("[Incluir] Inconsistências: \n" + erros);
                        return Request.CreateResponse(System.Net.HttpStatusCode.BadRequest, erros);
                    }

                    _log.Information("[Incluir] Atualizando dados...");
                    _pService.Atualizar(pessoa);

                    _log.Information("[Incluir] Finalizado.");
                    return Request.CreateResponse(System.Net.HttpStatusCode.OK);
                }
                else
                {
                    _log.Warning("[Incluir] Pessoa Id: " + pessoa.Id.ToString() + " não encontrado.");
                    return Request.CreateResponse(System.Net.HttpStatusCode.NotFound);
                }
            }
            catch (Exception e)
            {
                _log.Debug(e, "[Incluir] Erro Interno");
                return Request.CreateResponse(System.Net.HttpStatusCode.InternalServerError);
            }
        }

        #region anotacoes Swagger para documentacao da api
        ///<summary>
        ///Atualizar 1 ou mais dados de uma pessoa.
        ///</summary>
        ///<remarks>
        ///Importante destacar que esse Endpoint tem uma diferência em relação ao `PUT`.
        ///Aqui é possível atualizar apenas 1 campo ou quantos campos você precisar, enquanto o `PUT` lhe obriga a enviar todos os campos para que todos sejam atualizados. 
        ///
        /// **Atenção:** recomendamos analisar bem o desenvolvimento de sua solução Client para que a atualização de dados desnecessário de um campo cujo conteúdo na prática continuará o mesmo, não provoce disparo de processos no **Acrux ERP**.
        /// 
        ///Para acionar esse método é necessário seguir os seguintes passos para montagem da requisição:
        ///###1. Cabeçalho (Header) do Http Request###
        ///Informe através da variável `api-version` a versão do endpoint que você está usando.
        ///
        ///**Atenção:** Se `api-version` não for informada, a Web Api executará sua versão mais nova, podendo causar uma problema em sua aplicação de consumo.
        ///Verifique sempre em sua `Http Response` se a Web Api Consinco retorna uma varíavel `api-deprecated-versions`.
        ///Isso significa que a versão do endpoint está obselta e a mesma poderá deixar de funcionar em versões futuras do ERP (você receberá response com `HttpStatusCode = 400 (Bad Request)`.
        /// 
        ///###2. Query string (URI)###
        ///[não se aplica]
        /// 
        ///###3. Corpo (Body) do Http Request###
        ///Enviar a estrutura em formato JSON conforme seção abaixo: **Response Class**
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
        ///</remarks>               
        [SwaggerResponse(System.Net.HttpStatusCode.BadRequest, "Parâmetros inválidos.", typeof(Erro))]
        [SwaggerResponse(System.Net.HttpStatusCode.InternalServerError, "Erro ao incluir uma nova pessoa.")]
        [SwaggerResponse(System.Net.HttpStatusCode.OK, "Requisição processada com sucesso.")]
        [SwaggerResponse(System.Net.HttpStatusCode.Unauthorized, "Sem permissão para executar método.")]
        #endregion
        [HttpPatch]
        [Route("{id}")]
        public HttpResponseMessage AtualizarParcial(long id, [FromBody] PessoaPatch pessoa)
        {
            try
            {
                _log.Information("[AtualizarParcial] Iniciando...");
                _log.Debug("[AtualizarParcial] Pessoa Id: " + id.ToString() + "\n" + pessoa.ToJson());

                if (!ModelState.IsValid || pessoa == null)
                {
                    _log.Error("[AtualizarParcial] Formato da requisicao invalida.");
                    return Request.CreateResponse(System.Net.HttpStatusCode.BadRequest, _pService.GerarErro());
                }

                _log.Information("[AtualizarParcial] Validando id: " + id.ToString());
                if (!_pService.PessoaExiste(id))
                {
                    _log.Warning("[AtualizarParcial] Id da pessoa não encontrado.");
                    return Request.CreateResponse(System.Net.HttpStatusCode.NotFound);
                }

                var erros = _pService.ValidarRequisicao(id, pessoa);
                if (erros.Count > 0)
                {
                    _log.Warning("[AtualizarParcial] Inconsistencias:\n" + erros);
                    return Request.CreateResponse(System.Net.HttpStatusCode.BadRequest, erros);
                }

                _log.Warning("[AtualizarParcial] Atualizando dados...");
                _pService.Atualizar(id, pessoa);

                _log.Warning("[AtualizarParcial] Finalizado.");
                return Request.CreateResponse(System.Net.HttpStatusCode.OK);                
            }
            catch (Exception e)
            {
                _log.Error(e, "[AtualizarParcial] Erro interno");
                return Request.CreateResponse(System.Net.HttpStatusCode.InternalServerError);
            }
        }
    }
}
