namespace MassTransit.SagaStateMachine
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;


    public class NonTransitionEventObserver<TSaga> :
        IEventObserver<TSaga>
        where TSaga : class, ISaga
    {
        readonly IReadOnlyDictionary<string, StateMachineEvent<TSaga>> _eventCache;
        readonly IEventObserver<TSaga> _observer;

        public NonTransitionEventObserver(IReadOnlyDictionary<string, StateMachineEvent<TSaga>> eventCache, IEventObserver<TSaga> observer)
        {
            _eventCache = eventCache;
            _observer = observer;
        }

        public Task PreExecute(BehaviorContext<TSaga> context)
        {
            if (_eventCache.TryGetValue(context.Event.Name, out StateMachineEvent<TSaga> stateMachineEvent) && !stateMachineEvent.IsTransitionEvent)
                return _observer.PreExecute(context);

            return Task.CompletedTask;
        }

        public Task PreExecute<T>(BehaviorContext<TSaga, T> context)
            where T : class
        {
            if (_eventCache.TryGetValue(context.Event.Name, out StateMachineEvent<TSaga> stateMachineEvent) && !stateMachineEvent.IsTransitionEvent)
                return _observer.PreExecute(context);

            return Task.CompletedTask;
        }

        public Task PostExecute(BehaviorContext<TSaga> context)
        {
            if (_eventCache.TryGetValue(context.Event.Name, out StateMachineEvent<TSaga> stateMachineEvent) && !stateMachineEvent.IsTransitionEvent)
                return _observer.PostExecute(context);

            return Task.CompletedTask;
        }

        public Task PostExecute<T>(BehaviorContext<TSaga, T> context)
            where T : class
        {
            if (_eventCache.TryGetValue(context.Event.Name, out StateMachineEvent<TSaga> stateMachineEvent) && !stateMachineEvent.IsTransitionEvent)
                return _observer.PostExecute(context);

            return Task.CompletedTask;
        }

        public Task ExecuteFault(BehaviorContext<TSaga> context, Exception exception)
        {
            if (_eventCache.TryGetValue(context.Event.Name, out StateMachineEvent<TSaga> stateMachineEvent) && !stateMachineEvent.IsTransitionEvent)
                return _observer.ExecuteFault(context, exception);

            return Task.CompletedTask;
        }

        public Task ExecuteFault<T>(BehaviorContext<TSaga, T> context, Exception exception)
            where T : class
        {
            if (_eventCache.TryGetValue(context.Event.Name, out StateMachineEvent<TSaga> stateMachineEvent) && !stateMachineEvent.IsTransitionEvent)
                return _observer.ExecuteFault(context, exception);

            return Task.CompletedTask;
        }
    }
}
