namespace MassTransit.Testing.Implementations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;


    public class StateMachineSagaTestHarness<TInstance, TStateMachine> :
        SagaTestHarness<TInstance>,
        ISagaStateMachineTestHarness<TStateMachine, TInstance>
        where TInstance : class, SagaStateMachineInstance
        where TStateMachine : SagaStateMachine<TInstance>
    {
        public StateMachineSagaTestHarness(BusTestHarness testHarness, ISagaRepository<TInstance> repository, TStateMachine stateMachine, string queueName)
            : base(testHarness, repository, queueName)
        {
            StateMachine = stateMachine;
        }

        public TStateMachine StateMachine { get; }

        /// <summary>
        /// Waits until a saga exists with the specified correlationId in the specified state
        /// </summary>
        /// <param name="correlationId"></param>
        /// <param name="stateSelector"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public Task<Guid?> Exists(Guid correlationId, Func<TStateMachine, State> stateSelector, TimeSpan? timeout = default)
        {
            var state = stateSelector(StateMachine);

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

            ISagaQuery<TInstance> query = StateMachine.CreateSagaQuery(x => x.CorrelationId == correlationId, state);

            while (DateTime.Now < giveUpAt)
            {
                var saga = (await QuerySagaRepository.Find(query).ConfigureAwait(false)).FirstOrDefault();
                if (saga != Guid.Empty)
                    return saga;

                await Task.Delay(10).ConfigureAwait(false);
            }

            return default;
        }

        /// <summary>
        /// Waits until a saga exists with the specified correlationId in the specified state
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="stateSelector"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public Task<IList<Guid>> Exists(Expression<Func<TInstance, bool>> expression, Func<TStateMachine, State> stateSelector, TimeSpan? timeout = default)
        {
            var state = stateSelector(StateMachine);

            return Exists(expression, state, timeout);
        }

        /// <summary>
        /// Waits until a saga exists with the specified correlationId in the specified state
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="state">The expected state</param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public async Task<IList<Guid>> Exists(Expression<Func<TInstance, bool>> expression, State state, TimeSpan? timeout = default)
        {
            if (QuerySagaRepository == null)
                throw new InvalidOperationException("The repository does not support Query operations");

            var giveUpAt = DateTime.Now + (timeout ?? TestTimeout);

            ISagaQuery<TInstance> query = StateMachine.CreateSagaQuery(expression, state);

            while (DateTime.Now < giveUpAt)
            {
                List<Guid> sagas = (await QuerySagaRepository.Find(query).ConfigureAwait(false)).ToList();
                if (sagas.Count > 0)
                    return sagas;

                await Task.Delay(10).ConfigureAwait(false);
            }

            return default;
        }

        protected override void ConfigureReceiveEndpoint(IReceiveEndpointConfigurator configurator)
        {
            configurator.StateMachineSaga(StateMachine, TestRepository);
        }

        protected override void ConfigureNamedReceiveEndpoint(IBusFactoryConfigurator configurator, string queueName)
        {
            configurator.ReceiveEndpoint(queueName, x =>
            {
                x.StateMachineSaga(StateMachine, TestRepository);
            });
        }
    }
}
