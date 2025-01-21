namespace MassTransit.Configuration
{
    using System;
    using System.Collections.Generic;


    public class EndpointSettings<TConsumer> :
        IEndpointSettings<TConsumer>
        where TConsumer : class
    {
        List<Action<IRegistrationContext?, IReceiveEndpointConfigurator>>? _callbacks;

        public EndpointSettings()
        {
            ConfigureConsumeTopology = true;
        }

        public string? Name { get; set; }

        public bool IsTemporary { get; set; }

        public int? PrefetchCount { get; set; }

        public int? ConcurrentMessageLimit { get; set; }

        public bool ConfigureConsumeTopology { get; set; }

        public string? InstanceId { get; set; }

        public void ConfigureEndpoint<T>(T configurator, IRegistrationContext? context)
            where T : IReceiveEndpointConfigurator
        {
            if (_callbacks == null)
                return;

            foreach (Action<IRegistrationContext?, IReceiveEndpointConfigurator> callback in _callbacks)
                callback(context, configurator);
        }

        public void AddConfigureEndpointCallback(Action<IReceiveEndpointConfigurator>? callback)
        {
            if (callback == null)
                return;

            _callbacks ??= new List<Action<IRegistrationContext?, IReceiveEndpointConfigurator>>(1);

            _callbacks.Add((_, cfg) => callback(cfg));
        }

        public void AddConfigureEndpointCallback(Action<IRegistrationContext, IReceiveEndpointConfigurator>? callback)
        {
            if (callback == null)
                return;

            _callbacks ??= new List<Action<IRegistrationContext?, IReceiveEndpointConfigurator>>(1);

            _callbacks.Add((context, cfg) =>
            {
                if (context is null)
                    throw new ConfigurationException("The bus registration context cannot be null (via AddConfigureEndpointCallback).");

                callback(context, cfg);
            });
        }
    }
}
