namespace MassTransit.HttpTransport.Transport
{
    using System;
    using System.Threading.Tasks;
    using Context;
    using Transports;


    public interface IHttpHostControl :
        IHttpHost,
        IBusHostControl
    {
        Task<ISendTransport> CreateSendTransport(Uri address, ReceiveEndpointContext receiveEndpointContext);
    }
}
