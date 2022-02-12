namespace MassTransit.Transports
{
    using System;
    using System.Threading.Tasks;


    public interface IPublishTransportProvider
    {
        Task<ISendTransport> GetPublishTransport<T>(Uri? publishAddress)
            where T : class;
    }
}
