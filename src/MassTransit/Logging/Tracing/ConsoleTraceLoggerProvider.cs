namespace MassTransit.Logging.Tracing
{
    using System.Collections.Concurrent;
    using System.Diagnostics;
    using Microsoft.Extensions.Logging;


    class ConsoleTraceLoggerProvider : ILoggerProvider
    {
        readonly TraceSource _source;
        readonly ConcurrentDictionary<string, ILogger> _loggers = new ConcurrentDictionary<string, ILogger>();
        readonly ConcurrentDictionary<string, TraceSource> _sources;
        readonly TraceListener _listener;

        public ConsoleTraceLoggerProvider(TraceSource source)
        {
            _source = source;
            _sources = new ConcurrentDictionary<string, TraceSource>();
            _listener = AddDefaultConsoleTraceListener(_source);
        }

        public void Dispose()
        {
            if (_listener == null)
                return;

            Trace.Listeners.Remove(_listener);

            _listener.Flush();
            _listener.Dispose();
        }

        public ILogger CreateLogger(string categoryName) => _loggers.GetOrAdd(categoryName, CreateTraceLog);

        ILogger CreateTraceLog(string name) => new ConsoleTraceLogger(name, _sources.GetOrAdd(name, CreateTraceSource));

        TraceSource CreateTraceSource(string name)
        {
            const LogLevel logLevel = LogLevel.Information;
            var sourceLevel = ToSourceLevels(logLevel);
            var source = new TraceSource(name, sourceLevel);
            if (IsSourceConfigured(source))
                return source;

            ConfigureTraceSource(source, name, sourceLevel);

            return source;
        }

        static SourceLevels ToSourceLevels(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Information:
                    return SourceLevels.Information;

                case LogLevel.Trace:
                case LogLevel.Debug:
                    return SourceLevels.Verbose;

                case LogLevel.Warning:
                    return SourceLevels.Warning;

                case LogLevel.Error:
                    return SourceLevels.Error;

                case LogLevel.Critical:
                    return SourceLevels.Critical;

                default:
                    return SourceLevels.Off;
            }
        }

        void ConfigureTraceSource(TraceSource source, string name, SourceLevels sourceLevel)
        {
            var defaultSource = _source;
            for (var parentName = ShortenName(name);
                !string.IsNullOrEmpty(parentName);
                parentName = ShortenName(parentName))
            {
                var parentSource = new TraceSource(parentName, sourceLevel);
                if (!IsSourceConfigured(parentSource))
                    continue;

                defaultSource = parentSource;
                break;
            }

            source.Switch = defaultSource.Switch;
            source.Listeners.Clear();
            foreach (TraceListener listener in defaultSource.Listeners)
                source.Listeners.Add(listener);
        }

        static string ShortenName(string name)
        {
            var length = name.LastIndexOf('.');

            return length != -1
                ? name.Substring(0, length)
                : null;
        }

        static bool IsSourceConfigured(TraceSource source) =>
            source.Listeners.Count != 1
            || !(source.Listeners[0] is DefaultTraceListener)
            || source.Listeners[0].Name != "Default";

        static TraceListener AddDefaultConsoleTraceListener(TraceSource source)
        {
            var listener = new ConsoleTraceListener {Name = "MassTransit"};

            source.Listeners.Add(listener);

            return listener;
        }
    }
}
