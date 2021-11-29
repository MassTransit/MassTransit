namespace MassTransit.Tests.Conventional
{
    using MassTransit.Configuration;


    class CustomConsumerConvention :
        IConsumerConvention
    {
        IConsumerMessageConvention IConsumerConvention.GetConsumerMessageConvention<T>()
        {
            return new CustomConsumerMessageConvention<T>();
        }
    }
}
