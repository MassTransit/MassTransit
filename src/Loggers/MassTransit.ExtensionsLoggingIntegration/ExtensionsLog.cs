namespace MassTransit.ExtensionsLoggingIntegration
{
    using System;
    using Microsoft.Extensions.Logging;


    public class ExtensionsLog :
        Logging.ILog
    {
        readonly ILogger _logger;

        public ExtensionsLog(ILogger logger)
        {
            _logger = logger;
        }

        public bool IsDebugEnabled => _logger.IsEnabled(LogLevel.Debug);

        public bool IsErrorEnabled => _logger.IsEnabled(LogLevel.Error);

        public bool IsFatalEnabled => _logger.IsEnabled(LogLevel.Critical);

        public bool IsInfoEnabled => _logger.IsEnabled(LogLevel.Information);

        public bool IsWarnEnabled => _logger.IsEnabled(LogLevel.Warning);

        public void Debug(Logging.LogOutputProvider messageProvider)
        {
            _logger.Log(LogLevel.Debug, 0, messageProvider, null, MessageFormatter);
        }

        public void Debug(object obj)
        {
            _logger.LogDebug(obj?.ToString());
        }

        public void Debug(object obj, Exception exception)
        {
            _logger.LogDebug(0, exception, obj?.ToString());
        }

        public void DebugFormat(string format, params object[] args)
        {
            _logger.LogDebug(format, args);
        }

        public void DebugFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            _logger.LogDebug(string.Format(formatProvider, format, args));
        }

        public void Error(Logging.LogOutputProvider messageProvider)
        {
            _logger.Log(LogLevel.Error, 0, messageProvider, null, MessageFormatter);
        }

        public void Error(object obj)
        {
            _logger.LogError(obj?.ToString());
        }

        public void Error(object obj, Exception exception)
        {
            _logger.LogError(0, exception, obj?.ToString());
        }

        public void ErrorFormat(string format, params object[] args)
        {
            _logger.LogError(format, args);
        }

        public void ErrorFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            _logger.LogError(string.Format(formatProvider, format, args));
        }

        public void Fatal(Logging.LogOutputProvider messageProvider)
        {
            _logger.Log(LogLevel.Critical, 0, messageProvider, null, MessageFormatter);
        }

        public void Fatal(object obj)
        {
            _logger.LogCritical(obj?.ToString());
        }

        public void Fatal(object obj, Exception exception)
        {
            _logger.LogCritical(0, exception, obj?.ToString());
        }

        public void FatalFormat(string format, params object[] args)
        {
            _logger.LogCritical(format, args);
        }

        public void FatalFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            _logger.LogCritical(string.Format(formatProvider, format, args));
        }

        public void Info(Logging.LogOutputProvider messageProvider)
        {
            _logger.Log(LogLevel.Information, 0, messageProvider, null, MessageFormatter);
        }

        public void Info(object obj)
        {
            _logger.LogInformation(obj?.ToString());
        }

        public void Info(object obj, Exception exception)
        {
            _logger.LogInformation(0, exception, obj?.ToString());
        }

        public void InfoFormat(string format, params object[] args)
        {
            _logger.LogInformation(format, args);
        }

        public void InfoFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            _logger.LogInformation(string.Format(formatProvider, format, args));
        }

        public void Warn(Logging.LogOutputProvider messageProvider)
        {
            _logger.Log(LogLevel.Warning, 0, messageProvider, null, MessageFormatter);
        }

        public void Warn(object obj)
        {
            _logger.LogWarning(obj?.ToString());
        }

        public void Warn(object obj, Exception exception)
        {
            _logger.LogWarning(0, exception, obj?.ToString());
        }

        public void WarnFormat(string format, params object[] args)
        {
            _logger.LogWarning(format, args);
        }

        public void WarnFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            _logger.LogWarning(string.Format(formatProvider, format, args));
        }

        public void Log(Logging.LogLevel level, object obj)
        {
            if (level == Logging.LogLevel.Fatal)
                Fatal(obj);
            else if (level == Logging.LogLevel.Error)
                Error(obj);
            else if (level == Logging.LogLevel.Warn)
                Warn(obj);
            else if (level == Logging.LogLevel.Info)
                Info(obj);
            else if (level >= Logging.LogLevel.Debug)
                Debug(obj);
        }

        public void Log(Logging.LogLevel level, object obj, Exception exception)
        {
            if (level == Logging.LogLevel.Fatal)
                Fatal(obj, exception);
            else if (level == Logging.LogLevel.Error)
                Error(obj, exception);
            else if (level == Logging.LogLevel.Warn)
                Warn(obj, exception);
            else if (level == Logging.LogLevel.Info)
                Info(obj, exception);
            else if (level >= Logging.LogLevel.Debug)
                Debug(obj, exception);
        }

        public void Log(Logging.LogLevel level, Logging.LogOutputProvider messageProvider)
        {
            if (level == Logging.LogLevel.Fatal)
                Fatal(messageProvider);
            else if (level == Logging.LogLevel.Error)
                Error(messageProvider);
            else if (level == Logging.LogLevel.Warn)
                Warn(messageProvider);
            else if (level == Logging.LogLevel.Info)
                Info(messageProvider);
            else if (level >= Logging.LogLevel.Debug)
                Debug(messageProvider);
        }

        public void LogFormat(Logging.LogLevel level, string format, params object[] args)
        {
            if (level == Logging.LogLevel.Fatal)
                FatalFormat(format, args);
            else if (level == Logging.LogLevel.Error)
                ErrorFormat(format, args);
            else if (level == Logging.LogLevel.Warn)
                WarnFormat(format, args);
            else if (level == Logging.LogLevel.Info)
                InfoFormat(format, args);
            else if (level >= Logging.LogLevel.Debug)
                DebugFormat(format, args);
        }

        public void LogFormat(Logging.LogLevel level, IFormatProvider formatProvider, string format, params object[] args)
        {
            if (level == Logging.LogLevel.Fatal)
                FatalFormat(formatProvider, format, args);
            else if (level == Logging.LogLevel.Error)
                ErrorFormat(formatProvider, format, args);
            else if (level == Logging.LogLevel.Warn)
                WarnFormat(formatProvider, format, args);
            else if (level == Logging.LogLevel.Info)
                InfoFormat(formatProvider, format, args);
            else if (level >= Logging.LogLevel.Debug)
                DebugFormat(formatProvider, format, args);
        }

        public void LogFormat(LogLevel level, IFormatProvider formatProvider, string format, params object[] args)
        {
            throw new NotImplementedException();
        }

        static string MessageFormatter(Logging.LogOutputProvider output, Exception error)
        {
            return output == null ? null : output()?.ToString();
        }
    }
}
