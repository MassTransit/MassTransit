namespace MassTransit.LamarIntegration
{
    using Automatonymous;
    using GreenPipes;
    using Lamar;


    public class LamarStateMachineActivityFactory :
        IStateMachineActivityFactory
    {
        public static readonly IStateMachineActivityFactory Instance = new LamarStateMachineActivityFactory();

        public Activity<TInstance, TData> GetActivity<TActivity, TInstance, TData>(BehaviorContext<TInstance, TData> context)
            where TActivity : Activity<TInstance, TData>
        {
            var nestedContainer = context.GetPayload<INestedContainer>();

            return nestedContainer.GetInstance<TActivity>();
        }

        public Activity<TInstance> GetActivity<TActivity, TInstance>(BehaviorContext<TInstance> context)
            where TActivity : Activity<TInstance>
        {
            var nestedContainer = context.GetPayload<INestedContainer>();

            return nestedContainer.GetInstance<TActivity>();
        }
    }
}
