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

        public HttpResponseSendEndpointProvider(ISendEndpointProvider wrapped, 
            IOwinContext owinContext)
        {
            _wrapped = wrapped;
            _owinContext = owinContext;

            _sendObservable = new SendObservable();
        }

        public Task<ISendEndpoint> GetSendEndpoint(Uri address)
        {
            if (address.Scheme == "reply")
            {
                var responseTransport = new HttpResponseTransport(_owinContext);

                IMessageSerializer serializer = null;
                Uri inputAddress = null;
                ISendPipe sendPipe = null;
                var ep  = new SendEndpoint(responseTransport, serializer, address, inputAddress, sendPipe);
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