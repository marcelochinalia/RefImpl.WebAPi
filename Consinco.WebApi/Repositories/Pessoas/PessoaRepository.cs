using Consinco.WebApi.Models.Pessoas;
using Consinco.WebApi.Helpers;
using Consinco.WebApi.Logs;
using Oracle.ManagedDataAccess.Client;
using System.Configuration;
using System.Collections.Generic;
using System.Collections;
using System.Threading.Tasks;
using System.Threading;
using Serilog.Core;
using System;
using Dapper;

namespace Consinco.WebApi.Repositories.Pessoas
{
    // O encapsulamento da implementação de paginação depende de você herdar a classe abstrata do Repositório
    public class PessoaRepository : PaginadorBase<PessoaFiltro, PessoaPaginado, Pessoa>, IPessoaRepository
    {
        private readonly string _connStr;
        private readonly Logger _log = PessoasLog.Instace.ObterLogger();

        public PessoaRepository()
        {
            _connStr = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        }

        public int Atualizar(Pessoa pessoa)
        {
            throw new NotImplementedException();
        }

        public int Atualizar(long id, PessoaPatch patch)
        {
            int result = 0;
            Hashtable atributos = PatchHelper.SegmentarAtributos<PessoaPatch>(patch);
            string sql = "update ge_pessoa set ";

            foreach (DictionaryEntry atributo in atributos)
            {
                switch (atributo.Key)
                {
                    case "NomeCompleto":
                        sql += "nomerazao = :NomeCompleto,";
                        break;

                    case "NomeReduzido":
                        sql += "fantasia = :NomeReduzido,";
                        break;

                    default:
                        sql += "";
                        break;
                }
            }

            sql = sql.Substring(0, sql.Length - 1);
            sql += " where seqpessoa = :Id";

            object parametros = new
            {
                Id = id,
                NomeCompleto = atributos["NomeCompleto"],
                NomeReduzido = atributos["NomeReduzido"]
            };

            using (OracleConnection connection = new OracleConnection(_connStr))
            {
                result = connection.Execute(sql, parametros);
            }

            return result;
        }

        public void Excluir(long id)
        {
            throw new NotImplementedException();
        }

        public Pessoa Novo(Pessoa pessoa)
        {
            throw new NotImplementedException();
        }

        // Exemplo usando Dapper para pegar um único registro no banco
        public Pessoa Obter(long id)
        {
            Pessoa result = null;
            string sql = "select seqpessoa Id, nomerazao NomeCompleto, fantasia NomeReduzido, " +
                         "fisicajuridica Tipo, dtainclusao CadastradoEm " +
                         "from ge_pessoa " +
                         "where seqpessoa = :Id";

            object parametros = new
            {
                Id = id
            };

            _log.Debug("[PessoaRepository] [Obter] Id: " + id);
            _log.Debug("[PessoaRepository] [Obter] Query: " + sql);
            _log.Debug("[PessoaRepository] [Obter] Parâmetros Query: " + parametros.ToString());

            using (OracleConnection connection = new OracleConnection(_connStr))
            {
                result = connection.QueryFirstOrDefault<Pessoa>(@sql, parametros);
            }

            return result;
        }

        // Exemplo usando Dapper e estratégia de Paginação no Banco de Dados
        public async Task<PessoaPaginado> ObterAsync(PessoaFiltro filtro, CancellationToken tokenCancel)
        {
            List<Pessoa> pessoas = null;
            Hashtable paginacao = Calcular(filtro.Pagina, filtro.TamanhoPagina);
            string clausulaOrderBy = MontaClausulaOrdenacao(filtro);

            string sql = "  Select * " +
                         "    from ( select a.seqpessoa Id, a.nomerazao NomeCompleto, a.fantasia NomeReduzido, " +
                         "                  a.fisicajuridica Tipo, a.dtainclusao CadastradoEm, " +
                         "                  row_number() over (order by " + clausulaOrderBy + ") line_number " +
                         "             from ge_pessoa a " +
                         "            where 1 = 1 ";

            if (!string.IsNullOrEmpty(filtro.NomeCompleto) && !string.IsNullOrWhiteSpace(filtro.NomeCompleto))
            {
                sql += " and upper(a.nomerazao) like :NomeCompleto ";
            }

            if (!string.IsNullOrEmpty(filtro.NomeReduzido) && !string.IsNullOrWhiteSpace(filtro.NomeReduzido))
            {
                sql += " and upper(a.fantasia) like :NomeReduzido ";
            }

            if (!string.IsNullOrEmpty(filtro.Tipo) && !string.IsNullOrWhiteSpace(filtro.Tipo))
            {
                sql += " and a.fisicajuridica = :Tipo ";
            }

            if (filtro.CadastradoEm != null)
            {
                sql += " and a.dtainclusao = :CadastradoEm";
            }

            sql += " ) " +
                   " where line_number between :Inicio and :Final " +
                   "  order by line_number";

            object parametros = new
            {
                NomeCompleto = filtro.NomeCompleto == null ? "" : "%" + filtro.NomeCompleto.Trim().ToUpper() + "%",
                NomeReduzido = filtro.NomeReduzido == null ? "" : "%" + filtro.NomeReduzido.Trim().ToUpper() + "%",
                Tipo = filtro.Tipo == null ? "" : filtro.Tipo.Trim().ToUpper(),
                CadastradoEm = filtro.CadastradoEm != null ? filtro.CadastradoEm : new DateTime(),
                Inicio = (int)paginacao["inicio"],
                Final = (int)paginacao["fim"],
            };

            using (System.Data.IDbConnection connection = new OracleConnection(_connStr))
            {
                var result = await connection.QueryAsync<Pessoa>(
                    new CommandDefinition(@sql, parametros, cancellationToken: tokenCancel)
                );

                pessoas = result.AsList();
            }
            
            return TratarPaginacao(filtro, pessoas, new PessoaPaginado());
        }

        private string MontaClausulaOrdenacao(PessoaFiltro filtro)
        {
            string clausulaOrderBy = "";
            var ordenacoes = filtro.ObterOrdenacoes();

            // sempre prestar atenção para o alias correto da tabela 
            if (ordenacoes.Count > 0)
            {
                // preste atenção em alias de tabela para fazer a referência correta
                foreach (KeyValuePair<string, string> item in ordenacoes)
                {
                    //compare tudo com letras minúsculas
                    switch (item.Key.ToLower())
                    {
                        case "id":
                            clausulaOrderBy += "a.seqpessoa " + item.Value + ",";
                            break;
                        case "nomecompleto":
                            clausulaOrderBy += "a.nomerazao " + item.Value + ",";
                            break;
                        case "nomereduzido":
                            clausulaOrderBy += "a.fantasia " + item.Value + ",";
                            break;
                        case "tipo":
                            clausulaOrderBy += "a.fisicajuridica " + item.Value + ",";
                            break;
                        case "cadastradoem":
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