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

        public PessoaPaginado Obter(int pagina, int tamanhoPagina, Pessoa filtro, string ordenacao)
        {
            return _repo.Obter(new PessoaFiltro
            {
                Pagina = pagina,
                TamanhoPagina = tamanhoPagina,
                Ordenacoes = OrdenacaoHelper.ObterOrdenadores(ordenacao),
                NomeCompleto = filtro.NomeCompleto,
                NomeReduzido = filtro.NomeReduzido,
                Tipo = filtro.Tipo
            });
        }

        public List<string> ValidarRequisicao(string ordenacao)
        {
            List<string> ret = new List<string>();

            if (!OrdenacaoHelper.OrdenacaoValida(typeof(Pessoa), ordenacao))
            {
                ret.Add("Parâmetro de Ordenação Inválido");
            }

            return ret;
        }
    }
}