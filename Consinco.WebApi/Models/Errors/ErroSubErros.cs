﻿namespace Consinco.WebApi.Models.Errors
{
    public partial class ErroSubErros
    {
        public string Codigo { get; set; }
        public string Descricao { get; set; }
        public string Mensagem { get; set; }
        public Erro[] Erros { get; set; }
    }
}