namespace MassTransit.DependencyInjection.Registration
{
    using System;
    using Configuration;
    using Microsoft.Extensions.DependencyInjection;


    public class EndpointRegistration<T> :
        IEndpointRegistration
        where T : class
    {
        readonly IRegistration _registration;

        public EndpointRegistration(IRegistration registration)
        {
            _registration = registration;
        }

        public Type Type => typeof(T);

        public bool IncludeInConfigureEndpoints
        {
            get => _registration.IncludeInConfigureEndpoints;
            set { }
        }

        public IEndpointDefinition GetDefinition(IServiceProvider provider)
        {
            return provider.GetRequiredService<IEndpointDefinition<T>>();
        }
    }
}
