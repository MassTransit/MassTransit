namespace MassTransit.Configuration
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
