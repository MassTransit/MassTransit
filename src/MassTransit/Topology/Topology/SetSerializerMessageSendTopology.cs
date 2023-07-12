namespace MassTransit.Topology
{
    using System;
    using System.Net.Mime;
    using Configuration;
    using Middleware;


    public class SetSerializerMessageSendTopology<T> :
        IMessageSendTopology<T>
        where T : class
    {
        readonly IFilter<SendContext<T>> _filter;

        public SetSerializerMessageSendTopology(ContentType contentType)
        {
            if (contentType == null)
                throw new ArgumentNullException(nameof(contentType));

            _filter = new SetSerializerFilter<T>(contentType);
        }

        public void Apply(ITopologyPipeBuilder<SendContext<T>> builder)
        {
            builder.AddFilter(_filter);
        }
    }
}
