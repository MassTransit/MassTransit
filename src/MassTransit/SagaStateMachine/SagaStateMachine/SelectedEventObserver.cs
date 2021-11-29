namespace MassTransit.SagaStateMachine
{
    using System;
    using System.Threading.Tasks;


    public class SelectedEventObserver<TSaga> :
        IEventObserver<TSaga>
        where TSaga : class, ISaga
    {
        readonly Event _event;
        readonly IEventObserver<TSaga> _observer;

        public SelectedEventObserver(Event @event, IEventObserver<TSaga> observer)
        {
            _event = @event;
            _observer = observer;
        }

        public Task PreExecute(BehaviorContext<TSaga> context)
        {
            return _event.Equals(context.Event)
                ? _observer.PreExecute(context)
                : Task.CompletedTask;
        }

        public Task PreExecute<T>(BehaviorContext<TSaga, T> context)
            where T : class
        {
            return _event.Equals(context.Event)
                ? _observer.PreExecute(context)
                : Task.CompletedTask;
        }

        public Task PostExecute(BehaviorContext<TSaga> context)
        {
            return _event.Equals(context.Event)
                ? _observer.PostExecute(context)
                : Task.CompletedTask;
        }

        public Task PostExecute<T>(BehaviorContext<TSaga, T> context)
            where T : class
        {
            return _event.Equals(context.Event)
                ? _observer.PostExecute(context)
                : Task.CompletedTask;
        }

        public Task ExecuteFault(BehaviorContext<TSaga> context, Exception exception)
        {
            return _event.Equals(context.Event)
                ? _observer.ExecuteFault(context, exception)
                : Task.CompletedTask;
        }

        public Task ExecuteFault<T>(BehaviorContext<TSaga, T> context, Exception exception)
            where T : class
        {
            return _event.Equals(context.Event)
                ? _observer.ExecuteFault(context, exception)
                : Task.CompletedTask;
        }
    }
}
