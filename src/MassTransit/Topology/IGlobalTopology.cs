namespace MassTransit
{
    using System;


    public interface IGlobalTopology
    {
        ISendTopologyConfigurator Send { get; }

        IPublishTopologyConfigurator Publish { get; }

        /// <summary>
        /// This must be called early, methinks
        /// </summary>
        void SeparatePublishFromSend();

        void MarkMessageTypeNotConsumable(Type type);

        bool IsConsumableMessageType(Type type);
    }
}
