namespace MassTransit
{
    using GreenPipes;
    using Metadata;


    public static class ProbeContextExtensions
    {
        public static ProbeContext CreateConsumerFactoryScope<TConsumer>(this ProbeContext context, string source)
        {
            var scope = context.CreateScope("consumerFactory");
            scope.Add("source", source);
            scope.Add("consumerType", TypeMetadataCache<TConsumer>.ShortName);

            return scope;
        }

        public static ProbeContext CreateMessageScope(this ProbeContext context, string messageType)
        {
            var scope = context.CreateScope(messageType);

            return scope;
        }
    }
}
