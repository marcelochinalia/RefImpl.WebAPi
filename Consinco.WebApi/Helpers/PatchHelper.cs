using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Collections;

namespace Consinco.WebApi.Helpers
{
    public class PatchHelper
    {
        private PatchHelper()
        {

        }

        private static ConcurrentDictionary<Type, PropertyInfo[]> TypePropertiesCache = new ConcurrentDictionary<Type, PropertyInfo[]>();

        public static bool ModeloRequisicaoValida<TPatch>(TPatch patch)
            where TPatch : class
        {
            bool ret = false;

            PropertyInfo[] properties = TypePropertiesCache.GetOrAdd(
                patch.GetType(),
                (type) => type.GetProperties(BindingFlags.Instance | BindingFlags.Public));

            foreach (PropertyInfo prop in properties)
            {
                object value = prop.GetValue(patch);
                if (value != null)
                {
                    ret = true;
                }
            }

            return ret;
        }

        public static Hashtable SegmentarAtributos<TPatch>(TPatch patch)
            where TPatch : class
        {
            Hashtable parametros = new Hashtable();

            PropertyInfo[] properties = TypePropertiesCache.GetOrAdd(
                patch.GetType(),
                (type) => type.GetProperties(BindingFlags.Instance | BindingFlags.Public));

            foreach (PropertyInfo prop in properties)
            {
                object value = prop.GetValue(patch);
                if (value != null)
                {
                    parametros.Add(prop.Name, value);
                }
            }

            return parametros;            
        }
    }
}