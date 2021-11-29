namespace MassTransit
{
    using System.Collections.Generic;


    public interface IActiveMqHostConfigurator
    {
        /// <summary>
        /// Sets the username for the connection to ActiveMQ
        /// </summary>
        /// <param name="username"></param>
        void Username(string username);

        /// <summary>
        /// Sets the password for the connection to ActiveMQ
        /// </summary>
        /// <param name="password"></param>
        void Password(string password);

        void UseSsl();

        /// <summary>
        /// Sets a list of hosts to enable the failover transport
        /// </summary>
        /// <param name="hosts"></param>
        void FailoverHosts(string[] hosts);

        /// <summary>
        /// Sets options on the underlying NMS transport
        /// </summary>
        /// <param name="options"></param>
        void TransportOptions(IEnumerable<KeyValuePair<string, string>> options);

        /// <summary>
        /// </summary>
        void EnableOptimizeAcknowledge();

        void SetPrefetchPolicy(int limit);

        void SetQueuePrefetchPolicy(int limit);
    }
}
