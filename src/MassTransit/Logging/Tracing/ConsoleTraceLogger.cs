namespace MassTransit.Logging.Tracing
{
    using System;
    using System.Diagnostics;
    using Microsoft.Extensions.Logging;


    class ConsoleTraceLogger : ILogger
    {
        readonly TraceSource _source;
        readonly string _name;

        public ConsoleTraceLogger(string name, TraceSource source)
        {
            _name = name;
            _source = source;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
                return;

            if (formatter == null)
                throw new ArgumentNullException(nameof(formatter));

            var message = formatter(state, exception);

            if (!string.IsNullOrEmpty(message) || exception != null)
                LogInternal(logLevel, _name, eventId.Id, message, exception);
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            if (logLevel == LogLevel.None)
                return false;


            var traceEventType = GetEventType(logLevel);
            return _source.Switch.ShouldTrace(traceEventType);
        }

        public IDisposable BeginScope<TState>(TState state) => new TraceSourceScope(state);

        void LogInternal(LogLevel level, string name, int eventId, string message, Exception exception)
        {
            var traceLevel = GetEventType(level);
            if (exception == null)
                _source.TraceEvent(traceLevel, eventId, message);
            else
                _source.TraceData(traceLevel, eventId, message, exception);
        }

        static TraceEventType GetEventType(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Critical:
                    return TraceEventType.Critical;

                case LogLevel.Error:
                    return TraceEventType.Error;

                case LogLevel.Warning:
                    return TraceEventType.Warning;

                case LogLevel.Information:
                    return TraceEventType.Information;

                case LogLevel.Trace:
                default:
                    return TraceEventType.Verbose;
            }
        }


        class TraceSourceScope : IDisposable
        {
            // To detect redundant calls
            bool _isDisposed;

            /// <summary>
            /// Pushes state onto the LogicalOperationStack by calling
            /// <see cref="CorrelationManager.StartLogicalOperation(object)"/>
            /// </summary>
            /// <param name="state">The state.</param>
            public TraceSourceScope(object state)
            {
                Trace.CorrelationManager.StartLogicalOperation(state);
            }

            /// <summary>
            /// Pops a state off the LogicalOperationStack by calling
            /// <see cref="CorrelationManager.StopLogicalOperation()"/>
            /// </summary>
            public void Dispose()
            {
                if (_isDisposed)
                    return;

                Trace.CorrelationManager.StopLogicalOperation();
                _isDisposed = true;
            }
        }
    }
}
