namespace MassTransit.RabbitMqTransport.Transport
{
    using System;
    using System.Threading.Tasks;
    using Integration;
    using Transports;


    public class RabbitMqSendTransportProvider :
        ISendTransportProvider
    {
        readonly IRabbitMqHostControl _host;
        readonly IModelContextSupervisor _modelContextSupervisor;

        public RabbitMqSendTransportProvider(IRabbitMqHostControl host, IModelContextSupervisor modelContextSupervisor)
        {
            _host = host;
            _modelContextSupervisor = modelContextSupervisor;
        }

        public Uri NormalizeAddress(Uri address)
        {
            return new RabbitMqEndpointAddress(_host.Address, address);
        }

        async Task<ISendTransport> ISendTransportProvider.GetSendTransport(Uri address)
        {
            var endpointAddress = new RabbitMqEndpointAddress(_host.Address, address);

            var transport = await _host.CreateSendTransport(endpointAddress, _modelContextSupervisor).ConfigureAwait(false);

            return transport;
        }
    }
}
