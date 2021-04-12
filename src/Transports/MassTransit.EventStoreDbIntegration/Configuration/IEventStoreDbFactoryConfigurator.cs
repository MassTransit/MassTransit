using System;
using EventStore.Client;
using MassTransit.Riders;

namespace MassTransit.EventStoreDbIntegration
{
    public interface IEventStoreDbFactoryConfigurator :
        IRiderFactoryConfigurator,
        ISendObserverConnector,
        ISendPipelineConfigurator
    {
        /// <summary>
        /// Configure EventStoreDB using connection string and connection name.  Will use the existing EventStoreClient
        /// if one is already registered.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="connectionName">The name of the connection.</param>
        void Host(string connectionString, string connectionName);

        /// <summary>
        /// Configure EventStoreDB using connection string, connection name and default credentials.  Will use the
        /// existing EventStoreClient if one is already registered.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="connectionName">The name of the connection.</param>
        /// <param name="defaultCredentials">
        /// The optional EventStore.Client.UserCredentials to use if none have been supplied to the operation.
        /// </param>
        void Host(string connectionString, string connectionName, UserCredentials defaultCredentials);

        /// <summary>
        /// Subscribe to an EventStoreDB stream.
        /// </summary>
        /// <param name="streamCategory">The stream category to subscribe to.</param>
        /// <param name="subscriptionName">Subscription name.</param>
        /// <param name="configure"></param>
        void ReceiveEndpoint(StreamCategory streamCategory, string subscriptionName, Action<IEventStoreDbReceiveEndpointConfigurator> configure);

        /// <summary>
        /// Sets the outbound message serializer
        /// </summary>
        /// <param name="serializerFactory">The factory to create the message serializer</param>
        void SetMessageSerializer(SerializerFactory serializerFactory);
    }
}
