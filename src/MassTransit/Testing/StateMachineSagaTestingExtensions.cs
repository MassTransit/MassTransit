namespace MassTransit.Testing
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using Implementations;


    public static class StateMachineSagaTestHarnessExtensions
    {
        public static ISagaStateMachineTestHarness<TStateMachine, TInstance> StateMachineSaga<TInstance, TStateMachine>(this BusTestHarness harness,
            TStateMachine stateMachine, string queueName = null)
            where TInstance : class, SagaStateMachineInstance
            where TStateMachine : SagaStateMachine<TInstance>
        {
            if (stateMachine == null)
                throw new ArgumentNullException(nameof(stateMachine));

            var repository = new InMemorySagaRepository<TInstance>();

            return new StateMachineSagaTestHarness<TInstance, TStateMachine>(harness, repository, stateMachine, queueName);
        }

        public static ISagaStateMachineTestHarness<TStateMachine, TInstance> StateMachineSaga<TInstance, TStateMachine>(this BusTestHarness harness,
            TStateMachine stateMachine, ISagaRepository<TInstance> repository, string queueName = null)
            where TInstance : class, SagaStateMachineInstance
            where TStateMachine : SagaStateMachine<TInstance>
        {
            if (stateMachine == null)
                throw new ArgumentNullException(nameof(stateMachine));

            if (repository == null)
                throw new ArgumentNullException(nameof(repository));

            return new StateMachineSagaTestHarness<TInstance, TStateMachine>(harness, repository, stateMachine, queueName);
        }

        public static TInstance ContainsInState<TStateMachine, TInstance>(this ISagaList<TInstance> sagas, Guid correlationId, TStateMachine machine,
            Func<TStateMachine, State> stateSelector)
            where TStateMachine : SagaStateMachine<TInstance>
            where TInstance : class, SagaStateMachineInstance
        {
            var state = stateSelector(machine);

            return ContainsInState(sagas, correlationId, machine, state);
        }

        public static T ContainsInState<T>(this ISagaList<T> sagas, Guid correlationId, SagaStateMachine<T> machine, State state)
            where T : class, SagaStateMachineInstance
        {
            Func<T, bool> filter = machine.CreateSagaFilter(x => x.CorrelationId == correlationId, state);

            var any = sagas.Select(x => filter(x)).Any();
            return any ? sagas.Contains(correlationId) : null;
        }

        public static Task<Guid?> ShouldContainSagaInState<TStateMachine, TInstance>(this ISagaRepository<TInstance> repository, Guid correlationId,
            TStateMachine machine, Func<TStateMachine, State> stateSelector, TimeSpan timeout)
            where TStateMachine : SagaStateMachine<TInstance>
            where TInstance : class, SagaStateMachineInstance
        {
            var state = stateSelector(machine);

            return ShouldContainSagaInState(repository, correlationId, machine, state, timeout);
        }

        public static Task<Guid?> ShouldContainSagaInState<TStateMachine, TInstance>(this ISagaRepository<TInstance> repository, Guid correlationId,
            TStateMachine machine, State state, TimeSpan timeout)
            where TStateMachine : SagaStateMachine<TInstance>
            where TInstance : class, SagaStateMachineInstance
        {
            return ShouldContainSagaInState(repository, x => x.CorrelationId == correlationId, machine, state, timeout);
        }

        public static Task<Guid?> ShouldContainSagaInState<TStateMachine, TInstance>(this ISagaRepository<TInstance> repository,
            Expression<Func<TInstance, bool>> expression, TStateMachine machine, Func<TStateMachine, State> stateSelector, TimeSpan timeout)
            where TStateMachine : SagaStateMachine<TInstance>
            where TInstance : class, SagaStateMachineInstance
        {
            var state = stateSelector(machine);

            return ShouldContainSagaInState(repository, expression, machine, state, timeout);
        }

        public static async Task<Guid?> ShouldContainSagaInState<TStateMachine, TInstance>(this ISagaRepository<TInstance> repository,
            Expression<Func<TInstance, bool>> expression, TStateMachine machine, State state, TimeSpan timeout)
            where TStateMachine : SagaStateMachine<TInstance>
            where TInstance : class, SagaStateMachineInstance
        {
            var querySagaRepository = repository as IQuerySagaRepository<TInstance>;
            if (querySagaRepository == null)
                throw new ArgumentException("The repository must support querying", nameof(repository));

            var giveUpAt = DateTime.Now + timeout;

            ISagaQuery<TInstance> query = machine.CreateSagaQuery(expression, state);

            while (DateTime.Now < giveUpAt)
            {
                var saga = (await querySagaRepository.Find(query).ConfigureAwait(false)).FirstOrDefault();
                if (saga != Guid.Empty)
                    return saga;

                await Task.Delay(10).ConfigureAwait(false);
            }

            return default;
        }
    }
}
