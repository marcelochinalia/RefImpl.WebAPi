using Consinco.WebApi.Helpers;
using Consinco.WebApi.Models.Errors;
using Consinco.WebApi.Models.Pessoas;
using Consinco.WebApi.Repositories.Pessoas;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Consinco.WebApi.Services
{
    public class PessoaService
    {
        private const string PessoaFisica = "F";
        private const string PessoaJuridica = "J";
        private readonly IPessoaRepository _repo;

        public PessoaService()
        {
            _repo = new PessoaRepository();
        }

        public void Atualizar(Pessoa pessoa)
        {
            int ret = _repo.Atualizar(pessoa);

            if (ret == 0)
            {
                throw new Exception("Erro ao atualizar pessoa.");
            }
        }

        public void Atualizar(long id, PessoaPatch pessoa)
        {
            int ret = _repo.Atualizar(id, pessoa);
            if (ret == 0)
            {
                throw new Exception("Erro ao atualizar pessoa.");
            }
        }

        public Pessoa Obter(long id)
        {
            return _repo.Obter(id);
        }

        public PessoaPaginado Obter(PessoaFiltro filtro)
        {
            return _repo.Obter(filtro);
        }

        public void Excluir(long id)
        {
            _repo.Excluir(id);
        }

        public Pessoa Novo(Pessoa pessoa)
        {
            return _repo.Novo(pessoa);
        }

        public bool PessoaExiste(long id)
        {
            Pessoa p = Obter(id);
            if (p == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public List<Erro> ValidarRequisicao(PessoaFiltro filtro)
        {
            List<Erro> ret = new List<Erro>();

            if (filtro == null)
            {
                ret.Add(GerarErro());
            }
            else
            {
                if (!OrdenacaoHelper.OrdenacaoValida(typeof(Pessoa), filtro.Ordenacao))
                {
                    ret.Add(GerarErro(ErrorsConstants.BusinessErrorCode, "Requisição inválida.", "Parâmetro de ordenação inválido."));
                }
            }

            return ret;
        }
      
        public List<Erro> ValidarRequisicao(Pessoa pessoa, bool inclusao = false)
        {
            List<Erro> ret = new List<Erro>();

            if (pessoa == null)
            {
                ret.Add(GerarErro());
            }
            else
            {
                if (inclusao && pessoa.Id > 0)
                {
                    ret.Add(GerarErro(ErrorsConstants.BusinessErrorCode, "Id inválido.", "Para incluir uma pessoa nova, informe zero."));
                }

                if (string.IsNullOrEmpty(pessoa.NomeCompleto) || string.IsNullOrWhiteSpace(pessoa.NomeCompleto))
                {
                    ret.Add(GerarErro(ErrorsConstants.BusinessErrorCode, "Nome completo não informado.", "Nome completo não informado."));
                }

                if (string.IsNullOrEmpty(pessoa.NomeReduzido) || string.IsNullOrWhiteSpace(pessoa.NomeReduzido))
                {
                    ret.Add(GerarErro(ErrorsConstants.BusinessErrorCode, "Nome reduzido não informado.", "Nome reduzido não informado."));
                }

                if (string.IsNullOrEmpty(pessoa.Tipo) || string.IsNullOrWhiteSpace(pessoa.Tipo))
                {
                    ret.Add(GerarErro(ErrorsConstants.BusinessErrorCode, "Tipo de Pessoa não informado.", "Tipo de Pessoa não informado."));
                }
                else
                {
                    if (!pessoa.Tipo.Equals(PessoaFisica) && !pessoa.Tipo.Equals(PessoaJuridica))
                    {
                        ret.Add(GerarErro(ErrorsConstants.BusinessErrorCode, "Tipo de Pessoa inválido.", "Tipo de Pessoa inválido."));
                    }
                }
            }

            return ret;
        }

        public List<Erro> ValidarRequisicao(long id, PessoaPatch patch)
        {
            List<Erro> ret = new List<Erro>();

            if (patch == null)
            {
                ret.Add(GerarErro());
            }
            else
            {
                if (!PatchHelper.ModeloRequisicaoValida<PessoaPatch>(patch))
                {
                    ret.Add(GerarErro());
                }
                else
                {
                    if (patch.NomeCompleto != null)
                    {
                        if (string.IsNullOrWhiteSpace(patch.NomeCompleto) || patch.NomeCompleto.Trim().Length < 5)
                        {
                            ret.Add(GerarErro(ErrorsConstants.BusinessErrorCode, "Nome completo inválido.", "Nome completo inválido."));
                        }
                    }

                    if (patch.NomeReduzido != null)
                    {
                        if (string.IsNullOrWhiteSpace(patch.NomeReduzido))
                        {
                            ret.Add(GerarErro(ErrorsConstants.BusinessErrorCode, "Nome reduzido inválido.", "Nome reduzido inválido."));
                        }
                    }
                }                
            }

            return ret;
        }

        public Erro GerarErro()
        {
            return GerarErro(ErrorsConstants.BusinessErrorCode, "Formato de requisição inválido.", "Formato de requisição inválido.");
        }

        public Erro GerarErro(long codigo, string descricao, string mensagem)
        {
            return new Erro
                {
                    Codigo = codigo,
                    Descricao = descricao,
                    Mensagem = mensagem
                };            
        }        
    }
}