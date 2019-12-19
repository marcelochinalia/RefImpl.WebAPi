using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Consinco.WebApi.Helpers
{
    public class OrdenacaoHelper
    {
        private OrdenacaoHelper()
        {

        }

        public static bool OrdenacaoValida(Type objType, string ordenacao)
        {
            bool ret = true;
            if (ordenacao != null)
            {
                List<PropertyInfo> properties = objType.GetProperties().ToList();

                string[] ordens = ordenacao.Split(',');
                for (int i = 0; i < ordens.Length; i++)
                {
                    string campo = ordens[i];
                    if (ordens[i].Contains(":"))
                    {
                        campo = ordens[i].Substring(0, campo.IndexOf(":"));
                    }

                    ret = properties.Where(W => campo.ToLower().Contains(W.Name.ToLower())).Any();
                    if (!ret)
                    {
                        ret = false;
                        break;
                    }                    
                }
            }

            return ret;
        }

        public static IDictionary<string, string> ObterOrdenadores(string ordenacao)
        {
            IDictionary<string, string> ret = new Dictionary<string, string>();
            if (ordenacao != null)
            {
                
                string[] ordens = ordenacao.Split(',');
                for (int i = 0; i < ordens.Length; i++)
                {
                    string chave = ordens[i];
                    string valor = "asc";

                    if (chave.Contains(":"))
                    {
                        chave = ordens[i].Substring(0, ordens[i].IndexOf(":"));
                        valor = ordens[i].Substring(ordens[i].IndexOf(":")+1, ordens[i].Length - chave.Length - 1);                        
                    }

                    ret.Add(chave, valor);                    
                }
            }

            return ret;
        }
    }
}