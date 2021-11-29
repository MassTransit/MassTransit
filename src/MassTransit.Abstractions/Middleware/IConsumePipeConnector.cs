namespace MassTransit
{
    public interface IConsumePipeConnector
    {
        ConnectHandle ConnectConsumePipe<T>(IPipe<ConsumeContext<T>> pipe)
            where T : class;

        ConnectHandle ConnectConsumePipe<T>(IPipe<ConsumeContext<T>> pipe, ConnectPipeOptions options)
            where T : class;
    }
}
