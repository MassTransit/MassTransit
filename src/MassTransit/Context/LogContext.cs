namespace MassTransit.Context
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using Logging;
    using Microsoft.Extensions.Logging;


    public static class LogContext
    {
        public static EnabledLogger? Critical => Current?.Critical;
        public static EnabledLogger? Debug => Current?.Debug;
        public static EnabledLogger? Error => Current?.Error;
        public static EnabledLogger? Info => Current?.Info;
        public static EnabledLogger? Trace => Current?.Trace;
        public static EnabledLogger? Warning => Current?.Warning;

        static readonly AsyncLocal<ILogContext> _current;

        static LogContext()
        {
            _current = new AsyncLocal<ILogContext>();
        }

        /// <summary>
        /// Gets or sets the current operation (Activity) for the current thread.  This flows
        /// across async calls.
        /// </summary>
        public static ILogContext Current
        {
            get => _current.Value;
            set => _current.Value = value;
        }

        public static void ConfigureCurrentLogContext(ILoggerFactory loggerFactory = null)
        {
            Current = new BusLogContext(loggerFactory ?? NullLoggerFactory.Instance, Cached.Default.Value);
        }

        /// <summary>
        /// Configure the current <see cref="LogContext"/> using the specified <paramref name="logger"/>, which will be
        /// used for all log output.
        /// </summary>
        /// <param name="logger">An existing logger</param>
        public static void ConfigureCurrentLogContext(ILogger logger)
        {
            Current = new BusLogContext(new SingleLoggerFactory(logger), Cached.Default.Value);
        }

        public static EnabledScope? BeginScope()
        {
            return Current?.BeginScope();
        }

        public static ILogContext CreateLogContext(string categoryName)
        {
            var current = Current ??= CreateDefaultLogContext();

            return current.CreateLogContext(categoryName);
        }

        public static void SetCurrentIfNull(ILogContext context)
        {
            if (Current == null)
                Current = context;
        }

        public static LogMessage<T1> Define<T1>(LogLevel logLevel, string formatString)
        {
            var logAction = LoggerMessage.Define<T1>(logLevel, default, formatString);

            void Log(T1 arg1, Exception exception)
            {
                var logContext = Current;
                if (logContext != null)
                    logAction(logContext.Logger, arg1, exception);
            }

            return Log;
        }

        public static LogMessage<T1, T2> Define<T1, T2>(LogLevel logLevel, string formatString)
        {
            var logAction = LoggerMessage.Define<T1, T2>(logLevel, default, formatString);

            void Log(T1 arg1, T2 arg2, Exception exception)
            {
                var logContext = Current;
                if (logContext != null)
                    logAction(logContext.Logger, arg1, arg2, exception);
            }

            return Log;
        }

        public static LogMessage<T1, T2, T3> Define<T1, T2, T3>(LogLevel logLevel, string formatString)
        {
            var logAction = LoggerMessage.Define<T1, T2, T3>(logLevel, default, formatString);

            void Log(T1 arg1, T2 arg2, T3 arg3, Exception exception)
            {
                var logContext = Current;
                if (logContext != null)
                    logAction(logContext.Logger, arg1, arg2, arg3, exception);
            }

            return Log;
        }

        public static LogMessage<T1, T2, T3, T4> Define<T1, T2, T3, T4>(LogLevel logLevel, string formatString)
        {
            var logAction = LoggerMessage.Define<T1, T2, T3, T4>(logLevel, default, formatString);

            void Log(T1 arg1, T2 arg2, T3 arg3, T4 arg4, Exception exception)
            {
                var logContext = Current;
                if (logContext != null)
                    logAction(logContext.Logger, arg1, arg2, arg3, arg4, exception);
            }

            return Log;
        }

        public static LogMessage<T1, T2, T3, T4, T5> Define<T1, T2, T3, T4, T5>(LogLevel logLevel, string formatString)
        {
            var logAction = LoggerMessage.Define<T1, T2, T3, T4, T5>(logLevel, default, formatString);

            void Log(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, Exception exception)
            {
                var logContext = Current;
                if (logContext != null)
                    logAction(logContext.Logger, arg1, arg2, arg3, arg4, arg5, exception);
            }

            return Log;
        }

        static ILogContext CreateDefaultLogContext()
        {
            var source = Cached.Default.Value;

            var loggerFactory = NullLoggerFactory.Instance;

            return new BusLogContext(loggerFactory, source);
        }

        public static EnabledDiagnosticSource? IfEnabled(string name)
        {
            return Current?.IfEnabled(name);
        }


        static class Cached
        {
            internal static readonly Lazy<DiagnosticListener> Default =
                new Lazy<DiagnosticListener>(() => new DiagnosticListener(DiagnosticHeaders.DefaultListenerName));
        }
    }
}
