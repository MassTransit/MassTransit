namespace MassTransit.AmazonSqsTransport.Transport
{
    using System;
    using System.Threading.Tasks;
    using Transports;


    public interface IConnectionContextSupervisor :
        ITransportSupervisor<ConnectionContext>
    {
        Uri NormalizeAddress(Uri address);

        Task<ISendTransport> CreateSendTransport(IClientContextSupervisor clientContextSupervisor, Uri address);

        Task<ISendTransport> CreatePublishTransport<T>(IClientContextSupervisor clientContextSupervisor)
            where T : class;
    }
}
