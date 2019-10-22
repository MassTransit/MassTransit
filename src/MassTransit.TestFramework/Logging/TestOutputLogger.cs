namespace MassTransit.TestFramework.Logging
{
    using System;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;


    public class TestOutputLogger<T> :
        ILogger<T>
    {
        readonly ILogger _logger;

        public TestOutputLogger(ILoggerFactory factory)
        {
            _logger = factory.CreateLogger<T>();
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return _logger.BeginScope(state);
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return _logger.IsEnabled(logLevel);
        }

        public void Log<TState>(LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception exception,
            Func<TState, Exception, string> formatter)
        {
            _logger.Log(logLevel, eventId, state, exception, formatter);
        }
    }


    public class TestOutputLogger :
        ILogger
    {
        object _scope;
        readonly Func<LogLevel, bool> _filter;

        public TestOutputLogger(bool enabled)
            : this(_ => enabled)
        {
        }

        public TestOutputLogger(Func<LogLevel, bool> filter)
        {
            _filter = filter;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            _scope = state;

            return TestDisposable.Instance;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            if (formatter == null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }

            var message = formatter(state, exception);

            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            message = $"{DateTime.Now:HH:mm:ss.fff}-{logLevel.ToString()[0]} {message}";

            if (exception != null)
            {
                message += Environment.NewLine + Environment.NewLine + exception;
            }

            TestContext.WriteLine(message);
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel != LogLevel.None && _filter(logLevel);
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
