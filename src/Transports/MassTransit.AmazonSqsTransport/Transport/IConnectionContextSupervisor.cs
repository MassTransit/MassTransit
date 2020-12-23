namespace MassTransit.AmazonSqsTransport.Transport
{
    using System;
    using System.Threading.Tasks;
    using Transports;


    public interface IConnectionContextSupervisor :
        ITransportSupervisor<ConnectionContext>
    {
        IClientContextSupervisor CreateClientContextSupervisor();

        Uri NormalizeAddress(Uri address);

        Task<ISendTransport> CreateSendTransport(IClientContextSupervisor supervisor, Uri address);

        Task<ISendTransport> CreatePublishTransport<T>(IClientContextSupervisor supervisor)
            where T : class;
    }
}
