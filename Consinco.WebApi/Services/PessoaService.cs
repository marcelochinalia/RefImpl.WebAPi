using Consinco.WebApi.Helpers;
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

        public List<string> ValidarRequisicao(PessoaFiltro filtro)
        {
            List<string> ret = new List<string>();

            if (!OrdenacaoHelper.OrdenacaoValida(typeof(Pessoa), filtro.ordenacao))
            {
                ret.Add("Parâmetro de Ordenação Inválido");
            }

            return ret;
        }
    }
}