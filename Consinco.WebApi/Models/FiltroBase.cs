using Consinco.WebApi.Helpers;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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

        /// <summary>
        /// Número da Página que está consultando (inicie com 1)
        /// </summary>
        [Required]
        public int Pagina { get; set; }

        /// <summary>
        /// Qtde de registros que são retornados em uma página (máx 100)
        /// </summary>
        [Required]
        public int TamanhoPagina { get; set; }

        /// <summary>
        /// Possibilidade de ordenar os dados retornados em uma página
        /// </summary>
        public string Ordenacao { get; set; }
        
        public IDictionary<string, string> ObterOrdenacoes()
        {
            IDictionary<string, string> ret = null;

            if (Ordenacao!= null)
            {
                ret = OrdenacaoHelper.ObterOrdenadores(Ordenacao);
            }      
            
            if (ret == null)
            {
                ret = new Dictionary<string, string>();
            }

            return ret;
        }
    }
}