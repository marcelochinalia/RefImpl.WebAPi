using Consinco.WebApi.Models.Pessoas;
using Consinco.WebApi.Helpers;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;
using System.Collections;
using System;
using Dapper;

namespace Consinco.WebApi.Repositories.Pessoas
{
    // O encapsulamento da implementação de paginação depende de você herdar a classe abstrata do Repositório
    public class PessoaRepository : APessoaRepository
    {
        public override int Atualizar(Pessoa pessoa)
        {
            throw new NotImplementedException();
        }

        public override int Atualizar(long id, PessoaPatch patch)
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

        public override void Excluir(long id)
        {
            throw new NotImplementedException();
        }

        public override Pessoa Novo(Pessoa pessoa)
        {
            throw new NotImplementedException();
        }

        // Exemplo usando Dapper para pegar um único registro no banco
        public override Pessoa Obter(long id)
        {
            Pessoa result = null;
            string sql = "SELECT seqpessoa Id, nomerazao NomeCompleto, fantasia NomeReduzido, " +
                         "       fisicajuridica Tipo, dtainclusao CadastradoEm" +
                         "  FROM ge_pessoa " +
                         " WHERE seqpessoa = :Id";

            object parametros = new
            {
                Id = id
            };

            using (OracleConnection connection = new OracleConnection(_connStr))
            {
                result = connection.QueryFirstOrDefault<Pessoa>(@sql, parametros);
            }

            return result;
        }

        // Exemplo usando Dapper e estratégia de Paginação no Banco de Dados
        public override PessoaPaginado Obter(PessoaFiltro filtro)
        {
            List<Pessoa> pessoas = null;
            Paginacao paginacao = CalcularPaginacao(filtro.Pagina, filtro.TamanhoPagina);
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
                Inicio = paginacao.Inicio,
                Final = paginacao.Final
            };
            
            using (OracleConnection connection = new OracleConnection(_connStr))
            {
                pessoas = connection.Query<Pessoa>(@sql, parametros).AsList();                
            }
            
            return TratarPaginacao(filtro, pessoas);
        }        
    }
}