namespace MassTransit
{
    using System;
    using ExtensionsLoggingIntegration;
    using Microsoft.Extensions.Logging;


    public static class ExtensionsLoggingConfiguratorExtensions
    {
        /// <summary> Configure the Mass Transit Service Bus to use Microsoft.Extensions.Logging. </summary>
        /// <param name="configurator">The configurator to act on. </param>
        /// <param name="loggerFactory">The ILoggerFactory. If none supplied, will be created.</param>
        public static ILoggerFactory UseExtensionsLogging(this IBusFactoryConfigurator configurator, ILoggerFactory loggerFactory)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            if (loggerFactory == null)
                throw new ArgumentNullException(nameof(loggerFactory));

            return ExtensionsLogger.Use(loggerFactory);
        }
    }
}
