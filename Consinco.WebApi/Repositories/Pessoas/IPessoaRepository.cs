using Consinco.WebApi.Models.Pessoas;
using System.Threading;
using System.Threading.Tasks;

namespace Consinco.WebApi.Repositories.Pessoas
{
    public interface IPessoaRepository
    {
        Pessoa Obter(long id);
        Task<PessoaPaginado> ObterAsync(PessoaFiltro filtro, CancellationToken cancellationToken);
        Pessoa Novo(Pessoa pessoa);
        int Atualizar(Pessoa pessoa);
        int Atualizar(long id, PessoaPatch patch);
        void Excluir(long id);
    }
}
