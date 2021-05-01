namespace MassTransit
{
    using System;
    using GreenPipes;


    public interface IServiceInstanceConfigurator<out TEndpointConfigurator> :
        IReceiveConfigurator<TEndpointConfigurator>,
        IOptionsSet
        where TEndpointConfigurator : IReceiveEndpointConfigurator
    {
        IEndpointNameFormatter EndpointNameFormatter { get; }

        /// <summary>
        /// If the InstanceEndpoint is enabled, the address of the instance endpoint
        /// </summary>
        Uri InstanceAddress { get; }

        IBusFactoryConfigurator<TEndpointConfigurator> BusConfigurator { get; }
        TEndpointConfigurator InstanceEndpointConfigurator { get; }

        /// <summary>
        /// Add a specification for validation
        /// </summary>
        /// <param name="specification"></param>
        void AddSpecification(ISpecification specification);
    }
}
