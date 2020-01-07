namespace Consinco.WebApi.Models.Errors
{
    public class Erro
    {
        /// <summary>
        /// Código do erro ou inconsistência
        /// </summary>
        public string Codigo { get; set; }
        /// <summary>
        /// Descrição em alto nível (título) do erro ou inconsistência
        /// </summary>
        public string Descricao { get; set; }
        /// <summary>
        /// Texto mais detalhado do erro ou inconsistência
        /// </summary>
        public string Mensagem { get; set; }
    }
}