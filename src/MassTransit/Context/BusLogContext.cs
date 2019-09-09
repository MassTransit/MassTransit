namespace MassTransit.Context
{
    using System.Diagnostics;
    using Logging;
    using Microsoft.Extensions.Logging;


    public class BusLogContext :
        ILogContext
    {
        readonly ILogger _logger;
        readonly ILoggerFactory _loggerFactory;
        readonly ILogContext _messageLogger;
        readonly DiagnosticSource _source;

        public BusLogContext(ILoggerFactory loggerFactory, DiagnosticSource source)
        {
            _source = source;
            _loggerFactory = loggerFactory;
            _logger = loggerFactory.CreateLogger(LogCategoryName.MassTransit);

            _messageLogger = new BusLogContext(source, loggerFactory, loggerFactory.CreateLogger("MassTransit.Messages"));
        }

        protected BusLogContext(DiagnosticSource source, ILoggerFactory loggerFactory, ILogContext messageLogger, ILogger logger)
        {
            _source = source;
            _loggerFactory = loggerFactory;
            _messageLogger = messageLogger;
            _logger = logger;
        }

        BusLogContext(DiagnosticSource source, ILoggerFactory loggerFactory, ILogger logger)
        {
            _source = source;
            _loggerFactory = loggerFactory;
            _logger = logger;

            _messageLogger = this;
        }

        ILogContext ILogContext.Messages => _messageLogger;

        public EnabledDiagnosticSource? IfEnabled(string name)
        {
            return _source.IsEnabled(name) ? new EnabledDiagnosticSource(_source, name) : default(EnabledDiagnosticSource?);
        }

        public ILogContext<T> CreateLogContext<T>()
        {
            ILogger<T> logger = _loggerFactory.CreateLogger<T>();

            return new BusLogContext<T>(_source, _loggerFactory, _messageLogger, logger);
        }

        public ILogContext CreateLogContext(string categoryName)
        {
            var logger = _loggerFactory.CreateLogger(categoryName);

            return new BusLogContext(_source, _loggerFactory, _messageLogger, logger);
        }

        public EnabledLogger? IfEnabled(LogLevel level)
        {
            return _logger.IsEnabled(level) ? new EnabledLogger(_logger, level) : default(EnabledLogger?);
        }

        public EnabledLogger? Critical => _logger.IsEnabled(LogLevel.Critical) ? new EnabledLogger(_logger, LogLevel.Critical) : default(EnabledLogger?);

        public EnabledLogger? Debug => _logger.IsEnabled(LogLevel.Debug) ? new EnabledLogger(_logger, LogLevel.Debug) : default(EnabledLogger?);

        public EnabledLogger? Error => _logger.IsEnabled(LogLevel.Error) ? new EnabledLogger(_logger, LogLevel.Error) : default(EnabledLogger?);

        public EnabledLogger? Info => _logger.IsEnabled(LogLevel.Information) ? new EnabledLogger(_logger, LogLevel.Information) : default(EnabledLogger?);

        public EnabledLogger? Trace => _logger.IsEnabled(LogLevel.Trace) ? new EnabledLogger(_logger, LogLevel.Trace) : default(EnabledLogger?);

        public EnabledLogger? Warning => _logger.IsEnabled(LogLevel.Warning) ? new EnabledLogger(_logger, LogLevel.Warning) : default(EnabledLogger?);

        public EnabledScope? BeginScope()
        {
            return new EnabledScope(_logger);
        }
    }


    public class BusLogContext<T> :
        BusLogContext,
        ILogContext<T>
    {
        public BusLogContext(DiagnosticSource source, ILoggerFactory loggerFactory, ILogContext messageLogger, ILogger<T> logger)
            : base(source, loggerFactory, messageLogger, logger)
        {
        }
    }
}
