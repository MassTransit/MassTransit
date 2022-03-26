namespace MassTransit.Middleware
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Logging;
    using SagaStateMachine;


    /// <summary>
    /// Dispatches the ConsumeContext to the consumer method for the specified message type
    /// </summary>
    /// <typeparam name="TSaga">The consumer type</typeparam>
    /// <typeparam name="TMessage">The message type</typeparam>
    public class StateMachineSagaMessageFilter<TSaga, TMessage> :
        ISagaMessageFilter<TSaga, TMessage>
        where TSaga : class, ISaga, SagaStateMachineInstance
        where TMessage : class
    {
        readonly string _activityName;
        readonly Event<TMessage> _event;
        readonly SagaStateMachine<TSaga> _machine;

        public StateMachineSagaMessageFilter(SagaStateMachine<TSaga> machine, Event<TMessage> @event)
        {
            _machine = machine;
            _event = @event;

            _activityName = $"{_machine.Name} process";
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateScope("sagaStateMachine");
            scope.Set(new
            {
                Event = _event.Name,
                DataType = TypeCache<TMessage>.ShortName,
                InstanceType = TypeCache<TSaga>.ShortName
            });

            List<State<TSaga>> states = _machine.States.Cast<State<TSaga>>().Where(x => x.Events.Contains(_event)).ToList();
            if (states.Any())
                scope.Add("states", states.Select(x => x.Name).ToArray());

            _machine.Probe(context);
        }

        public async Task Send(SagaConsumeContext<TSaga, TMessage> context, IPipe<SagaConsumeContext<TSaga, TMessage>> next)
        {
            BehaviorContext<TSaga, TMessage> behaviorContext = new BehaviorContextProxy<TSaga, TMessage>(_machine, context, context, _event);

            StartedActivity? activity = LogContext.Current?.StartSagaStateMachineActivity(behaviorContext);
            try
            {
                if (activity != null)
                {
                    State<TSaga> beginState = await behaviorContext.StateMachine.Accessor.Get(behaviorContext).ConfigureAwait(false);
                    if (beginState != null)
                        activity?.AddTag(DiagnosticHeaders.BeginState, beginState.Name);
                }

                await _machine.RaiseEvent(behaviorContext).ConfigureAwait(false);

                if (await _machine.IsCompleted(behaviorContext).ConfigureAwait(false))
                    await context.SetCompleted().ConfigureAwait(false);
            }
            catch (UnhandledEventException ex)
            {
                State<TSaga> currentState = await _machine.Accessor.Get(behaviorContext).ConfigureAwait(false);

                throw new NotAcceptedStateMachineException(typeof(TSaga), typeof(TMessage), context.CorrelationId ?? Guid.Empty, currentState.Name, ex);
            }
            finally
            {
                activity?.AddTag(DiagnosticHeaders.EndState, (await _machine.Accessor.Get(behaviorContext).ConfigureAwait(false)).Name);
                activity?.Stop();
            }
        }
    }
}
