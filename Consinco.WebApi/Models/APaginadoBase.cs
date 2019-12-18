using System.Collections.Generic;

namespace Consinco.WebApi.Models
{
    public abstract class APaginadoBase<TFiltro, TEntidade> 
        where TEntidade: class
        where TFiltro: class
    {
        public TFiltro filtro { get; set; }
        public string PaginaAnterior { get; set; }
        public string ProximaPagina { get; set; }
        public List<TEntidade> Resultados { get; set; }        
    }
}