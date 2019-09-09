namespace MassTransit.Testing
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Automatonymous;
    using Saga;


    public static class StateMachineSagaTestHarnessExtensions
    {
        public static StateMachineSagaTestHarness<TInstance, TStateMachine> StateMachineSaga<TInstance, TStateMachine>(this BusTestHarness harness,
            TStateMachine stateMachine, string queueName = null)
            where TInstance : class, SagaStateMachineInstance
            where TStateMachine : SagaStateMachine<TInstance>
        {
            if (stateMachine == null)
                throw new ArgumentNullException(nameof(stateMachine));

            var repository = new InMemorySagaRepository<TInstance>();

            return new StateMachineSagaTestHarness<TInstance, TStateMachine>(harness, repository, stateMachine, queueName);
        }

        public static StateMachineSagaTestHarness<TInstance, TStateMachine> StateMachineSaga<TInstance, TStateMachine>(this BusTestHarness harness,
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

        public static TInstance ContainsInState<TStateMachine, TInstance>(this ISagaList<TInstance> sagas, Guid sagaId, TStateMachine machine,
            Func<TStateMachine, State> stateSelector)
            where TStateMachine : SagaStateMachine<TInstance>
            where TInstance : class, SagaStateMachineInstance
        {
            var state = stateSelector(machine);

            return ContainsInState(sagas, sagaId, machine, state);
        }

        public static TSaga ContainsInState<TSaga>(this ISagaList<TSaga> sagas, Guid sagaId, SagaStateMachine<TSaga> machine, State state)
            where TSaga : class, SagaStateMachineInstance
        {
            bool any = sagas.Select(x => x.CorrelationId == sagaId && machine.GetState(x).Result.Equals(state)).Any();
            return any ? sagas.Contains(sagaId) : null;
        }

        public static async Task<Guid?> ShouldContainSagaInState<TStateMachine, TInstance>(this ISagaRepository<TInstance> repository, Guid sagaId,
            TStateMachine machine, Func<TStateMachine, State> stateSelector, TimeSpan timeout)
            where TStateMachine : SagaStateMachine<TInstance>
            where TInstance : class, SagaStateMachineInstance
        {
            var querySagaRepository = repository as IQuerySagaRepository<TInstance>;
            if (querySagaRepository == null)
                throw new ArgumentException("The repository must support querying", nameof(repository));

            var state = stateSelector(machine);

            DateTime giveUpAt = DateTime.Now + timeout;

            while (DateTime.Now < giveUpAt)
            {
                Guid saga = (await querySagaRepository.Where(x => x.CorrelationId == sagaId && machine.GetState(x).Result.Equals(state))
                    .ConfigureAwait(false)).FirstOrDefault();

                if (saga != Guid.Empty)
                    return saga;

                await Task.Delay(10).ConfigureAwait(false);
            }

            return default;
        }
    }
}
