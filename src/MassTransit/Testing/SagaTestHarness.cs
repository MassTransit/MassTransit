// Copyright 2007-2017 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
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
        readonly ISagaRepository<TSaga> _repository;
        readonly SagaList<TSaga> _sagas;
        readonly TimeSpan _testTimeout;

        public SagaTestHarness(BusTestHarness testHarness, ISagaRepository<TSaga> repository, string queueName)
        {
            _repository = repository;
            _querySagaRepository = _repository as IQuerySagaRepository<TSaga>;

            _testTimeout = testHarness.TestTimeout;

            _consumed = new ReceivedMessageList(testHarness.TestTimeout);
            _created = new SagaList<TSaga>(testHarness.TestTimeout);
            _sagas = new SagaList<TSaga>(testHarness.TestTimeout);

            TestRepository = new TestSagaRepositoryDecorator<TSaga>(_repository, _consumed, _created, _sagas);

            if (string.IsNullOrWhiteSpace(queueName))
                testHarness.OnConfigureReceiveEndpoint += ConfigureReceiveEndpoint;
            else
                testHarness.OnConfigureBus += configurator => ConfigureNamedReceiveEndpoint(configurator, queueName);
        }

        public TestSagaRepositoryDecorator<TSaga> TestRepository { get; }

        public IReceivedMessageList Consumed => _consumed;
        public ISagaList<TSaga> Sagas => _sagas;
        public ISagaList<TSaga> Created => _created;

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
        /// <param name="sagaId"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public async Task<Guid?> Exists(Guid sagaId, TimeSpan? timeout = default(TimeSpan?))
        {
            if (_querySagaRepository == null)
                throw new InvalidOperationException("The repository does not support Query operations");

            var giveUpAt = DateTime.Now + (timeout ?? _testTimeout);

            while (DateTime.Now < giveUpAt)
            {
                var saga = (await _querySagaRepository.Where(x => x.CorrelationId == sagaId).ConfigureAwait(false)).FirstOrDefault();
                if (saga != Guid.Empty)
                    return saga;

                await Task.Delay(10).ConfigureAwait(false);
            }

            return default(Guid?);
        }

        /// <summary>
        /// Waits until at least one saga exists matching the specified filter
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public async Task<IList<Guid>> Match(Expression<Func<TSaga, bool>> filter, TimeSpan? timeout = default(TimeSpan?))
        {
            if (_querySagaRepository == null)
                throw new InvalidOperationException("The repository does not support Query operations");

            var giveUpAt = DateTime.Now + (timeout ?? _testTimeout);

            var query = new SagaQuery<TSaga>(filter);

            while (DateTime.Now < giveUpAt)
            {
                List<Guid> sagas = (await _querySagaRepository.Where(query.FilterExpression).ConfigureAwait(false)).ToList();
                if (sagas.Count > 0)
                    return sagas;

                await Task.Delay(10).ConfigureAwait(false);
            }

            return new List<Guid>();
        }

        /// <summary>
        /// Waits until the saga matching the specified correlationId does NOT exist
        /// </summary>
        /// <param name="sagaId"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public async Task<Guid?> NotExists(Guid sagaId, TimeSpan? timeout = default(TimeSpan?))
        {
            if (_querySagaRepository == null)
                throw new InvalidOperationException("The repository does not support Query operations");

            var giveUpAt = DateTime.Now + (timeout ?? _testTimeout);

            Guid? saga = default(Guid?);
            while (DateTime.Now < giveUpAt)
            {
                saga = (await _querySagaRepository.Where(x => x.CorrelationId == sagaId).ConfigureAwait(false)).FirstOrDefault();
                if (saga == Guid.Empty)
                    return default(Guid?);

                await Task.Delay(10).ConfigureAwait(false);
            }

            return saga;
        }
    }
}