using Consinco.WebApi.Models.Pessoas;

namespace Consinco.WebApi.Repositories.Pessoas
{
    public interface IPessoaRepository
    {
        Pessoa Obter(long id);
        PessoaPaginado Obter(PessoaFiltro filtro);
        Pessoa Novo(Pessoa pessoa);
        int Atualizar(Pessoa pessoa);
        int Atualizar(long id, PessoaPatch patch);
        void Excluir(long id);
    }
}
