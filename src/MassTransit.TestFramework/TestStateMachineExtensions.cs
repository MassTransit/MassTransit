namespace MassTransit
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Saga;
    using SagaStateMachine;
    using TestFramework;


    public static class TestStateMachineExtensions
    {
        public static Task RaiseEvent<T, TData, TInstance>(this T machine, TInstance instance, Event<TData> @event, TData data,
            CancellationToken cancellationToken = default)
            where T : class, StateMachine, StateMachine<TInstance>
            where TInstance : class, SagaStateMachineInstance
            where TData : class
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));

            var consumeContext = new TestConsumeContext<TData>(data, cancellationToken);
            var sagaConsumeContext = new InMemorySagaConsumeContext<TInstance, TData>(consumeContext, new SagaInstance<TInstance>(instance));
            var behaviorContext = new MassTransitStateMachine<TInstance>.BehaviorContextProxy<TData>(machine, sagaConsumeContext, consumeContext, @event);

            return machine.RaiseEvent(behaviorContext);
        }

        public static Task RaiseEvent<T, TData, TInstance>(this T machine, TInstance instance, Func<T, Event<TData>> eventSelector, TData data,
            CancellationToken cancellationToken = default)
            where T : class, StateMachine, StateMachine<TInstance>
            where TInstance : class, SagaStateMachineInstance
            where TData : class
        {
            Event<TData> @event = eventSelector(machine);
            if (@event == null)
                throw new ArgumentNullException(nameof(eventSelector), "The event selector did not return a valid event from the state machine");

            var consumeContext = new TestConsumeContext<TData>(data, cancellationToken);
            var sagaConsumeContext = new InMemorySagaConsumeContext<TInstance, TData>(consumeContext, new SagaInstance<TInstance>(instance));
            var behaviorContext = new MassTransitStateMachine<TInstance>.BehaviorContextProxy<TData>(machine, sagaConsumeContext, consumeContext, @event);

            return machine.RaiseEvent(behaviorContext);
        }

        public static Task RaiseEvent<T, TInstance>(this T machine, TInstance instance, Event @event, CancellationToken cancellationToken = default)
            where T : class, StateMachine, StateMachine<TInstance>
            where TInstance : class, SagaStateMachineInstance
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));

            var consumeContext = new TestConsumeContext<Data>(new Data(), cancellationToken);
            var sagaConsumeContext = new InMemorySagaConsumeContext<TInstance, Data>(consumeContext, new SagaInstance<TInstance>(instance));
            var behaviorContext = new MassTransitStateMachine<TInstance>.BehaviorContextProxy(machine, sagaConsumeContext, @event);

            return machine.RaiseEvent(behaviorContext);
        }

        public static Task RaiseEvent<T, TInstance>(this T machine, TInstance instance, Func<T, Event> eventSelector,
            CancellationToken cancellationToken = default)
            where T : class, StateMachine, StateMachine<TInstance>
            where TInstance : class, SagaStateMachineInstance
        {
            var @event = eventSelector(machine);
            if (@event == null)
                throw new ArgumentNullException(nameof(eventSelector), "The event selector did not return a valid event from the state machine");

            var consumeContext = new TestConsumeContext<Data>(new Data(), cancellationToken);
            var sagaConsumeContext = new InMemorySagaConsumeContext<TInstance, Data>(consumeContext, new SagaInstance<TInstance>(instance));
            var behaviorContext = new MassTransitStateMachine<TInstance>.BehaviorContextProxy(machine, sagaConsumeContext, @event);

            return machine.RaiseEvent(behaviorContext);
        }

        public static async Task<IEnumerable<Event>> NextEvents<TInstance>(this StateMachine<TInstance> machine, TInstance instance)
            where TInstance : class, SagaStateMachineInstance
        {
            var consumeContext = new TestConsumeContext<Data>(new Data());
            var sagaConsumeContext = new InMemorySagaConsumeContext<TInstance, Data>(consumeContext, new SagaInstance<TInstance>(instance));
            var behaviorContext = new MassTransitStateMachine<TInstance>.BehaviorContextProxy(machine, sagaConsumeContext, machine.Initial.Enter);

            return machine.NextEvents(await machine.Accessor.Get(behaviorContext).ConfigureAwait(false));
        }

        public static Task<State<TInstance>> GetState<TInstance>(this StateMachine<TInstance> machine, TInstance instance)
            where TInstance : class, SagaStateMachineInstance
        {
            var consumeContext = new TestConsumeContext<Data>(new Data());
            var sagaConsumeContext = new InMemorySagaConsumeContext<TInstance, Data>(consumeContext, new SagaInstance<TInstance>(instance));
            var behaviorContext = new MassTransitStateMachine<TInstance>.BehaviorContextProxy(machine, sagaConsumeContext, machine.Initial.Enter);

            return machine.Accessor.Get(behaviorContext);
        }

        public static Task TransitionToState<TInstance>(this StateMachine<TInstance> machine, TInstance instance, State state)
            where TInstance : class, SagaStateMachineInstance
        {
            IStateAccessor<TInstance> accessor = machine.Accessor;
            State<TInstance> toState = machine.GetState(state.Name);

            IStateMachineActivity<TInstance> activity = new TransitionActivity<TInstance>(toState, accessor);
            IBehavior<TInstance> behavior = new LastBehavior<TInstance>(activity);

            var consumeContext = new TestConsumeContext<Data>(new Data());
            var sagaConsumeContext = new InMemorySagaConsumeContext<TInstance, Data>(consumeContext, new SagaInstance<TInstance>(instance));
            var behaviorContext = new MassTransitStateMachine<TInstance>.BehaviorContextProxy(machine, sagaConsumeContext, machine.Initial.Enter);

            return behavior.Execute(behaviorContext);
        }


        class Data
        {
        }
    }
}
