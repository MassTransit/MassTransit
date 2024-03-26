namespace MassTransit
{
    using System;
    using System.Threading.Tasks;
    using Util;


    public partial class MassTransitStateMachine<TInstance>
        where TInstance : class, SagaStateMachineInstance
    {
        public class EventObservable :
            Connectable<IEventObserver<TInstance>>,
            IEventObserver<TInstance>
        {
            public Task PreExecute(BehaviorContext<TInstance> context)
            {
                return ForEachAsync(x => x.PreExecute(context));
            }

            public Task PreExecute<T>(BehaviorContext<TInstance, T> context)
                where T : class
            {
                return ForEachAsync(x => x.PreExecute(context));
            }

            public Task PostExecute(BehaviorContext<TInstance> context)
            {
                return ForEachAsync(x => x.PostExecute(context));
            }

            public Task PostExecute<T>(BehaviorContext<TInstance, T> context)
                where T : class
            {
                return ForEachAsync(x => x.PostExecute(context));
            }

            public Task ExecuteFault(BehaviorContext<TInstance> context, Exception exception)
            {
                return ForEachAsync(x => x.ExecuteFault(context, exception));
            }

            public Task ExecuteFault<T>(BehaviorContext<TInstance, T> context, Exception exception)
                where T : class
            {
                return ForEachAsync(x => x.ExecuteFault(context, exception));
            }

            public void Method4()
            {
            }

            public void Method5()
            {
            }

            public void Method6()
            {
            }
        }
    }
}
