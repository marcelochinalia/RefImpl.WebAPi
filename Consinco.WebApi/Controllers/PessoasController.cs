using Consinco.WebApi.Models.Errors;
using Consinco.WebApi.Models.Pessoas;
using Consinco.WebApi.Services;
using Swashbuckle.Swagger.Annotations;
using System.Web.Http;
using System.Net.Http;
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
        
        //TODO Colocar Logger

        public PessoasController()
        {
            _pService = new PessoaService();
        }

        #region anotacoes Swagger para documentacao da api
///<summary>
/// Retorna uma lista de pessoas.        
/// </summary>
///<remarks>
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
///</remarks>       
/*
 {
     protocolo://meuservidor/api/pessoas?pagina=1&tamanhoPagina=50&nomecompleto=MARC&tipo=J&ordenacao=nomecompleto:asc,cadastradoem:desc
     protocolo://meuservidor/api/pessoas?pagina=1&tamanhoPagina=50&ordenacao=nomecompleto
     protocolo://meuservidor/api/pessoas?pagina=1&tamanhoPagina=50
 }
 */
[SwaggerResponse(System.Net.HttpStatusCode.OK, "Requisição processada com sucesso.", typeof(PessoaPaginado))]
[SwaggerResponse(System.Net.HttpStatusCode.BadRequest, "Formato de requisição inválido ou parametrização errada.", typeof(PessoaPaginado))]
[SwaggerResponse(System.Net.HttpStatusCode.NotFound, "Não foram encontrados registros para a página informada.")]
[SwaggerResponse(System.Net.HttpStatusCode.Unauthorized, "Sem permissão para executar método.")]
#endregion
[HttpGet]
[Route("")]
public HttpResponseMessage ObterPessoa([FromUri] PessoaFiltro filtro)
{
    try {
        if (!ModelState.IsValid)
        {
            return Request.CreateResponse(System.Net.HttpStatusCode.BadRequest, _pService.GerarErro());
        }

        var erros = _pService.ValidarRequisicao(filtro);
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
    catch (Exception) {
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
