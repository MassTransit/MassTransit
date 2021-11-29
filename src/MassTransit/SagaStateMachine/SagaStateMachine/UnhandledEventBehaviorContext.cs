namespace MassTransit.SagaStateMachine
{
    using System.Threading.Tasks;


    public class UnhandledEventBehaviorContext<TSaga> :
        BehaviorContextProxy<TSaga>,
        UnhandledEventContext<TSaga>
        where TSaga : class, ISaga
    {
        readonly BehaviorContext<TSaga> _context;
        readonly StateMachine<TSaga> _machine;

        public UnhandledEventBehaviorContext(StateMachine<TSaga> machine, BehaviorContext<TSaga> context, State state)
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
