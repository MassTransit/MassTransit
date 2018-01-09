// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using Context;
    using GreenPipes;
    using Logging;
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

        public SagaInstance<TSaga> this[Guid id] => _sagas[id];

        public int Count => _sagas.Count;

        public Task<IEnumerable<Guid>> Find(ISagaQuery<TSaga> query)
        {
            return Task.FromResult(_sagas.Where(query).Select(x => x.Instance.CorrelationId));
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateScope("sagaRepository");
            scope.Set(new
            {
                _sagas.Count,
                Persistence = "memory"
            });
        }

        async Task ISagaRepository<TSaga>.Send<T>(ConsumeContext<T> context, ISagaPolicy<TSaga, T> policy, IPipe<SagaConsumeContext<TSaga, T>> next)
        {
            if (!context.CorrelationId.HasValue)
                throw new SagaException("The CorrelationId was not specified", typeof(TSaga), typeof(T));

            var sagaId = context.CorrelationId.Value;

            var needToLeaveSagas = true;

            await _sagas.MarkInUse(context.CancellationToken).ConfigureAwait(false);
            try
            {
                SagaInstance<TSaga> saga = _sagas[sagaId];
                if (saga != null)
                {
                    await saga.MarkInUse(context.CancellationToken).ConfigureAwait(false);
                    try
                    {
                        _sagas.Release();
                        needToLeaveSagas = false;

                        if (saga.IsRemoved)
                        {
                            saga.Release();
                            saga = null;
                        }
                        else
                        {
                            if (_log.IsDebugEnabled)
                                _log.DebugFormat("SAGA:{0}:{1} Used {2}", TypeMetadataCache<TSaga>.ShortName, sagaId, TypeMetadataCache<T>.ShortName);

                            SagaConsumeContext<TSaga, T> sagaConsumeContext = new InMemorySagaConsumeContext<TSaga, T>(context, saga.Instance,
                                () => Remove(saga, context.CancellationToken));

                            await policy.Existing(sagaConsumeContext, next).ConfigureAwait(false);
                        }
                    }
                    finally
                    {
                        saga?.Release();
                    }
                }

                if (saga == null)
                {
                    var missingSagaPipe = new MissingPipe<T>(this, next, true);

                    await policy.Missing(context, missingSagaPipe).ConfigureAwait(false);

                    _sagas.Release();
                    needToLeaveSagas = false;
                }
            }
            finally
            {
                if (needToLeaveSagas)
                {
                    _sagas.Release();
                }
            }
        }

        async Task ISagaRepository<TSaga>.SendQuery<T>(SagaQueryConsumeContext<TSaga, T> context, ISagaPolicy<TSaga, T> policy,
            IPipe<SagaConsumeContext<TSaga, T>> next)
        {
            SagaInstance<TSaga>[] existingSagas = _sagas.Where(context.Query).ToArray();
            if (existingSagas.Length == 0)
            {
                var missingSagaPipe = new MissingPipe<T>(this, next);
                await policy.Missing(context, missingSagaPipe).ConfigureAwait(false);
            }
            else
                await Task.WhenAll(existingSagas.Select(instance => SendToInstance(context, policy, instance, next))).ConfigureAwait(false);
        }

        async Task SendToInstance<T>(SagaQueryConsumeContext<TSaga, T> context, ISagaPolicy<TSaga, T> policy, SagaInstance<TSaga> saga,
            IPipe<SagaConsumeContext<TSaga, T>> next)
            where T : class
        {
            await saga.MarkInUse(context.CancellationToken).ConfigureAwait(false);
            try
            {
                if (saga.IsRemoved)
                    return;

                if (_log.IsDebugEnabled)
                    _log.DebugFormat("SAGA:{0}:{1} Used {2}", TypeMetadataCache<TSaga>.ShortName, saga.Instance.CorrelationId, TypeMetadataCache<T>.ShortName);

                SagaConsumeContext<TSaga, T> sagaConsumeContext = new InMemorySagaConsumeContext<TSaga, T>(context, saga.Instance,
                    () => Remove(saga, context.CancellationToken));

                await policy.Existing(sagaConsumeContext, next).ConfigureAwait(false);
            }
            finally
            {
                saga.Release();
            }
        }

        /// <summary>
        /// Add an instance to the saga repository
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task Add(SagaInstance<TSaga> instance, CancellationToken cancellationToken)
        {
            await _sagas.MarkInUse(cancellationToken).ConfigureAwait(false);
            try
            {
                _sagas.Add(instance);
            }
            finally
            {
                _sagas.Release();
            }
        }

        /// <summary>
        /// Adds the saga within an existing lock
        /// </summary>
        /// <param name="instance"></param>
        void AddWithinLock(SagaInstance<TSaga> instance)
        {
            _sagas.Add(instance);
        }

        async Task Remove(SagaInstance<TSaga> instance, CancellationToken cancellationToken)
        {
            instance.Remove();

            await _sagas.MarkInUse(cancellationToken).ConfigureAwait(false);
            try
            {
                _sagas.Remove(instance);
            }
            finally
            {
                _sagas.Release();
            }
        }

        void RemoveWithinLock(SagaInstance<TSaga> instance)
        {
            instance.Remove();

            _sagas.Remove(instance);
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
            readonly bool _withinLock;

            public MissingPipe(InMemorySagaRepository<TSaga> repository, IPipe<SagaConsumeContext<TSaga, TMessage>> next, bool withinLock = false)
            {
                _repository = repository;
                _next = next;
                _withinLock = withinLock;
            }

            void IProbeSite.Probe(ProbeContext context)
            {
                _next.Probe(context);
            }

            public async Task Send(SagaConsumeContext<TSaga, TMessage> context)
            {
                var instance = new SagaInstance<TSaga>(context.Saga);

                await instance.MarkInUse(context.CancellationToken).ConfigureAwait(false);
                try
                {
                    var proxy = new InMemorySagaConsumeContext<TSaga, TMessage>(context, context.Saga, () => RemoveNewSaga(instance, context.CancellationToken));

                    if (_withinLock)
                        _repository.AddWithinLock(instance);
                    else
                        await _repository.Add(instance, context.CancellationToken).ConfigureAwait(false);

                    if (_log.IsDebugEnabled)
                    {
                        _log.DebugFormat("SAGA:{0}:{1} Added {2}", TypeMetadataCache<TSaga>.ShortName, context.Saga.CorrelationId,
                            TypeMetadataCache<TMessage>.ShortName);
                    }

                    try
                    {
                        await _next.Send(proxy).ConfigureAwait(false);

                        if (proxy.IsCompleted)
                        {
                            if (_log.IsDebugEnabled)
                            {
                                _log.DebugFormat("SAGA:{0}:{1} Removed {2}", TypeMetadataCache<TSaga>.ShortName, context.Saga.CorrelationId,
                                    TypeMetadataCache<TMessage>.ShortName);
                            }

                            await RemoveNewSaga(instance, context.CancellationToken).ConfigureAwait(false);
                        }
                    }
                    catch (Exception)
                    {
                        if (_log.IsDebugEnabled)
                        {
                            _log.DebugFormat("SAGA:{0}:{1} Removed(Fault) {2}", TypeMetadataCache<TSaga>.ShortName, context.Saga.CorrelationId,
                                TypeMetadataCache<TMessage>.ShortName);
                        }

                        await RemoveNewSaga(instance, context.CancellationToken).ConfigureAwait(false);

                        throw;
                    }
                }
                finally
                {
                    instance.Release();
                }
            }

            async Task RemoveNewSaga(SagaInstance<TSaga> instance, CancellationToken cancellationToken)
            {
                if (_withinLock)
                    _repository.RemoveWithinLock(instance);
                else
                    await _repository.Remove(instance, cancellationToken).ConfigureAwait(false);
            }
        }
    }
}