using Newtonsoft.Json;

namespace Consinco.WebApi.Models.Pessoas
{
    // Esse pattern é criado para quando você precisar construir 
    // um método Patch em uma Controller do seu Web Api
    public class PessoaPatch
    {
        /// <summary>
        /// Nome Completo da pessoa física ou Razão Social da pessoa jurídica
        /// </summary>
        public string NomeCompleto { get; set; }
        /// <summary>
        /// Apelido da pessoa física ou Nome Fantasia da pessoa jurídica
        /// </summary>
        public string NomeReduzido { get; set; }        

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}