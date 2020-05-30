namespace MassTransit.EventHubIntegration.Configuration
{
    using Azure.Core;


    public interface IHostSettings
    {
        /// <summary>
        /// The connection string to use for connecting to the Event Hubs namespace; it is expected that the shared key properties are contained in this connection string, but not the Event
        /// Hub name.
        /// </summary>
        string ConnectionString { get; }

        /// <summary>
        /// The fully qualified Event Hubs namespace to connect to.  This is likely to be similar to <c>{yournamespace}.servicebus.windows.net</c>.
        /// </summary>
        /// <returns></returns>
        string FullyQualifiedNamespace { get; }

        /// <summary>
        /// >The Azure identity credential to use for authorization.  Access controls may be specified by the Event Hubs namespace or the requested Event Hub, depending on Azure
        /// configuration.
        /// </summary>
        TokenCredential TokenCredential { get; }
    }
}
