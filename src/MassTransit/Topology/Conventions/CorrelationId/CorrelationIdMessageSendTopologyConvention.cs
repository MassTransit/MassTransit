namespace MassTransit.Topology.Conventions.CorrelationId
{
    using System.Collections.Generic;
    using Context;


    public class CorrelationIdMessageSendTopologyConvention<TMessage> :
        ICorrelationIdMessageSendTopologyConvention<TMessage>
        where TMessage : class
    {
        readonly IList<ICorrelationIdSelector<TMessage>> _selectors;

        public CorrelationIdMessageSendTopologyConvention()
        {
            _selectors = new List<ICorrelationIdSelector<TMessage>>
            {
                new CorrelatedByCorrelationIdSelector<TMessage>(),
                new PropertyCorrelationIdSelector<TMessage>("CorrelationId"),
                new PropertyCorrelationIdSelector<TMessage>("EventId"),
                new PropertyCorrelationIdSelector<TMessage>("CommandId")
            };
        }

        bool IMessageSendTopologyConvention.TryGetMessageSendTopologyConvention<T>(out IMessageSendTopologyConvention<T> convention)
        {
            convention = this as IMessageSendTopologyConvention<T>;

            return convention != null;
        }

        bool IMessageSendTopologyConvention<TMessage>.TryGetMessageSendTopology(out IMessageSendTopology<TMessage> messageSendTopology)
        {
            foreach (ICorrelationIdSelector<TMessage> selector in _selectors)
            {
                if (selector.TryGetSetCorrelationId(out ISetCorrelationId<TMessage> setCorrelationId))
                {
                    messageSendTopology = new SetCorrelationIdMessageSendTopology<TMessage>(setCorrelationId);
                    return true;
                }
            }

            messageSendTopology = null;
            return false;
        }

        public void SetCorrelationId(ISetCorrelationId<TMessage> setCorrelationId)
        {
            _selectors.Insert(0, new SetCorrelationIdSelector<TMessage>(setCorrelationId));
        }
    }
}
