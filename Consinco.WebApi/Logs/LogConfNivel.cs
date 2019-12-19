using Newtonsoft.Json;
using Serilog.Core;
using Serilog.Events;
using System;
using System.Collections;
using System.IO;

namespace Consinco.WebApi.Logs
{
    public static class LogConfNivel
    {
        public static LoggingLevelSwitch ObterNivel()
        {
            LoggingLevelSwitch nivelLog = new LoggingLevelSwitch();
            string nivel = LerConfNivelLog();

            switch (nivel.ToLower())
            {
                // nível de log extremo, mostra todas as mensagens de todos os níveis de log
                case "verbose":
                    nivelLog.MinimumLevel = LogEventLevel.Verbose;
                    break;

                // nível de log para rastrear eventos do sistema que não são visíveis em alto nível
                case "debug":
                    nivelLog.MinimumLevel = LogEventLevel.Debug;
                    break;

                // nível de log para rastrear eventos do sistema que são apropriados para exibir em alto nível
                case "info":
                    nivelLog.MinimumLevel = LogEventLevel.Information;
                    break;

                // nível de log para rastrear eventos do sistema que podem revelar algumas condição não atendida ou mapeada pelo sistema
                case "warning":
                    nivelLog.MinimumLevel = LogEventLevel.Warning;
                    break;

                // nível de log para rastrear erros internos ocorridos no sistema (geralmente exceptions)
                case "error":
                    nivelLog.MinimumLevel = LogEventLevel.Error;
                    break;

                // nível de log para rastrear níveis de falhas graves no sistema (conexões com APIs, bancos de dados, filas de mensagens, entre outros)
                case "fatal":
                    nivelLog.MinimumLevel = LogEventLevel.Fatal;
                    break;

                default:
                    nivelLog.MinimumLevel = LogEventLevel.Information;
                    break;
            }

            return nivelLog;
        }

        private static string LerConfNivelLog()
        {
            try
            {
                Hashtable conteudo = new Hashtable();
                conteudo.Add("nivel", "Debug");

                string arquivo = $@"{AppDomain.CurrentDomain.BaseDirectory}Release\log-config.json";
                
                using (StreamReader r = new StreamReader(arquivo))
                {
                    var json = r.ReadToEnd();
                    conteudo = JsonConvert.DeserializeObject<Hashtable>(json);
                }

                return (string)conteudo["nivel"];
            }
            catch (Exception)
            {
                return "Debug";
            }
        }
    }
}