namespace MassTransit.Configuration
{
    using System;
    using System.Collections.Generic;


    public class EndpointSettings<T> :
        IEndpointSettings<T>
        where T : class
    {
        List<Action<IReceiveEndpointConfigurator>>? _callbacks;

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

        public void ConfigureEndpoint(IReceiveEndpointConfigurator configurator)
        {
            if (_callbacks != null)
            {
                foreach (Action<IReceiveEndpointConfigurator> callback in _callbacks)
                    callback(configurator);
            }
        }

        public void AddConfigureEndpointCallback(Action<IReceiveEndpointConfigurator>? callback)
        {
            if (callback == null)
                return;

            _callbacks ??= new List<Action<IReceiveEndpointConfigurator>>(1);

            _callbacks.Add(callback);
        }
    }
}
