namespace MassTransit.Configuration
{
    using System;


    public interface IEndpointRegistration :
        IRegistration
    {
        IEndpointDefinition GetDefinition(IServiceProvider provider);
    }
}
