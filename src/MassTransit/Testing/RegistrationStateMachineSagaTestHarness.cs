namespace MassTransit.Testing
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Automatonymous;
    using Saga;


    public class RegistrationStateMachineSagaTestHarness<TInstance, TStateMachine> :
        BaseSagaTestHarness<TInstance>,
        IStateMachineSagaTestHarness<TInstance, TStateMachine>
        where TInstance : class, SagaStateMachineInstance
        where TStateMachine : SagaStateMachine<TInstance>
    {
        readonly TStateMachine _stateMachine;

        public RegistrationStateMachineSagaTestHarness(SagaTestHarnessRegistration<TInstance> registration, ISagaRepository<TInstance> repository,
            TStateMachine stateMachine)
            : base(repository, registration.TestTimeout)
        {
            _stateMachine = stateMachine;
            Consumed = registration.Consumed;
            Created = registration.Created;
            Sagas = registration.Sagas;
        }

        public IReceivedMessageList Consumed { get; }

        public ISagaList<TInstance> Sagas { get; }

        public ISagaList<TInstance> Created { get; }

        /// <summary>
        /// Waits until a saga exists with the specified correlationId in the specified state
        /// </summary>
        /// <param name="correlationId"></param>
        /// <param name="stateSelector"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public Task<Guid?> Exists(Guid correlationId, Func<TStateMachine, State> stateSelector, TimeSpan? timeout = default)
        {
            var state = stateSelector(_stateMachine);

            return Exists(correlationId, state, timeout);
        }

        /// <summary>
        /// Waits until a saga exists with the specified correlationId in the specified state
        /// </summary>
        /// <param name="correlationId"></param>
        /// <param name="state">The expected state</param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public async Task<Guid?> Exists(Guid correlationId, State state, TimeSpan? timeout = default)
        {
            if (QuerySagaRepository == null)
                throw new InvalidOperationException("The repository does not support Query operations");

            var giveUpAt = DateTime.Now + (timeout ?? TestTimeout);

            var query = new SagaQuery<TInstance>(x => x.CorrelationId == correlationId && _stateMachine.GetState(x).Result.Equals(state));

            while (DateTime.Now < giveUpAt)
            {
                var saga = (await QuerySagaRepository.Find(query).ConfigureAwait(false)).FirstOrDefault();
                if (saga != Guid.Empty)
                    return saga;

                await Task.Delay(10).ConfigureAwait(false);
            }

            return default;
        }
    }
}
