namespace MassTransit.Transports.InMemory
{
    using System;
    using System.Threading.Tasks;
    using Topology;


    public class InMemoryPublishTransportProvider :
        IPublishTransportProvider
    {
        readonly IInMemoryHostControl _host;
        readonly IInMemoryPublishTopology _publishTopology;
        readonly ISendTransportProvider _transportProvider;

        public InMemoryPublishTransportProvider(ISendTransportProvider transportProvider, IInMemoryPublishTopology publishTopology)
        {
            _transportProvider = transportProvider;
            _publishTopology = publishTopology;
            _host = transportProvider as IInMemoryHostControl
                ?? throw new ArgumentException("The transport provider must be an InMemoryHost", nameof(transportProvider));
        }

        public Task<ISendTransport> GetPublishTransport<T>(Uri publishAddress)
            where T : class
        {
            var publishTopology = _publishTopology.GetMessageTopology<T>();

            ApplyTopologyToMessageFabric(publishTopology);

            return _transportProvider.GetSendTransport(publishAddress);
        }

        void ApplyTopologyToMessageFabric<T>(IInMemoryMessagePublishTopology<T> publishTopology)
            where T : class
        {
            var builder = _host.CreatePublishTopologyBuilder();

            publishTopology.Apply(builder);
        }
    }
}
