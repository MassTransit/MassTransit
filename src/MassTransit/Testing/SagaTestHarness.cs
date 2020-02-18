namespace MassTransit.Testing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using Decorators;
    using MessageObservers;
    using Saga;


    public class SagaTestHarness<TSaga>
        where TSaga : class, ISaga
    {
        readonly ReceivedMessageList _consumed;
        readonly SagaList<TSaga> _created;
        readonly IQuerySagaRepository<TSaga> _querySagaRepository;
        readonly SagaList<TSaga> _sagas;

        public SagaTestHarness(BusTestHarness testHarness, ISagaRepository<TSaga> repository, string queueName)
        {
            _querySagaRepository = repository as IQuerySagaRepository<TSaga>;

            TestTimeout = testHarness.TestTimeout;

            _consumed = new ReceivedMessageList(testHarness.TestTimeout);
            _created = new SagaList<TSaga>(testHarness.TestTimeout);
            _sagas = new SagaList<TSaga>(testHarness.TestTimeout);

            TestRepository = new TestSagaRepositoryDecorator<TSaga>(repository, _consumed, _created, _sagas);

            if (string.IsNullOrWhiteSpace(queueName))
                testHarness.OnConfigureReceiveEndpoint += ConfigureReceiveEndpoint;
            else
                testHarness.OnConfigureBus += configurator => ConfigureNamedReceiveEndpoint(configurator, queueName);
        }

        protected TestSagaRepositoryDecorator<TSaga> TestRepository { get; }
        protected TimeSpan TestTimeout { get; }

        public IReceivedMessageList Consumed => _consumed;
        public ISagaList<TSaga> Sagas => _sagas;
        public ISagaList<TSaga> Created => _created;

        protected IQuerySagaRepository<TSaga> QuerySagaRepository => _querySagaRepository;

        protected virtual void ConfigureReceiveEndpoint(IReceiveEndpointConfigurator configurator)
        {
            configurator.Saga(TestRepository);
        }

        protected virtual void ConfigureNamedReceiveEndpoint(IBusFactoryConfigurator configurator, string queueName)
        {
            configurator.ReceiveEndpoint(queueName, x =>
            {
                x.Saga(TestRepository);
            });
        }

        /// <summary>
        /// Waits until a saga exists with the specified correlationId
        /// </summary>
        /// <param name="correlationId"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public async Task<Guid?> Exists(Guid correlationId, TimeSpan? timeout = default)
        {
            if (_querySagaRepository == null)
                throw new InvalidOperationException("The repository does not support Query operations");

            var giveUpAt = DateTime.Now + (timeout ?? TestTimeout);

            var query = new SagaQuery<TSaga>(x => x.CorrelationId == correlationId);

            while (DateTime.Now < giveUpAt)
            {
                var saga = (await _querySagaRepository.Find(query).ConfigureAwait(false)).FirstOrDefault();
                if (saga != Guid.Empty)
                    return saga;

                await Task.Delay(10).ConfigureAwait(false);
            }

            return default;
        }

        /// <summary>
        /// Waits until at least one saga exists matching the specified filter
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public async Task<IList<Guid>> Match(Expression<Func<TSaga, bool>> filter, TimeSpan? timeout = default)
        {
            if (_querySagaRepository == null)
                throw new InvalidOperationException("The repository does not support Query operations");

            var giveUpAt = DateTime.Now + (timeout ?? TestTimeout);

            var query = new SagaQuery<TSaga>(filter);

            while (DateTime.Now < giveUpAt)
            {
                List<Guid> sagas = (await _querySagaRepository.Find(query).ConfigureAwait(false)).ToList();
                if (sagas.Count > 0)
                    return sagas;

                await Task.Delay(10).ConfigureAwait(false);
            }

            return new List<Guid>();
        }

        /// <summary>
        /// Waits until the saga matching the specified correlationId does NOT exist
        /// </summary>
        /// <param name="correlationId"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public async Task<Guid?> NotExists(Guid correlationId, TimeSpan? timeout = default)
        {
            if (_querySagaRepository == null)
                throw new InvalidOperationException("The repository does not support Query operations");

            var giveUpAt = DateTime.Now + (timeout ?? TestTimeout);

            var query = new SagaQuery<TSaga>(x => x.CorrelationId == correlationId);

            Guid? saga = default;
            while (DateTime.Now < giveUpAt)
            {
                saga = (await _querySagaRepository.Find(query).ConfigureAwait(false)).FirstOrDefault();
                if (saga == Guid.Empty)
                    return default;

                await Task.Delay(10).ConfigureAwait(false);
            }

            return saga;
        }
    }
}
