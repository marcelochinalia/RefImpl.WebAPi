using Consinco.WebApi.Models.Pessoas;

namespace Consinco.WebApi.Repositories.Pessoas
{
    public interface IPessoaRepository
    {
        Pessoa Obter(long id);
        PessoaPaginado Obter(PessoaFiltro filtro);
        Pessoa Novo(Pessoa pessoa);
        bool Atualizar(Pessoa pessoa);
        bool Excluir(long id);
    }
}
