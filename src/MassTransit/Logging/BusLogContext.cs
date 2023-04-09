#nullable enable
namespace MassTransit.Logging
{
    using Microsoft.Extensions.Logging;


    public class BusLogContext :
        ILogContext
    {
        readonly ILoggerFactory _loggerFactory;
        readonly ILogContext _messageLogger;

        public BusLogContext(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
            Logger = loggerFactory.CreateLogger(LogCategoryName.MassTransit);

            _messageLogger = new BusLogContext(loggerFactory, loggerFactory.CreateLogger("MassTransit.Messages"));
        }

        BusLogContext(ILoggerFactory loggerFactory, ILogContext messageLogger, ILogger logger)
        {
            _loggerFactory = loggerFactory;
            _messageLogger = messageLogger;
            Logger = logger;
        }

        BusLogContext(ILoggerFactory loggerFactory, ILogger logger)
        {
            _loggerFactory = loggerFactory;
            Logger = logger;

            _messageLogger = this;
        }

        ILogContext ILogContext.Messages => _messageLogger;

        public ILogContext CreateLogContext(string categoryName)
        {
            var logger = _loggerFactory.CreateLogger(categoryName);

            return new BusLogContext(_loggerFactory, _messageLogger, logger);
        }

        public ILogger Logger { get; }

        public EnabledLogger? Critical => Logger.IsEnabled(LogLevel.Critical) ? new EnabledLogger(Logger, LogLevel.Critical) : default(EnabledLogger?);

        public EnabledLogger? Debug => Logger.IsEnabled(LogLevel.Debug) ? new EnabledLogger(Logger, LogLevel.Debug) : default(EnabledLogger?);

        public EnabledLogger? Error => Logger.IsEnabled(LogLevel.Error) ? new EnabledLogger(Logger, LogLevel.Error) : default(EnabledLogger?);

        public EnabledLogger? Info => Logger.IsEnabled(LogLevel.Information) ? new EnabledLogger(Logger, LogLevel.Information) : default(EnabledLogger?);

        public EnabledLogger? Trace => Logger.IsEnabled(LogLevel.Trace) ? new EnabledLogger(Logger, LogLevel.Trace) : default(EnabledLogger?);

        public EnabledLogger? Warning => Logger.IsEnabled(LogLevel.Warning) ? new EnabledLogger(Logger, LogLevel.Warning) : default(EnabledLogger?);
    }
}
