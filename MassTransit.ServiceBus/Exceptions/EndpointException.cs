using System;

namespace MassTransit.ServiceBus.Exceptions
{
    public class EndpointException :
        Exception
    {
        private IEndpoint _endpoint;

        public EndpointException(IEndpoint endpoint, string message)
            : base(message)
        {
            _endpoint = endpoint;
        }

        public EndpointException(IEndpoint endpoint, string message, Exception innerException)
            : base(message, innerException)
        {
            _endpoint = endpoint;
        }

        public IEndpoint Endpoint
        {
            get { return _endpoint; }
            set { _endpoint = value; }
        }
    }
}