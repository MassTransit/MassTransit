namespace MassTransit
{
    using System.Threading.Tasks;


    public partial class MassTransitStateMachine<TInstance>
        where TInstance : class, SagaStateMachineInstance
    {
        class UnhandledEventBehaviorContext :
            BehaviorContextProxy,
            UnhandledEventContext<TInstance>
        {
            readonly BehaviorContext<TInstance> _context;
            readonly StateMachine<TInstance> _machine;

            public UnhandledEventBehaviorContext(StateMachine<TInstance> machine, BehaviorContext<TInstance> context, State state)
                : base(machine, context, context.Event)
            {
                _context = context;
                CurrentState = state;
                _machine = machine;
            }

            public State CurrentState { get; }

            public Event Event => _context.Event;

            public Task Ignore()
            {
                return Task.CompletedTask;
            }

            public Task Throw()
            {
                throw new UnhandledEventException(_machine.Name, _context.Event.Name, CurrentState.Name);
            }
        }
    }
}
