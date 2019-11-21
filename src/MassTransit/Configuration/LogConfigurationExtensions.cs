namespace MassTransit
{
    using System;
    using Context;
    using Microsoft.Extensions.Logging;


    public static class LogConfigurationExtensions
    {
        /// <summary>
        /// Configure the built-in <see cref="LogContext"/> to use the specified <see cref="ILoggerFactory"/>. Note that this is actually a global
        /// instance, and is not configured for each bus.
        /// </summary>
        /// <param name="configurator">The bus factory configurator</param>
        /// <param name="loggerFactory">The loggerFactory instance</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        [Obsolete("Still works, but use the SetLoggerFactory method instead, okay?")]
        public static void UseExtensionsLogging(this IBusFactoryConfigurator configurator, ILoggerFactory loggerFactory)
        {
            SetLoggerFactory(configurator, loggerFactory);
        }

        /// <summary>
        /// Configure the built-in <see cref="LogContext"/> to use the specified <see cref="ILoggerFactory"/>. Note that this is actually a global
        /// instance, and is not configured for each bus.
        /// </summary>
        /// <param name="configurator">The bus factory configurator</param>
        /// <param name="loggerFactory">The loggerFactory instance</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static void SetLoggerFactory(this IBusFactoryConfigurator configurator, ILoggerFactory loggerFactory)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));
            if (loggerFactory == null)
                throw new ArgumentNullException(nameof(loggerFactory));

            LogContext.ConfigureCurrentLogContext(loggerFactory);
        }
    }
}
