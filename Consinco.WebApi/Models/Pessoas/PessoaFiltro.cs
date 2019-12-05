using System.Collections;

namespace Consinco.WebApi.Models.Pessoas
{
    // Classe usada para realizar consultas no banco usando alguns atributos 
    // de pessoa + dados sobre quantidade de registros a devolver
    public class PessoaFiltro : Pessoa
    {
        public int Pagina { get; set; }
        public int TamanhoPagina { get; set; }
        public Hashtable Ordenacoes { get; set; }
        
        //inicialize sempre o construtor default caso o client não tenha lhe enviado o dado
        public PessoaFiltro()
        {
            Pagina = 1;
            TamanhoPagina = 25;
        }        
    }
}