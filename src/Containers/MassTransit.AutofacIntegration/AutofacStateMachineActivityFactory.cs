namespace MassTransit.AutofacIntegration
{
    using Autofac;
    using Automatonymous;
    using GreenPipes;


    public class AutofacStateMachineActivityFactory :
        IStateMachineActivityFactory
    {
        public static readonly IStateMachineActivityFactory Instance = new AutofacStateMachineActivityFactory();

        public Activity<TInstance, TData> GetActivity<TActivity, TInstance, TData>(BehaviorContext<TInstance, TData> context)
            where TActivity : Activity<TInstance, TData>
        {
            var lifetimeScope = context.GetPayload<ILifetimeScope>();

            return lifetimeScope.Resolve<TActivity>();
        }

        public Activity<TInstance> GetActivity<TActivity, TInstance>(BehaviorContext<TInstance> context)
            where TActivity : Activity<TInstance>
        {
            var lifetimeScope = context.GetPayload<ILifetimeScope>();

            return lifetimeScope.Resolve<TActivity>();
        }
    }
}
