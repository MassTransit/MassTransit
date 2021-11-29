namespace MassTransit.DependencyInjection.Registration
{
    using System;
    using Configuration;
    using Microsoft.Extensions.DependencyInjection;


    public class EndpointRegistration<T> :
        IEndpointRegistration
        where T : class
    {
        public Type Type => typeof(T);

        public IEndpointDefinition GetDefinition(IServiceProvider provider)
        {
            return provider.GetRequiredService<IEndpointDefinition<T>>();
        }
    }
}
