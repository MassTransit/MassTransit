namespace MassTransit.ExtensionsLoggingIntegration
{
    using System;
    using Logging;
    using Microsoft.Extensions.Logging;


    public class ExtensionsLogger :
        Logging.ILogger
    {
        readonly ILoggerFactory _loggerFactory;

        public ExtensionsLogger(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }

        public ILog Get(string name)
        {
            return new ExtensionsLog(_loggerFactory.CreateLogger(name));
        }

        public void Shutdown()
        {
            _loggerFactory.Dispose();
        }

        /// <summary>
        /// Use Microsoft.Extensions.Logging for logging in MassTransit. If no LoggerFactory is provided,
        /// one will be created.
        /// </summary>
        /// <param name="loggerFactory">The (optional) logger factory.</param>
        public static ILoggerFactory Use(ILoggerFactory loggerFactory)
        {
            if (loggerFactory == null)
                throw new ArgumentNullException(nameof(loggerFactory));

            Logger.UseLogger(new ExtensionsLogger(loggerFactory));
            return loggerFactory;
        }
    }
}
