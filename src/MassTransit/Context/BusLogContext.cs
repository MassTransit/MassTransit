namespace MassTransit.Context
{
    using System.Diagnostics;
    using Logging;
    using Microsoft.Extensions.Logging;


    public class BusLogContext :
        ILogContext
    {
        readonly DiagnosticSource _source;
        readonly ILoggerFactory _loggerFactory;
        readonly Microsoft.Extensions.Logging.ILogger _logger;
        readonly ILogContext _messageLogger;

        public BusLogContext(ILoggerFactory loggerFactory, DiagnosticSource source)
        {
            _source = source;
            _loggerFactory = loggerFactory;
            _logger = loggerFactory.CreateLogger(LogCategoryName.MassTransit);

            _messageLogger = new BusLogContext(source, loggerFactory, loggerFactory.CreateLogger("MassTransit.Messages"));
        }

        protected BusLogContext(DiagnosticSource source, ILoggerFactory loggerFactory, ILogContext messageLogger, Microsoft.Extensions.Logging.ILogger logger)
        {
            _source = source;
            _loggerFactory = loggerFactory;
            _messageLogger = messageLogger;
            _logger = logger;
        }

        BusLogContext(DiagnosticSource source, ILoggerFactory loggerFactory, Microsoft.Extensions.Logging.ILogger logger)
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

        public EnabledLogger? IfEnabled(Microsoft.Extensions.Logging.LogLevel level)
        {
            return _logger.IsEnabled(level) ? new EnabledLogger(_logger, level) : default(EnabledLogger?);
        }

        public EnabledLogger? Critical
        {
            get { return _logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Critical) ? new EnabledLogger(_logger, Microsoft.Extensions.Logging.LogLevel.Critical) : default(EnabledLogger?); }
        }

        public EnabledLogger? Debug
        {
            get { return _logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Debug) ? new EnabledLogger(_logger, Microsoft.Extensions.Logging.LogLevel.Debug) : default(EnabledLogger?); }
        }

        public EnabledLogger? Error
        {
            get { return _logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Error) ? new EnabledLogger(_logger, Microsoft.Extensions.Logging.LogLevel.Error) : default(EnabledLogger?); }
        }

        public EnabledLogger? Info
        {
            get { return _logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Information) ? new EnabledLogger(_logger, Microsoft.Extensions.Logging.LogLevel.Information) : default(EnabledLogger?); }
        }

        public EnabledLogger? Trace
        {
            get { return _logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Trace) ? new EnabledLogger(_logger, Microsoft.Extensions.Logging.LogLevel.Trace) : default(EnabledLogger?); }
        }

        public EnabledLogger? Warning
        {
            get { return _logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Warning) ? new EnabledLogger(_logger, Microsoft.Extensions.Logging.LogLevel.Warning) : default(EnabledLogger?); }
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
