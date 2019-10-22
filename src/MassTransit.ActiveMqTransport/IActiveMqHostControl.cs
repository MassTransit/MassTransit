namespace MassTransit.ActiveMqTransport
{
    using System;
    using System.Threading.Tasks;
    using Transports;


    public interface IActiveMqHostControl :
        IBusHostControl,
        IActiveMqHost
    {
        Task<ISendTransport> CreateSendTransport(Uri address);

        Task<ISendTransport> CreatePublishTransport<T>()
            where T : class;
    }
}
