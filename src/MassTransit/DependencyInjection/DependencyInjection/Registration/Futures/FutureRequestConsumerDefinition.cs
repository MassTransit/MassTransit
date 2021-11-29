namespace MassTransit.DependencyInjection.Registration
{
    using System;


    public class FutureRequestConsumerDefinition<TConsumer, TRequest> :
        ConsumerDefinition<TConsumer>,
        IFutureRequestDefinition<TRequest>
        where TRequest : class
        where TConsumer : class, IConsumer<TRequest>
    {
        Lazy<Uri> _requestAddress;

        public Uri RequestAddress =>
            _requestAddress?.Value ??
            throw new ConfigurationException($"The future consumer definition was not configured: {TypeCache<TConsumer>.ShortName}");

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<TConsumer> consumerConfigurator)
        {
            endpointConfigurator.ConfigureConsumeTopology = false;

            _requestAddress = new Lazy<Uri>(() => endpointConfigurator.InputAddress);

            base.ConfigureConsumer(endpointConfigurator, consumerConfigurator);
        }
    }
}
