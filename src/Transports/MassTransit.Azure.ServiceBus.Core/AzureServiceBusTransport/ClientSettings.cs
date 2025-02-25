namespace MassTransit.AzureServiceBusTransport
{
    using System;


    public interface ClientSettings
    {
        /// <summary>
        /// The number of concurrent messages to process
        /// </summary>
        int MaxConcurrentCalls { get; }

        /// <summary>
        /// The number of messages to push from the server to the client
        /// </summary>
        int PrefetchCount { get; }

        /// <summary>
        /// The timeout before the session state is renewed
        /// </summary>
        TimeSpan MaxAutoRenewDuration { get; }

        /// <summary>
        /// The timeout before a message session is abandoned
        /// - if unset the SDK will use
        /// <see
        ///     href="https://learn.microsoft.com/en-us/dotnet/api/azure.messaging.servicebus.servicebusretryoptions.trytimeout?view=azure-dotnet#azure-messaging-servicebus-servicebusretryoptions-trytimeout">
        /// ServiceBusRetryOptions.TryTimeout
        /// </see>
        /// </summary>
        TimeSpan? SessionIdleTimeout { get; }

        /// <summary>
        /// The maximum number of concurrent sessions
        /// </summary>
        int MaxConcurrentSessions { get; }

        /// <summary>
        /// The maximum number of concurrent calls per session
        /// </summary>
        int MaxConcurrentCallsPerSession { get; }

        /// <summary>
        /// The path of the message entity
        /// </summary>
        string Path { get; }

        /// <summary>
        /// The name of the message entity
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Get the input address for the client on the specified host
        /// </summary>
        Uri GetInputAddress(Uri serviceUri, string path);
    }
}
