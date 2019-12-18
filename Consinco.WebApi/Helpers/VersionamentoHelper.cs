using System.Collections;
using System.Reflection;

namespace Consinco.WebApi.Helpers
{
    public static class VersionamentoHelper
    {
        public static string ApiNome = "apiNome";
        public static string ApiVersao = "apiVersao";
        public static string ApiBuild = "apiBuild";

        public static Hashtable ObterVersaoAtualApi()
        {
            Hashtable versao = new Hashtable();

            string apinNome = Assembly.GetExecutingAssembly().GetName().Name.ToString();
            string apiVersao = Assembly.GetExecutingAssembly().GetName().Version.ToString();

            string[] dadosApiVersao = apiVersao.Split('.');

            versao.Add(ApiNome, apinNome);
            versao.Add(ApiVersao, dadosApiVersao[0]);
            versao.Add(ApiBuild, dadosApiVersao[1]);

            return versao;
        }
    }
}