using Serilog.Events;
using Serilog;

namespace Shared.Hosting
{
    public class SerilogConfigurationHelper
    {
        public static void Configure(string applicationName)
        {
            Log.Logger = new LoggerConfiguration()
#if DEBUG
           .MinimumLevel.Debug()
#else
            .MinimumLevel.Information()
#endif
           .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
           .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
           .Enrich.FromLogContext()
           .Enrich.WithProperty("Application", $"{applicationName}")
           .WriteTo.Async(c => c.File($"{AppDomain.CurrentDomain.BaseDirectory}/Logs/logs.txt"))
           .WriteTo.Async(c => c.Console())
           .CreateLogger();
        }
    }
}