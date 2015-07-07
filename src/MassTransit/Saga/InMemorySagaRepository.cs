// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Saga
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Logging;
    using MassTransit.Pipeline;
    using Util;


    public class InMemorySagaRepository<TSaga> :
        ISagaRepository<TSaga>,
        IQuerySagaRepository<TSaga>
        where TSaga : class, ISaga
    {
        static readonly ILog _log = Logger.Get(typeof(InMemorySagaRepository<TSaga>));
        readonly IndexedSagaDictionary<TSaga> _sagas;

        public InMemorySagaRepository()
        {
            _sagas = new IndexedSagaDictionary<TSaga>();
        }

        public SagaInstance<TSaga> this[Guid id]
        {
            get { return _sagas[id]; }
        }

        public Task<IEnumerable<Guid>> Find(ISagaQuery<TSaga> query)
        {
            return Task.FromResult(_sagas.Where(query).Select(x => x.Instance.CorrelationId));
        }

        async void IProbeSite.Probe(ProbeContext context)
        {
            ProbeContext scope = context.CreateScope("sagaRepository");
            scope.Set(new
            {
                _sagas.Count,
                Persistence = "memory",
            });
        }

        async Task ISagaRepository<TSaga>.Send<T>(ConsumeContext<T> context, ISagaPolicy<TSaga, T> policy, IPipe<SagaConsumeContext<TSaga, T>> next)
        {
            if (!context.CorrelationId.HasValue)
                throw new SagaException("The CorrelationId was not specified", typeof(TSaga), typeof(T));

            Guid sagaId = context.CorrelationId.Value;

            bool needToLeaveSagas = true;
            await _sagas.MarkInUse(context.CancellationToken);
            try
            {
                SagaInstance<TSaga> saga = _sagas[sagaId];
                if (saga == null)
                {
                    _sagas.Release();
                    needToLeaveSagas = false;

                    var missingSagaPipe = new MissingPipe<T>(this, next);

                    await policy.Missing(context, missingSagaPipe);
                }
                else
                {
                    await saga.MarkInUse(context.CancellationToken);
                    try
                    {
                        _sagas.Release();
                        needToLeaveSagas = false;

                        if (_log.IsDebugEnabled)
                            _log.DebugFormat("SAGA:{0}:{1} Used {2}", TypeMetadataCache<TSaga>.ShortName, sagaId, TypeMetadataCache<T>.ShortName);

                        SagaConsumeContext<TSaga, T> sagaConsumeContext = new InMemorySagaConsumeContext<TSaga, T>(this, context, saga.Instance);

                        await policy.Existing(sagaConsumeContext, next);
                    }
                    finally
                    {
                        saga.Release();
                    }
                }
            }
            finally
            {
                if (needToLeaveSagas)
                    _sagas.Release();
            }
        }

        async Task ISagaRepository<TSaga>.SendQuery<T>(SagaQueryConsumeContext<TSaga, T> context, ISagaPolicy<TSaga, T> policy,
            IPipe<SagaConsumeContext<TSaga, T>> next)
        {
            SagaInstance<TSaga>[] existingSagas = _sagas.Where(context.Query).ToArray();
            if (existingSagas.Length == 0)
            {
                var missingSagaPipe = new MissingPipe<T>(this, next);
                await policy.Missing(context, missingSagaPipe);
            }
            else
                await Task.WhenAll(existingSagas.Select(instance => SendToInstance(context, policy, instance, next)));
        }

        async Task SendToInstance<T>(SagaQueryConsumeContext<TSaga, T> context, ISagaPolicy<TSaga, T> policy, SagaInstance<TSaga> saga,
            IPipe<SagaConsumeContext<TSaga, T>> next)
            where T : class
        {
            await saga.MarkInUse(context.CancellationToken);
            try
            {
                if (_log.IsDebugEnabled)
                    _log.DebugFormat("SAGA:{0}:{1} Used {2}", TypeMetadataCache<TSaga>.ShortName, saga.Instance.CorrelationId, TypeMetadataCache<T>.ShortName);

                SagaConsumeContext<TSaga, T> sagaConsumeContext = new InMemorySagaConsumeContext<TSaga, T>(this, context, saga.Instance);

                await policy.Existing(sagaConsumeContext, next);
            }
            finally
            {
                saga.Release();
            }
        }

        public async Task Add(TSaga newSaga, CancellationToken cancellationToken)
        {
            await _sagas.MarkInUse(cancellationToken);
            try
            {
                _sagas.Add(newSaga);
            }
            finally
            {
                _sagas.Release();
            }
        }

        public async Task Remove(TSaga saga, CancellationToken cancellationToken)
        {
            await _sagas.MarkInUse(cancellationToken);
            try
            {
                _sagas.Remove(new SagaInstance<TSaga>(saga));
            }
            finally
            {
                _sagas.Release();
            }
        }


        /// <summary>
        /// Once the message pipe has processed the saga instance, add it to the saga repository
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        class MissingPipe<TMessage> :
            IPipe<SagaConsumeContext<TSaga, TMessage>>
            where TMessage : class
        {
            readonly IPipe<SagaConsumeContext<TSaga, TMessage>> _next;
            readonly InMemorySagaRepository<TSaga> _repository;

            public MissingPipe(InMemorySagaRepository<TSaga> repository, IPipe<SagaConsumeContext<TSaga, TMessage>> next)
            {
                _repository = repository;
                _next = next;
            }

            void IProbeSite.Probe(ProbeContext context)
            {
                 _next.Probe(context);
            }

            public async Task Send(SagaConsumeContext<TSaga, TMessage> context)
            {
                if (_log.IsDebugEnabled)
                {
                    _log.DebugFormat("SAGA:{0}:{1} Added {2}", TypeMetadataCache<TSaga>.ShortName, context.Saga.CorrelationId,
                        TypeMetadataCache<TMessage>.ShortName);
                }

                var proxy = new InMemorySagaConsumeContext<TSaga, TMessage>(_repository, context, context.Saga);

                await _repository.Add(context.Saga, context.CancellationToken);

                await _next.Send(proxy);

                if (proxy.IsCompleted)
                    await _repository.Remove(proxy.Saga, context.CancellationToken);
            }
        }
    }
}