using Newtonsoft.Json;
using System;

namespace Consinco.WebApi.Models.Pessoas
{
    // Classe de abstracao da estrutura do banco de dados
    // Modele sempre sua classe para um objeto do mundo real independentemente como esteja no banco de dados
    public class Pessoa
    {
        /// <summary>
        /// Identificador único de uma pessoa
        /// </summary>
        public long Id { get; set; }
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
        public DateTime CadastradoEm { get; set; }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}