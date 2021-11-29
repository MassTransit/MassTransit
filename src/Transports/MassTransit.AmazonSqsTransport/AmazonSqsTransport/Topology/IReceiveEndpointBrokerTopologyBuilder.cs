namespace MassTransit.AmazonSqsTransport.Topology
{
    /// <summary>
    /// A unique builder context should be created for each specification, so that the items added
    /// by it can be combined together into a group - so that if a subsequent specification yanks
    /// something that conflicts, the system can yank the group or warn that it's impacted.
    /// </summary>
    public interface IReceiveEndpointBrokerTopologyBuilder :
        IBrokerTopologyBuilder
    {
        /// <summary>
        /// A handle to the consuming queue
        /// </summary>
        QueueHandle Queue { get; }
    }
}
