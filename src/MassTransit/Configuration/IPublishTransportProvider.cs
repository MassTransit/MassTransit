namespace MassTransit
{
    using System;
    using System.Threading.Tasks;
    using Transports;


    public interface IPublishTransportProvider
    {
        Task<ISendTransport> GetPublishTransport<T>(Uri publishAddress)
            where T : class;
    }
}
