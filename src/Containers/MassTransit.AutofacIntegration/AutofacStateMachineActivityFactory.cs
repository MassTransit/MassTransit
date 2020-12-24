namespace MassTransit.AutofacIntegration
{
    using Autofac;
    using Automatonymous;
    using GreenPipes;
    using ScopeProviders;


    public class AutofacStateMachineActivityFactory :
        IStateMachineActivityFactory
    {
        public static readonly IStateMachineActivityFactory Instance = new AutofacStateMachineActivityFactory();

        public T GetService<T>(PipeContext context)
            where T : class
        {
            var lifetimeScope = context.GetPayload<ILifetimeScope>();

            return ActivatorUtils.GetOrCreateInstance<T>(lifetimeScope);
        }
    }
}
