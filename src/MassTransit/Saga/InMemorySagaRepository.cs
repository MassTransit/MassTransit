// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using Logging;
    using MassTransit.Pipeline;
    using Util;


    public class InMemorySagaRepository<TSaga> :
        ISagaRepository<TSaga>
        where TSaga : class, ISaga
    {
        static readonly ILog _log = Logger.Get(typeof(InMemorySagaRepository<TSaga>));
        IndexedSagaDictionary<TSaga> _sagas;

        public InMemorySagaRepository()
        {
            _sagas = new IndexedSagaDictionary<TSaga>();
        }

        public TSaga this[Guid id]
        {
            get { return _sagas[id]; }
        }

        async Task ISagaRepository<TSaga>.Send<T>(ConsumeContext<T> context, IPipe<SagaConsumeContext<TSaga, T>> next)
        {
            SagaContext<TSaga, T> sagaContext;
            if (!context.TryGetPayload(out sagaContext))
                throw new SagaException("Failed to load saga context", typeof(TSaga), typeof(T));

            ISagaPolicy<TSaga, T> policy = sagaContext.Policy;
            Guid sagaId = sagaContext.Id;

            bool needToLeaveSagas = true;
            Monitor.Enter(_sagas);
            try
            {
                TSaga instance = _sagas[sagaId];
                if (instance == null)
                {
                    if (policy.CanCreateInstance(context))
                    {
                        instance = policy.CreateInstance(context, sagaId);
                        _sagas.Add(instance);

                        if (_log.IsDebugEnabled)
                        {
                            _log.DebugFormat("SAGA: {0} Created {1} for {2}", TypeMetadataCache<TSaga>.ShortName, instance.CorrelationId,
                                TypeMetadataCache<T>.ShortName);
                        }

                        Monitor.Enter(instance);
                        try
                        {
                            Monitor.Exit(_sagas);
                            needToLeaveSagas = false;

                            SagaConsumeContext<TSaga, T> sagaConsumeContext = new SagaConsumeContextProxy<TSaga, T>(context, instance);

                            await next.Send(sagaConsumeContext);

                            if (policy.CanRemoveInstance(instance))
                                _sagas.Remove(instance);
                        }
                        catch (SagaException sex)
                        {
                            if (_log.IsErrorEnabled)
                                _log.Error("Created Saga Exception", sex);
                            throw;
                        }
                        catch (Exception ex)
                        {
                            var sagaException = new SagaException("Created Saga Instance Exception", typeof(TSaga), typeof(T),
                                instance.CorrelationId, ex);
                            if (_log.IsErrorEnabled)
                                _log.Error("Created Saga Exception", sagaException);

                            throw sagaException;
                        }
                        finally
                        {
                            Monitor.Exit(instance);
                        }
                    }
                    else
                    {
                        if (_log.IsWarnEnabled)
                        {
                            _log.WarnFormat("SAGA: {0} Ignoring Missing {1} for {2}", TypeMetadataCache<TSaga>.ShortName, sagaId,
                                TypeMetadataCache<T>.ShortName);
                        }
                    }
                }
                else
                {
                    if (policy.CanUseExistingInstance(context))
                    {
                        Monitor.Enter(instance);
                        try
                        {
                            Monitor.Exit(_sagas);
                            needToLeaveSagas = false;

                            if (_log.IsDebugEnabled)
                            {
                                _log.DebugFormat("SAGA: {0} Existing {1} for {2}", TypeMetadataCache<TSaga>.ShortName,
                                    instance.CorrelationId,
                                    TypeMetadataCache<T>.ShortName);
                            }

                            SagaConsumeContext<TSaga, T> sagaConsumeContext = new SagaConsumeContextProxy<TSaga, T>(context, instance);

                            await next.Send(sagaConsumeContext);

                            if (policy.CanRemoveInstance(instance))
                                _sagas.Remove(instance);
                        }
                        catch (SagaException sex)
                        {
                            if (_log.IsErrorEnabled)
                                _log.Error("Existing Saga Exception", sex);
                            throw;
                        }
                        catch (Exception ex)
                        {
                            var sagaException = new SagaException("Existing Saga Instance Exception", typeof(TSaga), typeof(T),
                                instance.CorrelationId, ex);
                            if (_log.IsErrorEnabled)
                                _log.Error("Created Saga Exception", sagaException);

                            throw sagaException;
                        }
                        finally
                        {
                            Monitor.Exit(instance);
                        }
                    }
                    else
                    {
                        if (_log.IsWarnEnabled)
                        {
                            _log.WarnFormat("SAGA: {0} Ignoring Existing {1} for {2}", TypeMetadataCache<TSaga>.ShortName, sagaId,
                                TypeMetadataCache<T>.ShortName);
                        }
                    }
                }
            }
            finally
            {
                if (needToLeaveSagas)
                    Monitor.Exit(_sagas);
            }
        }

        async Task<IEnumerable<Guid>> ISagaRepository<TSaga>.Find(ISagaFilter<TSaga> filter)
        {
            return _sagas.Where(filter).Select(x => x.CorrelationId);
        }

        public void Add(TSaga newSaga)
        {
            lock (_sagas)
                _sagas.Add(newSaga);
        }

        public void Remove(TSaga saga)
        {
            lock (_sagas)
                _sagas.Remove(saga);
        }
    }
}