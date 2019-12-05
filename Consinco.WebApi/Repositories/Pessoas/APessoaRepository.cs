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

        protected string MontaClausulaOrdenacao(PessoaFiltro filtro)
        {
            string clausulaOrderBy = "";
            var ordenacoes = filtro.ObterOrdenacoes();

            // sempre prestar atenção para o alias correto da tabela 
            if (ordenacoes.Count > 0)
            {
                // preste atenção em alias de tabela para fazer a referência correta
                foreach (KeyValuePair<string, string> item in ordenacoes)
                {
                    switch (item.Key)
                    {
                        case "Id":
                            clausulaOrderBy += "a.seqpessoa " + item.Value + ",";
                            break;
                        case "NomeCompleto":
                            clausulaOrderBy += "a.nomerazao " + item.Value + ",";
                            break;
                        case "NomeReduzido":
                            clausulaOrderBy += "a.fantasia " + item.Value + ",";
                            break;
                        case "Tipo":
                            clausulaOrderBy += "a.fisicajuridica " + item.Value + ",";
                            break;
                        case "CadastradoEm":
                            clausulaOrderBy += "a.dtainclusao " + item.Value + ",";
                            break;
                        default:
                            clausulaOrderBy += "";
                            break;
                    }
                }
                clausulaOrderBy = clausulaOrderBy.Substring(0, clausulaOrderBy.Length - 1);
            }
            else
            {
                // informar uma odernação padrão, caso o cliente não informe nenhum tipo de ordenação
                clausulaOrderBy = "a.seqpessoa asc";
            }

            return clausulaOrderBy;
        }
    }
}