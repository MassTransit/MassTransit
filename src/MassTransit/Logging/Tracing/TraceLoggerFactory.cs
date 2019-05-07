namespace MassTransit.Logging.Tracing
{
    using System.Diagnostics;
    using Microsoft.Extensions.Logging;


    class TraceLoggerFactory : ILoggerFactory
    {
        readonly ILoggerProvider _provider;

        public TraceLoggerFactory()
        {
            var defaultSource = new TraceSource("Default", SourceLevels.Information);
            _provider = new ConsoleTraceLoggerProvider(defaultSource);

            CreateLogger("MassTransit");
        }

        public void Dispose()
        {
            Trace.Flush();
            _provider.Dispose();
        }

        public ILogger CreateLogger(string categoryName) => _provider.CreateLogger(categoryName);

        public void AddProvider(ILoggerProvider provider)
        {
        }
    }
}
