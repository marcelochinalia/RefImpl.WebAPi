using System;

namespace Consinco.WebApi.Models.Pessoas
{
    // Classe usada para realizar consultas no banco usando alguns atributos 
    // de pessoa + dados sobre quantidade de registros a devolver
    public class PessoaFiltro : FiltroBase
    {
        // declarar apenas os atributos que fazem sentido para realização de condições de pesquisa
        public string NomeCompleto { get; set; }
        public string NomeReduzido { get; set; }
        public string Tipo { get; set; }
        public DateTime? CadastradoEm { get; set; }               
    }
}