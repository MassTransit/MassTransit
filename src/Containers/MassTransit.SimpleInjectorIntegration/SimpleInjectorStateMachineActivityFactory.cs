namespace MassTransit.SimpleInjectorIntegration
{
    using Automatonymous;
    using GreenPipes;
    using SimpleInjector;


    public class SimpleInjectorStateMachineActivityFactory :
        IStateMachineActivityFactory
    {
        public static readonly IStateMachineActivityFactory Instance = new SimpleInjectorStateMachineActivityFactory();

        public Activity<TInstance, TData> GetActivity<TActivity, TInstance, TData>(BehaviorContext<TInstance, TData> context)
            where TActivity : Activity<TInstance, TData>
        {
            var container = context.GetPayload<Scope>();

            return (TActivity)container.GetInstance(typeof(TActivity));
        }

        public Activity<TInstance> GetActivity<TActivity, TInstance>(BehaviorContext<TInstance> context)
            where TActivity : Activity<TInstance>
        {
            var container = context.GetPayload<Scope>();

            return (TActivity)container.GetInstance(typeof(TActivity));
        }
    }
}
