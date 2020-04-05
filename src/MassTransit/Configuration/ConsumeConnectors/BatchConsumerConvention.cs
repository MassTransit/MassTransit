namespace MassTransit.ConsumeConnectors
{
    public class BatchConsumerConvention :
        IConsumerConvention
    {
        IConsumerMessageConvention IConsumerConvention.GetConsumerMessageConvention<T>()
        {
            return new BatchConsumerMessageConvention<T>();
        }
    }
}
