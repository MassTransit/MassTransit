namespace MassTransit.Transports.Tests.Logging
{
    using System;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework.Internal;


    public class TestOutputLogger :
        Microsoft.Extensions.Logging.ILogger
    {
        readonly TestOutputLoggerFactory _factory;

        readonly Func<LogLevel, bool> _filter;
        object _scope;

        public TestOutputLogger(TestOutputLoggerFactory factory, bool enabled)
            : this(factory, _ => enabled)
        {
        }

        public TestOutputLogger(TestOutputLoggerFactory factory, Func<LogLevel, bool> filter)
        {
            _factory = factory;
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
                return;

            if (formatter == null)
                throw new ArgumentNullException(nameof(formatter));

            var message = formatter(state, exception);

            if (string.IsNullOrEmpty(message))
                return;

            message = $"{DateTime.Now:HH:mm:ss.fff}-{logLevel.ToString()[0]} {message}";

            if (exception != null)
                message += Environment.NewLine + Environment.NewLine + exception;

            var currentContext = TestExecutionContext.CurrentContext;

            if (currentContext.CurrentTest is TestMethod)
                _factory.Current = currentContext;

            var executionContext = _factory.Current ??= currentContext;

            executionContext.OutWriter.WriteLine(message);
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
