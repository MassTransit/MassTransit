namespace MassTransit.RabbitMqTransport.Transport
{
    using System;
    using System.Threading.Tasks;
    using Integration;
    using Transports;


    public class RabbitMqPublishTransportProvider :
        IPublishTransportProvider
    {
        readonly IRabbitMqHostControl _host;
        readonly IModelContextSupervisor _modelContextSupervisor;

        public RabbitMqPublishTransportProvider(IRabbitMqHostControl host, IModelContextSupervisor modelContextSupervisor)
        {
            _host = host;
            _modelContextSupervisor = modelContextSupervisor;
        }

        public Task<ISendTransport> GetPublishTransport<T>(Uri publishAddress)
            where T : class
        {
            return _host.CreatePublishTransport<T>(_modelContextSupervisor);
        }
    }
}
