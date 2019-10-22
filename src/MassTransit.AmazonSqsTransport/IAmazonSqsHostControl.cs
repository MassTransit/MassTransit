namespace MassTransit.AmazonSqsTransport
{
    using System;
    using System.Threading.Tasks;
    using Transports;


    public interface IAmazonSqsHostControl :
        IBusHostControl,
        IAmazonSqsHost
    {
        Task<ISendTransport> CreateSendTransport(Uri address);

        Task<ISendTransport> CreatePublishTransport<T>()
            where T : class;
    }
}
