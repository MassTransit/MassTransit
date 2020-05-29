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
        readonly ILoggerFactory _loggerFactory;
        readonly ILogContext _messageLogger;
        readonly DiagnosticListener _source;

        public BusLogContext(ILoggerFactory loggerFactory, DiagnosticListener source)
        {
            _source = source;
            _loggerFactory = loggerFactory;
            Logger = loggerFactory.CreateLogger(LogCategoryName.MassTransit);

            _enabled = GetEnabled();

            _messageLogger = new BusLogContext(source, _enabled, loggerFactory, loggerFactory.CreateLogger("MassTransit.Messages"));
        }

        protected BusLogContext(DiagnosticListener source, bool enabled, ILoggerFactory loggerFactory, ILogContext messageLogger, ILogger logger)
        {
            _source = source;
            _enabled = enabled;
            _loggerFactory = loggerFactory;
            _messageLogger = messageLogger;
            Logger = logger;
        }

        BusLogContext(DiagnosticListener source, bool enabled, ILoggerFactory loggerFactory, ILogger logger)
        {
            _source = source;
            _enabled = enabled;
            _loggerFactory = loggerFactory;
            Logger = logger;

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

        public ILogger Logger { get; }

        public EnabledLogger? IfEnabled(LogLevel level)
        {
            return Logger.IsEnabled(level) ? new EnabledLogger(Logger, level) : default(EnabledLogger?);
        }

        public EnabledLogger? Critical => Logger.IsEnabled(LogLevel.Critical) ? new EnabledLogger(Logger, LogLevel.Critical) : default(EnabledLogger?);

        public EnabledLogger? Debug => Logger.IsEnabled(LogLevel.Debug) ? new EnabledLogger(Logger, LogLevel.Debug) : default(EnabledLogger?);

        public EnabledLogger? Error => Logger.IsEnabled(LogLevel.Error) ? new EnabledLogger(Logger, LogLevel.Error) : default(EnabledLogger?);

        public EnabledLogger? Info => Logger.IsEnabled(LogLevel.Information) ? new EnabledLogger(Logger, LogLevel.Information) : default(EnabledLogger?);

        public EnabledLogger? Trace => Logger.IsEnabled(LogLevel.Trace) ? new EnabledLogger(Logger, LogLevel.Trace) : default(EnabledLogger?);

        public EnabledLogger? Warning => Logger.IsEnabled(LogLevel.Warning) ? new EnabledLogger(Logger, LogLevel.Warning) : default(EnabledLogger?);

        public EnabledScope? BeginScope()
        {
            return new EnabledScope(Logger);
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
