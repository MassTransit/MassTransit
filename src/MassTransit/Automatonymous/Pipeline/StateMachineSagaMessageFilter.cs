namespace Automatonymous.Pipeline
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Contexts;
    using GreenPipes;
    using MassTransit;
    using MassTransit.Context;
    using MassTransit.Logging;
    using MassTransit.Metadata;
    using MassTransit.Saga;
    using MassTransit.Saga.Pipeline.Filters;


    /// <summary>
    /// Dispatches the ConsumeContext to the consumer method for the specified message type
    /// </summary>
    /// <typeparam name="TInstance">The consumer type</typeparam>
    /// <typeparam name="TData">The message type</typeparam>
    public class StateMachineSagaMessageFilter<TInstance, TData> :
        ISagaMessageFilter<TInstance, TData>
        where TInstance : class, ISaga, SagaStateMachineInstance
        where TData : class
    {
        readonly Event<TData> _event;
        readonly SagaStateMachine<TInstance> _machine;

        public StateMachineSagaMessageFilter(SagaStateMachine<TInstance> machine, Event<TData> @event)
        {
            _machine = machine;
            _event = @event;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateScope("automatonymous");
            scope.Set(new
            {
                Event = _event.Name,
                DataType = TypeMetadataCache<TData>.ShortName,
                InstanceType = TypeMetadataCache<TInstance>.ShortName
            });

            List<State<TInstance>> states = _machine.States.Cast<State<TInstance>>()
                .Where(x => x.Events.Contains(_event)).ToList();
            if (states.Any())
            {
                scope.Add("states", states.Select(x => x.Name).ToArray());
            }

            _machine.Probe(context);
        }

        public async Task Send(SagaConsumeContext<TInstance, TData> context, IPipe<SagaConsumeContext<TInstance, TData>> next)
        {
            var eventContext = new StateMachineEventContextProxy<TInstance, TData>(context, _machine, context.Saga, _event, context.Message);

            var activity = LogContext.IfEnabled(OperationName.Saga.RaiseEvent)
                ?.StartSagaActivity(context, (await _machine.Accessor.Get(eventContext).ConfigureAwait(false)).Name);
            try
            {
                await _machine.RaiseEvent(eventContext).ConfigureAwait(false);

                if (await _machine.IsCompleted(context.Saga).ConfigureAwait(false))
                    await context.SetCompleted().ConfigureAwait(false);
            }
            catch (UnhandledEventException ex)
            {
                var currentState = await _machine.Accessor.Get(eventContext).ConfigureAwait(false);

                throw new NotAcceptedStateMachineException(typeof(TInstance), typeof(TData), context.CorrelationId ?? Guid.Empty, currentState.Name, ex);
            }
            finally
            {
                activity?.AddTag(DiagnosticHeaders.EndState, (await _machine.Accessor.Get(eventContext).ConfigureAwait(false)).Name);
                activity?.Stop();
            }
        }
    }
}
