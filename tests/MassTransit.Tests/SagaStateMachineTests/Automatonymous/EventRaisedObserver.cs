namespace MassTransit.Tests.SagaStateMachineTests.Automatonymous
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;


    class EventRaisedObserver<TInstance> :
        IEventObserver<TInstance>
        where TInstance : class, ISaga
    {
        public EventRaisedObserver()
        {
            Events = new List<BehaviorContext<TInstance>>();
        }

        public IList<BehaviorContext<TInstance>> Events { get; }

        public async Task PreExecute(BehaviorContext<TInstance> context)
        {
        }

        public async Task PreExecute<T>(BehaviorContext<TInstance, T> context)
            where T : class
        {
        }

        public async Task PostExecute(BehaviorContext<TInstance> context)
        {
            Events.Add(context);
        }

        public async Task PostExecute<T>(BehaviorContext<TInstance, T> context)
            where T : class
        {
            Events.Add(context);
        }

        public async Task ExecuteFault(BehaviorContext<TInstance> context, Exception exception)
        {
        }

        public async Task ExecuteFault<T>(BehaviorContext<TInstance, T> context, Exception exception)
            where T : class
        {
        }
    }
}
