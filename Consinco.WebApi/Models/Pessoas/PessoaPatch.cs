namespace Consinco.WebApi.Models.Pessoas
{
    // Esse pattern é criado para quando você precisar construir 
    // um método Patch em uma Controller do seu Web Api
    public class PessoaPatch
    {
        public string NomeCompleto { get; set; }
        public string NomeReduzido { get; set; }        
    }
}