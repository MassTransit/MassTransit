namespace MassTransit.Logging
{
    using System;
    using Microsoft.Extensions.Logging;


    public static class LoggerExtensions
    {
        public static void UseLoggerFactory(this IBusFactoryConfigurator configurator, ILoggerFactory loggerFactory)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            Logger.UseLoggerFactory(loggerFactory);
        }
    }
}
