#nullable enable
namespace MassTransit
{
    using System;


    public interface IInMemoryBusFactoryConfigurator :
        IBusFactoryConfigurator<IInMemoryReceiveEndpointConfigurator>
    {
        new IInMemoryPublishTopologyConfigurator PublishTopology { get; }

        /// <summary>
        /// Configure the send topology of the message type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configureTopology"></param>
        void Publish<T>(Action<IInMemoryMessagePublishTopologyConfigurator<T>>? configureTopology = null)
            where T : class;

        void Publish(Type messageType, Action<IInMemoryMessagePublishTopologyConfigurator>? configure = null);

        /// <summary>
        /// Configure the base address for the host
        /// </summary>
        /// <param name="configure"></param>
        /// <returns></returns>
        void Host(Action<IInMemoryHostConfigurator>? configure = null);

        /// <summary>
        /// Configure the base address for the host
        /// </summary>
        /// <param name="baseAddress">The base address for the in-memory host</param>
        /// <param name="configure"></param>
        /// <returns></returns>
        void Host(Uri baseAddress, Action<IInMemoryHostConfigurator>? configure = null);

        /// <summary>
        /// Configure the virtual host, to differentiate in-memory bus instances
        /// </summary>
        /// <param name="virtualHost">The virtual host path</param>
        /// <param name="configure"></param>
        /// <returns></returns>
        void Host(string virtualHost, Action<IInMemoryHostConfigurator>? configure = null);
    }
}
