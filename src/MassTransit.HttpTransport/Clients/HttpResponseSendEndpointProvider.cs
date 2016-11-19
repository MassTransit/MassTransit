namespace MassTransit.HttpTransport.Clients
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes;
    using MassTransit.Pipeline;
    using Microsoft.Owin;
    using Transports;


    public class HttpResponseSendEndpointProvider :
        ISendEndpointProvider
    {
        readonly ISendEndpointProvider _wrapped;
        readonly SendObservable _sendObservable;
        readonly IOwinContext _owinContext;
        readonly IMessageSerializer _messageSerializer;
        readonly Uri _inputAddress;
        readonly ISendPipe _sendPipe;

        public HttpResponseSendEndpointProvider(ISendEndpointProvider wrapped, 
            IOwinContext owinContext,
            IMessageSerializer messageSerializer,
            Uri inputAddress,
            ISendPipe sendPipe)
        {
            _wrapped = wrapped;
            _owinContext = owinContext;
            _messageSerializer = messageSerializer;
            _inputAddress = inputAddress;
            _sendPipe = sendPipe;

            _sendObservable = new SendObservable();
        }

        public Task<ISendEndpoint> GetSendEndpoint(Uri address)
        {
            if (address.Scheme == "reply")
            {
                var responseTransport = new HttpResponseTransport(_owinContext);

                var ep  = new SendEndpoint(responseTransport, _messageSerializer, address, _inputAddress, _sendPipe);
                return Task.FromResult<ISendEndpoint>(ep);
            }

            return _wrapped.GetSendEndpoint(address);
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return _sendObservable.Connect(observer);
        }
    }
}