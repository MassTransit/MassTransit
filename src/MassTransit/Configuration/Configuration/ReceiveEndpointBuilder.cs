namespace MassTransit.Configuration
{
    public class ReceiveEndpointBuilder :
        IReceiveEndpointBuilder
    {
        readonly IReceiveEndpointConfiguration _configuration;

        public ReceiveEndpointBuilder(IReceiveEndpointConfiguration configuration)
        {
            _configuration = configuration;
        }

        public ConnectHandle ConnectConsumePipe<T>(IPipe<ConsumeContext<T>> pipe)
            where T : class
        {
            return ConnectConsumePipe(pipe, ConnectPipeOptions.ConfigureConsumeTopology);
        }

        public virtual ConnectHandle ConnectConsumePipe<T>(IPipe<ConsumeContext<T>> pipe, ConnectPipeOptions options)
            where T : class
        {
            return _configuration.ConsumePipe.ConnectConsumePipe(pipe);
        }
    }
}
