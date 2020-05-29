namespace MassTransit.Tests.Conventional
{
    class CustomConsumerConvention :
        IConsumerConvention
    {
        IConsumerMessageConvention IConsumerConvention.GetConsumerMessageConvention<T>()
        {
            return new CustomConsumerMessageConvention<T>();
        }
    }
}
