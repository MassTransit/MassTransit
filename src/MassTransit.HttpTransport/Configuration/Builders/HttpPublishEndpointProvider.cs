namespace MassTransit.HttpTransport.Configuration.Builders
{
    using System;
    using System.Threading.Tasks;
    using MassTransit.Pipeline;


    public class HttpPublishEndpointProvider : IPublishEndpointProvider
    {
        readonly PublishObservable _publishObservable;

        public HttpPublishEndpointProvider()
        {
            _publishObservable = new PublishObservable();
        }

        public ConnectHandle ConnectPublishObserver(IPublishObserver observer)
        {
            return _publishObservable.Connect(observer);
        }

        public IPublishEndpoint CreatePublishEndpoint(Uri sourceAddress, Guid? correlationId = null, Guid? conversationId = null)
        {
            return null;
        }

        public Task<ISendEndpoint> GetPublishSendEndpoint(Type messageType)
        {
            return null;
        }
    }
}