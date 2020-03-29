namespace MassTransit.Builders
{
    using Configuration;
    using GreenPipes;


    public class ReceiveEndpointBuilder :
        IReceiveEndpointBuilder
    {
        readonly IReceiveEndpointConfiguration _configuration;

        public ReceiveEndpointBuilder(IReceiveEndpointConfiguration configuration)
        {
            _configuration = configuration;
        }

        public virtual ConnectHandle ConnectConsumePipe<T>(IPipe<ConsumeContext<T>> pipe)
            where T : class
        {
            return _configuration.ConsumePipe.ConnectConsumePipe(pipe);
        }
    }
}
