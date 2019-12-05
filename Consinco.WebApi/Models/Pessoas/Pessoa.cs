using System;

namespace Consinco.WebApi.Models.Pessoas
{
    // Classe de abstracao da estrutura do banco de dados
    // Modele sempre sua classe para um objeto do mundo real independentemente como esteja no banco de dados
    public class Pessoa
    {
        public long Id { get; set; }
        public string NomeCompleto { get; set; }
        public string NomeReduzido { get; set; }
        public string Tipo { get; set; }
        public DateTime CadastradoEm { get; set; }
    }
}