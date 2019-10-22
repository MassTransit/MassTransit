namespace MassTransit.SimpleInjectorIntegration
{
    using Automatonymous;
    using GreenPipes;
    using SimpleInjector;


    public class SimpleInjectorStateMachineActivityFactory :
        IStateMachineActivityFactory
    {
        public Activity<TInstance, TData> GetActivity<TActivity, TInstance, TData>(BehaviorContext<TInstance, TData> context)
            where TActivity : Activity<TInstance, TData>
        {
            var container = context.GetPayload<Container>();

            return (TActivity)container.GetInstance(typeof(TActivity));
        }

        public Activity<TInstance> GetActivity<TActivity, TInstance>(BehaviorContext<TInstance> context)
            where TActivity : Activity<TInstance>
        {
            var container = context.GetPayload<Container>();

            return (TActivity)container.GetInstance(typeof(TActivity));
        }
    }
}
