namespace MassTransit.Topology.Conventions.CorrelationId
{
    using System;
    using Context;
    using Filters;
    using GreenPipes;


    public class SetCorrelationIdMessageSendTopology<T> :
        IMessageSendTopology<T>
        where T : class
    {
        readonly IFilter<SendContext<T>> _filter;

        public SetCorrelationIdMessageSendTopology(ISetCorrelationId<T> setCorrelationId)
        {
            if (setCorrelationId == null)
                throw new ArgumentNullException(nameof(setCorrelationId));

            _filter = new SetCorrelationIdFilter<T>(setCorrelationId);
        }

        public void Apply(ITopologyPipeBuilder<SendContext<T>> builder)
        {
            builder.AddFilter(_filter);
        }
    }
}
