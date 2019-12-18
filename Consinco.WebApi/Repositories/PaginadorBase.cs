using Consinco.WebApi.Models;
using System.Collections;
using System.Collections.Generic;

namespace Consinco.WebApi.Repositories
{
    public class PaginadorBase<TFiltro, TPaginado, TEntidade>
        where TFiltro: FiltroBase
        where TPaginado : APaginadoBase<TFiltro, TEntidade>
        where TEntidade : class
    {
        
        protected TPaginado TratarPaginacao(TFiltro filtro, List<TEntidade> lista, TPaginado paginado)
        {
            paginado.filtro = filtro;
            paginado.Resultados = lista != null ? lista : new List<TEntidade>();

            paginado.ProximaPagina = "0";
            paginado.PaginaAnterior = filtro.Pagina > 1 ? (filtro.Pagina - 1).ToString() : "0";

            if (paginado.Resultados.Count > filtro.TamanhoPagina)
            {
                paginado.ProximaPagina = (paginado.filtro.Pagina + 1).ToString();
                paginado.Resultados.RemoveAt(paginado.Resultados.Count - 1);
            }        

            return paginado;
        }

        public static Hashtable Calcular(int pagina, int tamanhoPagina)
        {
            pagina = pagina <= 0 ? 1 : pagina;
            int meuTamanho = tamanhoPagina <= 0 || tamanhoPagina > 200 ? 50 : tamanhoPagina;

            int inicio = (meuTamanho * (pagina - 1)) + 1;
            int final = inicio + meuTamanho;

            Hashtable p = new Hashtable();
            p.Add("inicio", inicio);
            p.Add("fim", final);
            
            return p;
        }
    }
}