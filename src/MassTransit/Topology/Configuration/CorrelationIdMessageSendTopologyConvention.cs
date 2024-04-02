namespace MassTransit.Configuration
{
    using System.Collections.Generic;
    using Topology;


    public class CorrelationIdMessageSendTopologyConvention<TMessage> :
        ICorrelationIdMessageSendTopologyConvention<TMessage>
        where TMessage : class
    {
        readonly List<ICorrelationIdSelector<TMessage>> _selectors;

        public CorrelationIdMessageSendTopologyConvention()
        {
            _selectors =
            [
                new CorrelatedByCorrelationIdSelector<TMessage>(),
                new PropertyCorrelationIdSelector<TMessage>("CorrelationId"),
                new PropertyCorrelationIdSelector<TMessage>("EventId"),
                new PropertyCorrelationIdSelector<TMessage>("CommandId")
            ];
        }

        bool IMessageSendTopologyConvention.TryGetMessageSendTopologyConvention<T>(out IMessageSendTopologyConvention<T> convention)
        {
            convention = this as IMessageSendTopologyConvention<T>;

            return convention != null;
        }

        bool IMessageSendTopologyConvention<TMessage>.TryGetMessageSendTopology(out IMessageSendTopology<TMessage> messageSendTopology)
        {
            if (TryGetMessageCorrelationId(out IMessageCorrelationId<TMessage> messageCorrelationId))
            {
                messageSendTopology = new SetCorrelationIdMessageSendTopology<TMessage>(messageCorrelationId);
                return true;
            }

            messageSendTopology = null;
            return false;
        }

        public void SetCorrelationId(IMessageCorrelationId<TMessage> messageCorrelationId)
        {
            _selectors.Insert(0, new SetCorrelationIdSelector<TMessage>(messageCorrelationId));
        }

        public bool TryGetMessageCorrelationId(out IMessageCorrelationId<TMessage> messageCorrelationId)
        {
            for (var index = 0; index < _selectors.Count; index++)
            {
                if (_selectors[index].TryGetSetCorrelationId(out messageCorrelationId))
                    return true;
            }

            messageCorrelationId = null;
            return false;
        }
    }
}
