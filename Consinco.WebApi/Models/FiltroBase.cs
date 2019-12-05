using System.Collections.Generic;

namespace Consinco.WebApi.Models
{
    public class FiltroBase
    {
        //inicialize sempre o construtor default caso o client não tenha lhe enviado o dado
        public FiltroBase()
        {
            Pagina = 1;
            TamanhoPagina = 25;
        }

        public int Pagina { get; set; }
        public int TamanhoPagina { get; set; }
        public IDictionary<string, string> Ordenacoes { get; set; }
    }
}