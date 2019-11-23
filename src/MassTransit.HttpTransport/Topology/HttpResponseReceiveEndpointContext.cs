namespace MassTransit.HttpTransport.Topology
{
    using System;
    using System.Threading.Tasks;
    using ConsumePipeSpecifications;
    using Context;
    using GreenPipes;
    using MassTransit.Pipeline;
    using MassTransit.Pipeline.Observables;
    using MassTransit.Topology;
    using Microsoft.AspNetCore.Http;
    using Transport;
    using Transports;


    public class HttpResponseReceiveEndpointContext :
        ProxyPipeContext,
        ReceiveEndpointContext
    {
        readonly HttpContext _httpContext;
        readonly ReceiveEndpointContext _context;
        readonly Lazy<ISendEndpointProvider> _sendEndpointProvider;

        public HttpResponseReceiveEndpointContext(ReceiveEndpointContext context, HttpContext httpContext)
            : base(context)
        {
            _context = context;
            _httpContext = httpContext;

            _sendEndpointProvider = new Lazy<ISendEndpointProvider>(CreateSendEndpointProvider);
        }

        Uri ReceiveEndpointContext.InputAddress => _context.InputAddress;
        ReceiveObservable ReceiveEndpointContext.ReceiveObservers => _context.ReceiveObservers;
        IReceiveTransportObserver ReceiveEndpointContext.TransportObservers => _context.TransportObservers;
        public IConsumePipeSpecification ConsumePipeSpecification => _context.ConsumePipeSpecification;

        public ILogContext LogContext => _context.LogContext;
        public IReceiveEndpointObserver EndpointObservers => _context.EndpointObservers;

        IPublishTopology ReceiveEndpointContext.Publish => _context.Publish;
        IReceivePipe ReceiveEndpointContext.ReceivePipe => _context.ReceivePipe;
        public ISendPipe SendPipe => _context.SendPipe;

        public IMessageSerializer Serializer => _context.Serializer;

        ISendEndpointProvider ReceiveEndpointContext.SendEndpointProvider => _sendEndpointProvider.Value;
        public Task Dependencies => _context.Dependencies;

        IPublishEndpointProvider ReceiveEndpointContext.PublishEndpointProvider => _context.PublishEndpointProvider;

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return _context.ConnectSendObserver(observer);
        }

        public ConnectHandle ConnectPublishObserver(IPublishObserver observer)
        {
            return _context.ConnectPublishObserver(observer);
        }

        ISendEndpointProvider CreateSendEndpointProvider()
        {
            return new HttpResponseSendEndpointProvider(_httpContext, _context);
        }

        ConnectHandle IReceiveTransportObserverConnector.ConnectReceiveTransportObserver(IReceiveTransportObserver observer)
        {
            return _context.ConnectReceiveTransportObserver(observer);
        }

        ConnectHandle IReceiveObserverConnector.ConnectReceiveObserver(IReceiveObserver observer)
        {
            return _context.ConnectReceiveObserver(observer);
        }

        ConnectHandle IReceiveEndpointObserverConnector.ConnectReceiveEndpointObserver(IReceiveEndpointObserver observer)
        {
            return _context.ConnectReceiveEndpointObserver(observer);
        }
    }
}
