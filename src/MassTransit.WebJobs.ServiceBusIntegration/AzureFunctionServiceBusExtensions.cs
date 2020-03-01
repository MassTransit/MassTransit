namespace MassTransit.WebJobs.ServiceBusIntegration
{
    using System;
    using System.Threading;
    using Azure.ServiceBus.Core;
    using Azure.ServiceBus.Core.Configuration;
    using Azure.ServiceBus.Core.Transport;
    using Configuration;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Extensions.Logging;


    public static class AzureFunctionServiceBusExtensions
    {
        /// <summary>
        /// Create a message dispatcher using the specified <see cref="IBinder"/>.
        /// </summary>
        /// <param name="selector">Use of the <see cref="IBinder"/> extension method is preferred</param>
        /// <param name="binder"></param>
        /// <param name="logger">The function logger</param>
        /// <param name="configure"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        [Obsolete("Use the IBinder extension method instead")]
        public static IBrokeredMessageReceiver CreateBrokeredMessageReceiver(this IBusFactorySelector selector, IBinder binder, ILogger logger,
            Action<IWebJobReceiverConfigurator> configure)
        {
            return CreateBrokeredMessageReceiver(binder, logger, CancellationToken.None, configure);
        }

        /// <summary>
        /// Create a message dispatcher using the specified <see cref="IBinder"/>.
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="logger">The function logger</param>
        /// <param name="cancellationToken"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IBrokeredMessageReceiver CreateBrokeredMessageReceiver(this IBinder binder, ILogger logger, CancellationToken cancellationToken,
            Action<IWebJobReceiverConfigurator> configure)
        {
            if (binder == null)
                throw new ArgumentNullException(nameof(binder));
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));
            if (configure == null)
                throw new ArgumentNullException(nameof(configure));

            var topologyConfiguration = new ServiceBusTopologyConfiguration(AzureBusFactory.MessageTopology);
            IServiceBusBusConfiguration busConfiguration = new ServiceBusBusConfiguration(topologyConfiguration);

            var receiveEndpointConfiguration = busConfiguration.HostConfiguration.CreateReceiveEndpointConfiguration("unspecified");

            var configurator = new WebJobBrokeredMessageReceiverSpecification(binder, logger, receiveEndpointConfiguration, cancellationToken);

            configure(configurator);

            return configurator.Build();
        }
    }
}
