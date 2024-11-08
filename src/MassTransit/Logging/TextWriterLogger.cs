#nullable enable
namespace MassTransit.Logging
{
    using System;
    using Microsoft.Extensions.Logging;


    public class TextWriterLogger :
        ILogger
    {
        readonly TextWriterLoggerFactory _factory;
        readonly LogLevel _logLevel;

        public TextWriterLogger(TextWriterLoggerFactory factory, LogLevel logLevel)
        {
            _factory = factory;
            _logLevel = logLevel;
        }

        public IDisposable BeginScope<TState>(TState state)
            where TState : notnull
        {
            return TestDisposable.Instance;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel))
                return;

            if (formatter == null)
                throw new ArgumentNullException(nameof(formatter));

            var message = formatter(state, exception);

            if (string.IsNullOrEmpty(message))
                return;

            message = $"{DateTime.Now:HH:mm:ss.fff}-{logLevel.ToString()[0]} {message}";

            if (exception != null)
                message += Environment.NewLine + Environment.NewLine + exception;

            _factory.Writer.WriteLine(message);
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel >= _logLevel;
        }


        class TestDisposable : IDisposable
        {
            public static readonly TestDisposable Instance = new TestDisposable();

            public void Dispose()
            {
                // intentionally does nothing
            }
        }
    }
}
