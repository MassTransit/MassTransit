namespace MassTransit.Configuration
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
