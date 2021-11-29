namespace MassTransit.AmazonSqsTransport
{
    using Topology;


    public interface SendSettings :
        EntitySettings
    {
        /// <summary>
        /// Return the BrokerTopology to apply at startup (to create exchange and queue if binding is specified)
        /// </summary>
        /// <returns></returns>
        BrokerTopology GetBrokerTopology();
    }
}
