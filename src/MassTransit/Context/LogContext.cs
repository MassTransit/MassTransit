namespace MassTransit.Context
{
    using System;
    using System.Diagnostics;
    using Logging;
    using Microsoft.Extensions.Logging;


    public partial class LogContext
    {
        public static EnabledLogger? Critical => Current?.Critical;
        public static EnabledLogger? Debug => Current?.Debug;
        public static EnabledLogger? Error => Current?.Error;
        public static EnabledLogger? Info => Current?.Info;
        public static EnabledLogger? Trace => Current?.Trace;
        public static EnabledLogger? Warning => Current?.Warning;

        public static void ConfigureCurrentLogContext(ILoggerFactory loggerFactory = null, DiagnosticSource source = null)
        {
            Current = new BusLogContext(loggerFactory ?? NullLoggerFactory.Instance, source ?? Cached.Default.Value);
        }

        /// <summary>
        /// Configure the current <see cref="LogContext"/> using the specified <paramref name="logger"/>, which will be
        /// used for all log output.
        /// </summary>
        /// <param name="logger">An existing logger</param>
        /// <param name="source">An optional custom <see cref="DiagnosticSource"/></param>
        public static void ConfigureCurrentLogContext(ILogger logger, DiagnosticSource source = null)
        {
            Current = new BusLogContext(new SingleLoggerFactory(logger), source ?? Cached.Default.Value);
        }

        public static EnabledScope? BeginScope()
        {
            return Current?.BeginScope();
        }

        public static ILogContext CreateLogContext(string categoryName)
        {
            var current = Current ?? (Current = CreateDefaultLogContext());

            return current.CreateLogContext(categoryName);
        }

        public static void SetCurrentIfNull(ILogContext context)
        {
            if (Current == null)
                Current = context;
        }

        static bool ValidateSetCurrent(ILogContext logContext)
        {
            return true;
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
