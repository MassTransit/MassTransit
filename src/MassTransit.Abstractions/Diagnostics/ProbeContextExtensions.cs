namespace MassTransit
{
    using Internals;


    public static class ProbeContextExtensions
    {
        public static ProbeContext CreateFilterScope(this ProbeContext context, string filterType)
        {
            var scope = context.CreateScope("filters");

            scope.Add("filterType", filterType);

            return scope;
        }

        public static ProbeContext CreateConsumerFactoryScope<TConsumer>(this ProbeContext context, string source)
        {
            var scope = context.CreateScope("consumerFactory");
            scope.Add("source", source);
            scope.Add("consumerType", TypeCache<TConsumer>.ShortName);

            return scope;
        }

        public static ProbeContext CreateMessageScope(this ProbeContext context, string messageType)
        {
            var scope = context.CreateScope(messageType);

            return scope;
        }
    }
}
