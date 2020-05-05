namespace MassTransit.Topology
{
    public interface IGlobalTopology
    {
        ISendTopologyConfigurator Send { get; }

        IPublishTopologyConfigurator Publish { get; }

        /// <summary>
        /// This must be called early, methinks
        /// </summary>
        void SeparatePublishFromSend();
    }
}
