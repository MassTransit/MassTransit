namespace MassTransit.AmazonSqsTransport.Topology
{
    using System;
    using Builders;


    public interface SendSettings :
        EntitySettings
    {
        /// <summary>
        /// Attach context's headers using SQS/SNS Message Attributes.
        /// </summary>
        bool PushContextHeadersOverMessageAttributes { get; }

        /// <summary>
        /// Returns the send address for the settings
        /// </summary>
        /// <param name="hostAddress"></param>
        /// <returns></returns>
        Uri GetSendAddress(Uri hostAddress);

        /// <summary>
        /// Return the BrokerTopology to apply at startup (to create exchange and queue if binding is specified)
        /// </summary>
        /// <returns></returns>
        BrokerTopology GetBrokerTopology();
    }
}
