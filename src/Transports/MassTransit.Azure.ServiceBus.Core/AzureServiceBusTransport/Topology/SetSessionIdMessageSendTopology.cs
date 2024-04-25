namespace MassTransit.AzureServiceBusTransport.Topology
{
    using System;
    using MassTransit.Configuration;
    using Middleware;


    public class SetSessionIdMessageSendTopology<T> :
        IMessageSendTopology<T>
        where T : class
    {
        readonly IFilter<SendContext<T>> _filter;

        public SetSessionIdMessageSendTopology(IMessageSessionIdFormatter<T> sessionIdFormatter)
        {
            if (sessionIdFormatter == null)
                throw new ArgumentNullException(nameof(sessionIdFormatter));

            _filter = new ServiceBusSendContextFilter<T>(new SetSessionIdFilter<T>(sessionIdFormatter));
        }

        public void Apply(ITopologyPipeBuilder<SendContext<T>> builder)
        {
            builder.AddFilter(_filter);
        }
    }
}
