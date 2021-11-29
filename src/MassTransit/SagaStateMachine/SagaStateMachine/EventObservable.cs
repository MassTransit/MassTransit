namespace MassTransit.SagaStateMachine
{
    using System;
    using System.Threading.Tasks;
    using Util;


    public class EventObservable<TSaga> :
        Connectable<IEventObserver<TSaga>>,
        IEventObserver<TSaga>
        where TSaga : class, ISaga
    {
        public Task PreExecute(BehaviorContext<TSaga> context)
        {
            return ForEachAsync(x => x.PreExecute(context));
        }

        public Task PreExecute<T>(BehaviorContext<TSaga, T> context)
            where T : class
        {
            return ForEachAsync(x => x.PreExecute(context));
        }

        public Task PostExecute(BehaviorContext<TSaga> context)
        {
            return ForEachAsync(x => x.PostExecute(context));
        }

        public Task PostExecute<T>(BehaviorContext<TSaga, T> context)
            where T : class
        {
            return ForEachAsync(x => x.PostExecute(context));
        }

        public Task ExecuteFault(BehaviorContext<TSaga> context, Exception exception)
        {
            return ForEachAsync(x => x.ExecuteFault(context, exception));
        }

        public Task ExecuteFault<T>(BehaviorContext<TSaga, T> context, Exception exception)
            where T : class
        {
            return ForEachAsync(x => x.ExecuteFault(context, exception));
        }
    }
}
