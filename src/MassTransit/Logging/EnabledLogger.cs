#nullable enable
namespace MassTransit.Logging
{
    using System;
    using Microsoft.Extensions.Logging;


    public readonly struct EnabledLogger
    {
        readonly ILogger _logger;
        readonly LogLevel _level;

        public EnabledLogger(ILogger logger, LogLevel level)
        {
            _logger = logger;
            _level = level;
        }

        public void Log(string message, params object?[] args)
        {
            _logger.Log(_level, message, args);
        }

        public void Log(Exception exception, string message, params object?[] args)
        {
            _logger.Log(_level, exception, message, args);
        }
    }
}
