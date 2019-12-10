using System.Collections.Generic;

namespace Consinco.WebApi.Models.Pessoas
{
    public class PessoaPaginado
    {
        public PessoaFiltro filtro { get; set; }
        public string PaginaAnterior { get; set; }
        public string ProximaPagina { get; set; }
        public List<Pessoa> Pessoas { get; set; }
    }
}