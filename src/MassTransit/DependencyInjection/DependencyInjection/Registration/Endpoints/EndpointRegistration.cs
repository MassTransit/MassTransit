namespace MassTransit.DependencyInjection.Registration
{
    using System;
    using Configuration;


    public class EndpointRegistration<T> :
        IEndpointRegistration
        where T : class
    {
        readonly IRegistration _registration;
        readonly IContainerSelector _selector;

        public EndpointRegistration(IRegistration registration, IContainerSelector selector)
        {
            _registration = registration;
            _selector = selector;
        }

        public Type Type => typeof(T);

        public bool IncludeInConfigureEndpoints
        {
            get => _registration.IncludeInConfigureEndpoints;
            set { }
        }

        public IEndpointDefinition GetDefinition(IServiceProvider provider)
        {
            return _selector.GetEndpointDefinition<T>(provider)
                ?? throw new ConfigurationException($"Endpoint definition not found: {TypeCache<T>.ShortName}");
        }
    }
}
