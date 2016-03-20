namespace MassTransit.HttpTransport.Configuration.Builders
{
    using System.Collections.Generic;
    using System.Linq;
    using MassTransit.Pipeline;


    public class HttpReceiveEndpointBuilder :
        IHttpReceiveEndpointBuilder
    {
        readonly IConsumePipe _consumePipe;

        public HttpReceiveEndpointBuilder(IConsumePipe consumePipe)
        {
            _consumePipe = consumePipe;
        }

        public ConnectHandle ConnectConsumePipe<T>(IPipe<ConsumeContext<T>> pipe) where T : class
        {
            return _consumePipe.ConnectConsumePipe(pipe);
        }

        public ConnectHandle ConnectConsumeMessageObserver<T>(IConsumeMessageObserver<T> observer) where T : class
        {
            return _consumePipe.ConnectConsumeMessageObserver(observer);
        }

        public IEnumerable<object> GetHttpRouteBindings()
        {
            return Enumerable.Empty<object>();
        }
    }
}