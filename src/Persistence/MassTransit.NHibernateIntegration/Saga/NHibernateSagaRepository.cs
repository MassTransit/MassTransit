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
namespace MassTransit.NHibernateIntegration.Saga
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Transactions;
    using Context;
    using Logging;
    using MassTransit.Saga;
    using NHibernate;
    using Pipeline;
    using Util;


    public class NHibernateSagaRepository<TSaga> :
        ISagaRepository<TSaga>
        where TSaga : class, ISaga
    {
        static readonly ILog _log = Logger.Get(TypeMetadataCache<NHibernateSagaRepository<TSaga>>.ShortName);

        readonly ISessionFactory _sessionFactory;

        public NHibernateSagaRepository(ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
        }

        async Task ISagaRepository<TSaga>.Send<T>(ConsumeContext<T> context, IPipe<SagaConsumeContext<TSaga, T>> next)
        {
            SagaContext<TSaga, T> sagaContext;
            if (!context.TryGetPayload(out sagaContext))
                throw new SagaException("Failed to load saga context", typeof(TSaga), typeof(T));

            ISagaPolicy<TSaga, T> policy = sagaContext.Policy;
            Guid sagaId = sagaContext.Id;

            using (ISession session = _sessionFactory.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                var instance = session.Get<TSaga>(sagaId, LockMode.Upgrade);
                if (instance == null)
                {
                    if (policy.CanCreateInstance(context))
                    {
                        instance = policy.CreateInstance(context, sagaId);

                        if (_log.IsDebugEnabled)
                        {
                            _log.DebugFormat("SAGA: {0} Created {1} for {2}", TypeMetadataCache<TSaga>.ShortName, instance.CorrelationId,
                                TypeMetadataCache<T>.ShortName);
                        }
                        try
                        {
                            SagaConsumeContext<TSaga, T> sagaConsumeContext = new SagaConsumeContextProxy<TSaga, T>(context, instance);

                            await next.Send(sagaConsumeContext);

                            if (!policy.CanRemoveInstance(instance))
                                session.Save(instance);
                        }
                        catch (SagaException sex)
                        {
                            if (_log.IsErrorEnabled)
                                _log.Error("Created Saga Exception", sex);

                            if (transaction.IsActive)
                                transaction.Rollback();
                            throw;
                        }
                        catch (Exception ex)
                        {
                            var sagaException = new SagaException("Created Saga Instance Exception", typeof(TSaga), typeof(T),
                                instance.CorrelationId, ex);
                            if (_log.IsErrorEnabled)
                                _log.Error("Created Saga Exception", sagaException);

                            if (transaction.IsActive)
                                transaction.Rollback();

                            throw sagaException;
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
                        try
                        {
                            if (_log.IsDebugEnabled)
                            {
                                _log.DebugFormat("SAGA: {0} Existing {1} for {2}", TypeMetadataCache<TSaga>.ShortName,
                                    instance.CorrelationId,
                                    TypeMetadataCache<T>.ShortName);
                            }

                            SagaConsumeContext<TSaga, T> sagaConsumeContext = new SagaConsumeContextProxy<TSaga, T>(context, instance);

                            await next.Send(sagaConsumeContext);

                            if (policy.CanRemoveInstance(instance))
                                session.Delete(instance);
                        }
                        catch (SagaException sex)
                        {
                            if (_log.IsErrorEnabled)
                                _log.Error("Existing Saga Exception", sex);
                            if (transaction.IsActive)
                                transaction.Rollback();
                            throw;
                        }
                        catch (Exception ex)
                        {
                            var sagaException = new SagaException("Existing Saga Instance Exception", typeof(TSaga), typeof(T),
                                instance.CorrelationId, ex);
                            if (_log.IsErrorEnabled)
                                _log.Error("Created Saga Exception", sagaException);

                            if (transaction.IsActive)
                                transaction.Rollback();
                            throw sagaException;
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

                if (transaction.IsActive)
                    transaction.Commit();
            }
        }

        async Task<IEnumerable<Guid>> ISagaRepository<TSaga>.Find(ISagaFilter<TSaga> filter)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew))
            using (ISession session = _sessionFactory.OpenSession())
            {
                IList<Guid> result = session.QueryOver<TSaga>()
                    .Where(filter.FilterExpression)
                    .Select(x => x.CorrelationId)
                    .List<Guid>();

                scope.Complete();

                return result;
            }
        }
    }
}