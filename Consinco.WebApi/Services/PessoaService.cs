using Consinco.WebApi.Helpers;
using Consinco.WebApi.Models.Errors;
using Consinco.WebApi.Models.Pessoas;
using Consinco.WebApi.Repositories.Pessoas;
using System.Collections;
using System.Collections.Generic;

namespace Consinco.WebApi.Services
{
    public class PessoaService
    {
        private readonly IPessoaRepository _repo;

        public PessoaService()
        {
            _repo = new PessoaRepository();
        }

        public Pessoa Obter(long id)
        {
            return _repo.Obter(id);
        }

        public PessoaPaginado Obter(PessoaFiltro filtro)
        {
            return _repo.Obter(filtro);
        }

        public List<Erro> ValidarRequisicao(PessoaFiltro filtro)
        {
            List<Erro> ret = new List<Erro>();

            if (!OrdenacaoHelper.OrdenacaoValida(typeof(Pessoa), filtro.ordenacao))
            {
                ret.Add( new Erro {
                    Codigo = 9000,
                    Descricao = "Requisição inválida.",
                    Mensagem = "Parâmetro de ordenação inválido."
                });
            }

            return ret;
        }

        public Erro GerarErro(string mensagem)
        {
            return new Erro
                {
                    Codigo = 9999,
                    Descricao = "Requisição inválida.",
                    Mensagem = mensagem
                };            
        }
    }
}