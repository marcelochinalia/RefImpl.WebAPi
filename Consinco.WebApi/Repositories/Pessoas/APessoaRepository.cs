using Consinco.WebApi.Helpers;
using Consinco.WebApi.Models.Pessoas;
using System.Collections.Generic;
using System.Configuration;

namespace Consinco.WebApi.Repositories.Pessoas
{
    // Classe que abstrai da implementação concreta do Repositório as complexidades da técnica de paginação no banco de dados
    public abstract class APessoaRepository : IPessoaRepository
    {
        protected string _connStr;

        public APessoaRepository()
        {
            _connStr = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        }

        abstract public bool Atualizar(Pessoa pessoa);
        abstract public bool Excluir(long id);
        abstract public Pessoa Novo(Pessoa pessoa);
        abstract public Pessoa Obter(long id);
        abstract public PessoaPaginado Obter(PessoaFiltro filtro);

        protected PessoaPaginado TratarPaginacao(PessoaFiltro filtro, List<Pessoa> pessoas)
        {
            PessoaPaginado paginado = new PessoaPaginado();

            paginado.filtro = filtro;
            paginado.Pessoas = pessoas != null ? pessoas : new List<Pessoa>();

            paginado.ProximaPagina = "0";
            paginado.PaginaAnterior = filtro.Pagina > 1 ? (filtro.Pagina - 1).ToString() : "0";

            if (paginado.Pessoas.Count > filtro.TamanhoPagina)
            {
                paginado.ProximaPagina = (paginado.filtro.Pagina + 1).ToString();
                paginado.Pessoas.RemoveAt(paginado.Pessoas.Count - 1);
            }

            return paginado;
        }

        protected Paginacao CalcularPaginacao(int pagina, int tamanhoPagina)
        {
            return PaginacaoHelper.Calcular(pagina, tamanhoPagina);
        }

        protected string MontaOrdenacao(IDictionary<string, string> ordenacoes, IDictionary<int, string> alias, string ordenacaoPadrao)
        {
            string ret = (ordenacaoPadrao == null ? "" : ordenacaoPadrao)  + ",";

           

            return ret.Substring(0, ret.Length - 1);
        }
    }
}