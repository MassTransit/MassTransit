namespace MassTransit.Transports
{
    using System;
    using System.Threading.Tasks;


    public interface ISendTransportProvider
    {
        Task<ISendTransport> GetSendTransport(Uri address);

        Uri NormalizeAddress(Uri address);
    }
}
