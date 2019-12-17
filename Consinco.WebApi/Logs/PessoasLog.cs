using NLog;
using System;
using System.IO;

namespace Consinco.WebApi.Logs
{
    public class PessoasLog
    {
        private static PessoasLog _instancia = new PessoasLog();
        private readonly NLog.Logger _logger;

        private PessoasLog()
        {
            // veja documentação do NLog em: https://github.com/NLog/NLog/wiki/Tutorial#configure-nlog-targets-for-output
            var config = new NLog.Config.LoggingConfiguration();

            // Configura tipos de coleta de logs em arquivo texto e console
            var logfile = new NLog.Targets.FileTarget("logfile") { FileName = ObterNomeArquivoLog() };
            var logconsole = new NLog.Targets.ConsoleTarget("logconsole");

            // Configura os níveis de exibição dos logs por tipo de colta
            // Nível Info: usado para mapear as passagens do código durante seu fluxo de execução ou exibição de queries SQL.
            // Nível Debug: usado em situações de depuração de rotinas complexas ou liberação de exceptions.
            // Nível Fatal: usado em situações onde há tratamento lançamento de exceções ou erros que podem impedir o perfeito funcionamento
            //              da aplicação, como momento de conexão com banco de dados, gravação de um arquivo em disco sem permissão e etc.
            config.AddRule(LogLevel.Trace, LogLevel.Fatal, logconsole);
            config.AddRule(LogLevel.Info, LogLevel.Fatal, logfile);

            // Seta a configuração no gerenciado de Logs           
            NLog.LogManager.Configuration = config;
            _logger = NLog.LogManager.GetCurrentClassLogger();
        }

        public static PessoasLog Instace
        {
            get {
                return _instancia ?? (_instancia = new PessoasLog());
            }
        }

        public NLog.Logger ObterLogger()
        {
            return _logger;
        }

        private string ObterNomeArquivoLog()
        {
            string diretorio = $@"{AppDomain.CurrentDomain.BaseDirectory}logs\";
            string nomeArquivo = "pessoas-log-" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";

            string caminhoCompleto = diretorio + nomeArquivo;

            if (!File.Exists(caminhoCompleto))
            {
                File.Create(caminhoCompleto);
            }
            
            return caminhoCompleto;
        }
    }
}