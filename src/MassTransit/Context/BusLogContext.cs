namespace MassTransit.Context
{
    using System;
    using System.Diagnostics;
    using Logging;
    using Microsoft.Extensions.Logging;


    public class BusLogContext :
        ILogContext
    {
        const string EnableActivityPropagationEnvironmentVariableSettingName = "MASSTRANSIT_ENABLEACTIVITYPROPAGATION";
        const string EnableActivityPropagationAppCtxSettingName = "MassTransit.EnableActivityPropagation";
        readonly bool _enabled;
        readonly ILogger _logger;
        readonly ILoggerFactory _loggerFactory;
        readonly ILogContext _messageLogger;
        readonly DiagnosticListener _source;

        public BusLogContext(ILoggerFactory loggerFactory, DiagnosticListener source)
        {
            _source = source;
            _loggerFactory = loggerFactory;
            _logger = loggerFactory.CreateLogger(LogCategoryName.MassTransit);

            _enabled = GetEnabled();

            _messageLogger = new BusLogContext(source, _enabled, loggerFactory, loggerFactory.CreateLogger("MassTransit.Messages"));
        }

        protected BusLogContext(DiagnosticListener source, bool enabled, ILoggerFactory loggerFactory, ILogContext messageLogger, ILogger logger)
        {
            _source = source;
            _enabled = enabled;
            _loggerFactory = loggerFactory;
            _messageLogger = messageLogger;
            _logger = logger;
        }

        BusLogContext(DiagnosticListener source, bool enabled, ILoggerFactory loggerFactory, ILogger logger)
        {
            _source = source;
            _enabled = enabled;
            _loggerFactory = loggerFactory;
            _logger = logger;

            _messageLogger = this;
        }

        ILogContext ILogContext.Messages => _messageLogger;

        public EnabledDiagnosticSource? IfEnabled(string name)
        {
            return _enabled && (Activity.Current != null || _source.IsEnabled(name))
                ? new EnabledDiagnosticSource(_source, name)
                : default(EnabledDiagnosticSource?);
        }

        public ILogContext<T> CreateLogContext<T>()
        {
            ILogger<T> logger = _loggerFactory.CreateLogger<T>();

            return new BusLogContext<T>(_source, _enabled, _loggerFactory, _messageLogger, logger);
        }

        public ILogContext CreateLogContext(string categoryName)
        {
            var logger = _loggerFactory.CreateLogger(categoryName);

            return new BusLogContext(_source, _enabled, _loggerFactory, _messageLogger, logger);
        }

        public ILogger Logger => _logger;

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

        bool GetEnabled()
        {
            if (AppContext.TryGetSwitch(EnableActivityPropagationAppCtxSettingName, out var enableActivityPropagation))
                return enableActivityPropagation;

            var variable = Environment.GetEnvironmentVariable(EnableActivityPropagationEnvironmentVariableSettingName);
            if (variable != null && (variable.Equals("false", StringComparison.OrdinalIgnoreCase) || variable.Equals("0")))
                return false;

            return true;
        }
    }


    public class BusLogContext<T> :
        BusLogContext,
        ILogContext<T>
    {
        public BusLogContext(DiagnosticListener source, bool enabled, ILoggerFactory loggerFactory, ILogContext messageLogger, ILogger<T> logger)
            : base(source, enabled, loggerFactory, messageLogger, logger)
        {
        }
    }
}
