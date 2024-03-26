namespace MassTransit
{
    using System;
    using System.Threading.Tasks;


    public partial class MassTransitStateMachine<TInstance>
        where TInstance : class, SagaStateMachineInstance
    {
        public class SelectedEventObserver :
            IEventObserver<TInstance>
        {
            readonly Event _event;
            readonly IEventObserver<TInstance> _observer;

            public SelectedEventObserver(Event @event, IEventObserver<TInstance> observer)
            {
                _event = @event;
                _observer = observer;
            }

            public Task PreExecute(BehaviorContext<TInstance> context)
            {
                return _event.Equals(context.Event)
                    ? _observer.PreExecute(context)
                    : Task.CompletedTask;
            }

            public Task PreExecute<T>(BehaviorContext<TInstance, T> context)
                where T : class
            {
                return _event.Equals(context.Event)
                    ? _observer.PreExecute(context)
                    : Task.CompletedTask;
            }

            public Task PostExecute(BehaviorContext<TInstance> context)
            {
                return _event.Equals(context.Event)
                    ? _observer.PostExecute(context)
                    : Task.CompletedTask;
            }

            public Task PostExecute<T>(BehaviorContext<TInstance, T> context)
                where T : class
            {
                return _event.Equals(context.Event)
                    ? _observer.PostExecute(context)
                    : Task.CompletedTask;
            }

            public Task ExecuteFault(BehaviorContext<TInstance> context, Exception exception)
            {
                return _event.Equals(context.Event)
                    ? _observer.ExecuteFault(context, exception)
                    : Task.CompletedTask;
            }

            public Task ExecuteFault<T>(BehaviorContext<TInstance, T> context, Exception exception)
                where T : class
            {
                return _event.Equals(context.Event)
                    ? _observer.ExecuteFault(context, exception)
                    : Task.CompletedTask;
            }
        }
    }
}
