namespace MassTransit.LamarIntegration.ScopeProviders
{
    using GreenPipes;
    using Lamar;


    static class InternalScopeExtensions
    {
        public static void UpdateScope(this INestedContainer container, ConsumeContext consumeContext)
        {
            container.Inject(consumeContext);
        }

        public static void UpdatePayload(this PipeContext context, INestedContainer nestedContainer)
        {
            context.AddOrUpdatePayload(() => nestedContainer, existing => nestedContainer);
        }
    }
}
