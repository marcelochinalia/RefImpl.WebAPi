namespace Consinco.WebApi.Helpers
{
    public class Paginacao
    {
        public int Inicio;
        public int Final;                        
    }

    public class PaginacaoHelper
    {
        private PaginacaoHelper()
        {

        }

        public static Paginacao Calcular(int pagina, int tamanhoPagina)
        {
            pagina = pagina <= 0 ? 1 : pagina;
            int meuTamanho = tamanhoPagina <= 0 || tamanhoPagina > 200 ? 50 : tamanhoPagina;

            int inicio = (meuTamanho * (pagina - 1)) + 1;
            int final = inicio + meuTamanho;

            Paginacao p = new Paginacao
            {
                Inicio = inicio,
                Final = final        
            };

            return p;
        }        
    }
}