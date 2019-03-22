using Serilog;
using Serilog.Events;

namespace NetCoreCrud.Api
{
    public class Logging
    {
        public static ILogger GetLogger()
        {
            var loggerConfiguration = new LoggerConfiguration()
                .WriteTo.Trace(LogEventLevel.Debug);

            return loggerConfiguration.CreateLogger();
        }
    }
}
