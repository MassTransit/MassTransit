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

        public Activity<TInstance, TData> GetActivity<TActivity, TInstance, TData>(BehaviorContext<TInstance, TData> context)
            where TActivity : class, Activity<TInstance, TData>
        {
            var lifetimeScope = context.GetPayload<ILifetimeScope>();

            return ActivatorUtils.GetOrCreateInstance<TActivity>(lifetimeScope);
        }

        public Activity<TInstance> GetActivity<TActivity, TInstance>(BehaviorContext<TInstance> context)
            where TActivity : class, Activity<TInstance>
        {
            var lifetimeScope = context.GetPayload<ILifetimeScope>();

            return ActivatorUtils.GetOrCreateInstance<TActivity>(lifetimeScope);
        }
    }
}
