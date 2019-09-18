namespace MassTransit
{
    using System;
    using System.Threading.Tasks;
    using Transports;


    public interface ISendTransportProvider
    {
        Task<ISendTransport> GetSendTransport(Uri address);
    }
}
