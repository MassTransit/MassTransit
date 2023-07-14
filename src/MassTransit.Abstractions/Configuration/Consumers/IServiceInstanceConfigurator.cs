namespace MassTransit
{
    using System;
    using Configuration;


    public interface IServiceInstanceConfigurator :
        IReceiveConfigurator,
        IOptionsSet
    {
        IEndpointNameFormatter EndpointNameFormatter { get; }

        /// <summary>
        /// If the InstanceEndpoint is enabled, the address of the instance endpoint
        /// </summary>
        Uri InstanceAddress { get; }

        IReceiveConfigurator BusConfigurator { get; }
        IReceiveEndpointConfigurator InstanceEndpointConfigurator { get; }

        /// <summary>
        /// Add a specification for validation
        /// </summary>
        /// <param name="specification"></param>
        void AddSpecification(ISpecification specification);
    }


    public interface IServiceInstanceConfigurator<out TEndpointConfigurator> :
        IServiceInstanceConfigurator,
        IReceiveConfigurator<TEndpointConfigurator>
        where TEndpointConfigurator : IReceiveEndpointConfigurator
    {
        new IReceiveConfigurator<TEndpointConfigurator> BusConfigurator { get; }
        new TEndpointConfigurator InstanceEndpointConfigurator { get; }
    }
}
