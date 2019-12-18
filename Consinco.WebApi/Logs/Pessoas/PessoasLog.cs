using Serilog;
using Serilog.Core;
using System;

namespace Consinco.WebApi.Logs
{
    public class PessoasLog
    {
        private static PessoasLog _instancia = new PessoasLog();
        private readonly Logger _logger;

        private PessoasLog()
        {
            string arquivo = $@"{AppDomain.CurrentDomain.BaseDirectory}logs\pessoas-log-.txt";

            LoggingLevelSwitch nivelLog = LogConfNivel.ObterNivel();

            _logger = new LoggerConfiguration()
                .MinimumLevel.ControlledBy(nivelLog)
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
                .WriteTo.File(arquivo, outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}", rollingInterval: RollingInterval.Day)
                .CreateLogger();                   
        }

        public static PessoasLog Instace
        {
            get {
                return _instancia ?? (_instancia = new PessoasLog());
            }
        }

        public Logger ObterLogger()
        {
            return _logger;
        }        
    }
}