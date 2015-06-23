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
namespace MassTransit.NHibernateIntegration.Saga
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Transactions;
    using Logging;
    using MassTransit.Saga;
    using Monitoring.Introspection;
    using NHibernate;
    using Pipeline;
    using Util;


    public class NHibernateSagaRepository<TSaga> :
        ISagaRepository<TSaga>,
        IQuerySagaRepository<TSaga>
        where TSaga : class, ISaga
    {
        static readonly ILog _log = Logger.Get<NHibernateSagaRepository<TSaga>>();

        readonly ISessionFactory _sessionFactory;

        public NHibernateSagaRepository(ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
        }

        public async Task<IEnumerable<Guid>> Find(ISagaQuery<TSaga> query)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew))
            using (ISession session = _sessionFactory.OpenSession())
            {
                IList<Guid> result = session.QueryOver<TSaga>()
                    .Where(query.FilterExpression)
                    .Select(x => x.CorrelationId)
                    .List<Guid>();

                scope.Complete();

                return result;
            }
        }

        async Task IProbeSite.Probe(ProbeContext context)
        {
            ProbeContext scope = context.CreateScope("sagaRepository");
            scope.Set(new
            {
                Persistence = "nhibernate",
                Entities = _sessionFactory.GetAllClassMetadata().Select(x => x.Value.EntityName).ToArray(),
            });
        }

        public async Task Send<T>(ConsumeContext<T> context, ISagaPolicy<TSaga, T> policy, IPipe<SagaConsumeContext<TSaga, T>> next) where T : class
        {
            if (!context.CorrelationId.HasValue)
                throw new SagaException("The CorrelationId was not specified", typeof(TSaga), typeof(T));

            Guid sagaId = context.CorrelationId.Value;

            using (ISession session = _sessionFactory.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                try
                {
                    var instance = session.Get<TSaga>(sagaId, LockMode.Upgrade);
                    if (instance == null)
                    {
                        var missingSagaPipe = new MissingPipe<T>(session, next);

                        await policy.Missing(context, missingSagaPipe);
                    }
                    else
                    {
                        if (_log.IsDebugEnabled)
                            _log.DebugFormat("SAGA:{0}:{1} Used {2}", TypeMetadataCache<TSaga>.ShortName, instance.CorrelationId, TypeMetadataCache<T>.ShortName);

                        var sagaConsumeContext = new NHibernateSagaConsumeContext<TSaga, T>(session, context, instance);

                        await policy.Existing(sagaConsumeContext, next);
                    }


                    if (transaction.IsActive)
                        transaction.Commit();
                }
                catch (Exception)
                {
                    if (transaction.IsActive)
                        transaction.Rollback();

                    throw;
                }
            }
        }

        public async Task SendQuery<T>(SagaQueryConsumeContext<TSaga, T> context, ISagaPolicy<TSaga, T> policy, IPipe<SagaConsumeContext<TSaga, T>> next)
            where T : class
        {
            using (ISession session = _sessionFactory.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                try
                {
                    IList<TSaga> instances = session.QueryOver<TSaga>()
                        .Where(context.Query.FilterExpression)
                        .List<TSaga>();

                    if (instances.Count == 0)
                    {
                        var missingSagaPipe = new MissingPipe<T>(session, next);
                        await policy.Missing(context, missingSagaPipe);
                    }
                    else
                        await Task.WhenAll(instances.Select(instance => SendToInstance(context, policy, instance, next, session)));

                    // TODO partial failure should not affect them all

                    if (transaction.IsActive)
                        transaction.Commit();
                }
                catch (SagaException sex)
                {
                    if (_log.IsErrorEnabled)
                        _log.Error("Saga Exception Occurred", sex);
                }
                catch (Exception ex)
                {
                    if (_log.IsErrorEnabled)
                        _log.Error(string.Format("SAGA:{0} Exception {1}", TypeMetadataCache<TSaga>.ShortName, TypeMetadataCache<T>.ShortName), ex);

                    if (transaction.IsActive)
                        transaction.Rollback();

                    throw new SagaException(ex.Message, typeof(TSaga), typeof(T), Guid.Empty, ex);
                }
            }
        }

        async Task SendToInstance<T>(SagaQueryConsumeContext<TSaga, T> context, ISagaPolicy<TSaga, T> policy, TSaga instance,
            IPipe<SagaConsumeContext<TSaga, T>> next, ISession session)
            where T : class
        {
            try
            {
                if (_log.IsDebugEnabled)
                    _log.DebugFormat("SAGA:{0}:{1} Used {2}", TypeMetadataCache<TSaga>.ShortName, instance.CorrelationId, TypeMetadataCache<T>.ShortName);

                var sagaConsumeContext = new NHibernateSagaConsumeContext<TSaga, T>(session, context, instance);

                await policy.Existing(sagaConsumeContext, next);
            }
            catch (SagaException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SagaException(ex.Message, typeof(TSaga), typeof(T), instance.CorrelationId, ex);
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
            readonly ISession _session;

            public MissingPipe(ISession session, IPipe<SagaConsumeContext<TSaga, TMessage>> next)
            {
                _session = session;
                _next = next;
            }

            Task IProbeSite.Probe(ProbeContext context)
            {
                return _next.Probe(context);
            }

            public async Task Send(SagaConsumeContext<TSaga, TMessage> context)
            {
                if (_log.IsDebugEnabled)
                {
                    _log.DebugFormat("SAGA:{0}:{1} Added {2}", TypeMetadataCache<TSaga>.ShortName, context.Saga.CorrelationId,
                        TypeMetadataCache<TMessage>.ShortName);
                }

                var proxy = new NHibernateSagaConsumeContext<TSaga, TMessage>(_session, context, context.Saga);

                await _next.Send(proxy);

                if (!proxy.IsCompleted)
                    _session.Save(context.Saga);
            }
        }
    }
}