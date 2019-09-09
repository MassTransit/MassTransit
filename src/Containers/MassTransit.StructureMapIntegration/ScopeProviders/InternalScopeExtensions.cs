namespace MassTransit.StructureMapIntegration.ScopeProviders
{
    using GreenPipes;
    using StructureMap;


    static class InternalScopeExtensions
    {
        public static void UpdatePayload(this PipeContext context, IContainer container)
        {
            context.AddOrUpdatePayload(() => container, existing => container);
        }
    }
}
