using Newtonsoft.Json;
using System;

namespace Consinco.WebApi.Models.Pessoas
{
    // Classe usada para realizar consultas no banco usando alguns atributos 
    // de pessoa + dados sobre quantidade de registros a devolver
    public class PessoaFiltro : FiltroBase
    {
        // declarar apenas os atributos que fazem sentido para realização de condições de pesquisa

        /// <summary>
        /// Nome Completo da pessoa física ou Razão Social da pessoa jurídica
        /// </summary>
        public string NomeCompleto { get; set; }
        /// <summary>
        /// Apelido da pessoa física ou Nome Fantasia da pessoa jurídica
        /// </summary>
        public string NomeReduzido { get; set; }
        /// <summary>
        /// Tipo da Pessoa: (F)ísica ou (J)urídica
        /// </summary>
        public string Tipo { get; set; }
        /// <summary>
        /// Data em que a pessoa foi inserida no sistema
        /// </summary>
        public DateTime? CadastradoEm { get; set; }        
        
        public string toJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}