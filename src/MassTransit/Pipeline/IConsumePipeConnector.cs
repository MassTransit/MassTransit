namespace MassTransit.Pipeline
{
    using GreenPipes;


    public interface IConsumePipeConnector
    {
        ConnectHandle ConnectConsumePipe<T>(IPipe<ConsumeContext<T>> pipe)
            where T : class;
    }
}
