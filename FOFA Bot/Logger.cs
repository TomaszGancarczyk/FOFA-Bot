using Serilog;


namespace FOFA_Bot
{
    internal class Logger
    {
        private static readonly Serilog.Core.Logger log = new LoggerConfiguration()
            .WriteTo.File($"..\\..\\..\\Logs\\log_{DateTime.Now.Month}_{DateTime.Now.Year}.txt", outputTemplate:
        "[{Timestamp:dd:MM:yyyy HH:mm:ss}] [{Level:u3}] {Message:lj}{NewLine}{Exception}")
            .WriteTo.File($"..\\..\\..\\Logs\\log_warnings_{DateTime.Now.Month}_{DateTime.Now.Year}.txt", restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Warning, outputTemplate:
        "[{Timestamp:dd:MM:yyyy HH:mm:ss}] [{Level:u3}] {Message:lj}{NewLine}{Exception}")
            .WriteTo.Console(outputTemplate:
        "[{Timestamp:dd:MM:yyyy HH:mm:ss}] [{Level:u3}] {Message:lj}{NewLine}")
            .CreateLogger();

        internal static void LogCritical(string message)
        {
            log.Fatal("{message}", message);
        }
        internal static void LogError(string message)
        {
            log.Error("{message}", message);
        }
        internal static void LogWarning(string message)
        {
            log.Warning("{message}", message);
        }
        internal static void LogInformation(string message)
        {
            log.Information("{message}", message);
        }
    }
}