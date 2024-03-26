namespace MassTransit.Middleware
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Logging;


    /// <summary>
    /// Dispatches the ConsumeContext to the consumer method for the specified message type
    /// </summary>
    /// <typeparam name="TInstance">The consumer type</typeparam>
    /// <typeparam name="TMessage">The message type</typeparam>
    public class StateMachineSagaMessageFilter<TInstance, TMessage> :
        ISagaMessageFilter<TInstance, TMessage>
        where TInstance : class, ISaga, SagaStateMachineInstance
        where TMessage : class
    {
        readonly string _activityName;
        readonly Event<TMessage> _event;
        readonly SagaStateMachine<TInstance> _machine;

        public StateMachineSagaMessageFilter(SagaStateMachine<TInstance> machine, Event<TMessage> @event)
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
                InstanceType = TypeCache<TInstance>.ShortName
            });

            List<State<TInstance>> states = _machine.States.Cast<State<TInstance>>().Where(x => x.Events.Contains(_event)).ToList();
            if (states.Any())
                scope.Add("states", states.Select(x => x.Name).ToArray());

            _machine.Probe(context);
        }

        public async Task Send(SagaConsumeContext<TInstance, TMessage> context, IPipe<SagaConsumeContext<TInstance, TMessage>> next)
        {
            BehaviorContext<TInstance, TMessage> behaviorContext =
                new MassTransitStateMachine<TInstance>.BehaviorContextProxy<TMessage>(_machine, context, context, _event);

            StartedActivity? activity = LogContext.Current?.StartSagaStateMachineActivity(behaviorContext);
            StartedInstrument? instrument = LogContext.Current?.StartSagaStateMachineInstrument(behaviorContext);

            try
            {
                if (activity is { Activity: { IsAllDataRequested: true } })
                {
                    State<TInstance> beginState = await behaviorContext.StateMachine.Accessor.Get(behaviorContext).ConfigureAwait(false);
                    if (beginState != null)
                        activity?.SetTag(DiagnosticHeaders.BeginState, beginState.Name);
                }

                await _machine.RaiseEvent(behaviorContext).ConfigureAwait(false);

                if (await _machine.IsCompleted(behaviorContext).ConfigureAwait(false))
                    await context.SetCompleted().ConfigureAwait(false);
            }
            catch (UnhandledEventException ex)
            {
                State<TInstance> currentState = await _machine.Accessor.Get(behaviorContext).ConfigureAwait(false);

                var stateMachineException = new NotAcceptedStateMachineException(typeof(TInstance), typeof(TMessage),
                    context.CorrelationId ?? Guid.Empty, currentState.Name, ex);

                activity?.AddExceptionEvent(stateMachineException);
                instrument?.AddException(ex);

                throw stateMachineException;
            }
            catch (Exception exception)
            {
                activity?.AddExceptionEvent(exception);
                instrument?.AddException(exception);

                throw;
            }
            finally
            {
                if (activity != null)
                {
                    if (activity.Value.Activity.IsAllDataRequested)
                    {
                        State<TInstance> endState = await behaviorContext.StateMachine.Accessor.Get(behaviorContext).ConfigureAwait(false);
                        if (endState != null)
                            activity?.SetTag(DiagnosticHeaders.EndState, endState.Name);
                    }

                    activity.Value.Stop();
                }

                instrument?.Stop();
            }
        }
    }
}
