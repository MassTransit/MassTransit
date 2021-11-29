namespace MassTransit.Configuration
{
    public interface ICorrelationIdMessageSendTopologyConvention<TMessage> :
        IMessageSendTopologyConvention<TMessage>
        where TMessage : class
    {
        void SetCorrelationId(IMessageCorrelationId<TMessage> messageCorrelationId);

        /// <summary>
        /// Tries to get the message correlation id
        /// </summary>
        /// <param name="messageCorrelationId"></param>
        /// <returns></returns>
        bool TryGetMessageCorrelationId(out IMessageCorrelationId<TMessage> messageCorrelationId);
    }
}
