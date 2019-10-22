namespace MassTransit.Logging
{
    using System;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Internal;


    public readonly struct EnabledLogger
    {
        readonly ILogger _logger;
        readonly LogLevel _level;

        public EnabledLogger(ILogger logger, LogLevel level)
        {
            _logger = logger;
            _level = level;
        }

        public void Log(string message, params object[] args)
        {
            _logger.Log<object>(_level, 0, new FormattedLogValues(message, args), null, MessageFormatter);
        }

        public void Log(Exception exception, string message, params object[] args)
        {
            _logger.Log<object>(_level, 0, new FormattedLogValues(message, args), exception, MessageFormatter);
        }

        static string MessageFormatter(object state, Exception exception)
        {
            return state.ToString();
        }
    }
}
