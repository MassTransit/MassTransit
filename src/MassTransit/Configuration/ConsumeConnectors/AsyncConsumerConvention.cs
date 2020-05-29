namespace MassTransit.ConsumeConnectors
{
    public class AsyncConsumerConvention :
        IConsumerConvention
    {
        IConsumerMessageConvention IConsumerConvention.GetConsumerMessageConvention<T>()
        {
            return new AsyncConsumerMessageConvention<T>();
        }
    }
}
