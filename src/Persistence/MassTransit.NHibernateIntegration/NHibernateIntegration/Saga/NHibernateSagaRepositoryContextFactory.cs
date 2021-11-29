namespace MassTransit.NHibernateIntegration.Saga
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using MassTransit.Saga;
    using NHibernate;


    public class NHibernateSagaRepositoryContextFactory<TSaga> :
        ISagaRepositoryContextFactory<TSaga>
        where TSaga : class, ISaga
    {
        readonly ISagaConsumeContextFactory<ISession, TSaga> _factory;
        readonly ISessionFactory _sessionFactory;

        public NHibernateSagaRepositoryContextFactory(ISessionFactory sessionFactory, ISagaConsumeContextFactory<ISession, TSaga> factory)
        {
            _sessionFactory = sessionFactory;
            _factory = factory;
        }

        public async Task Send<T>(ConsumeContext<T> context, IPipe<SagaRepositoryContext<TSaga, T>> next)
            where T : class
        {
            var session = _sessionFactory.OpenSession();

            ITransaction transaction = null;
            try
            {
                transaction = session.BeginTransaction();

                var sagaRepositoryContext = new NHibernateSagaRepositoryContext<TSaga, T>(session, context, _factory);

                await next.Send(sagaRepositoryContext).ConfigureAwait(false);

                await transaction.CommitAsync(sagaRepositoryContext.CancellationToken).ConfigureAwait(false);
            }
            catch (Exception)
            {
                if (transaction != null && transaction.IsActive)
                {
                    try
                    {
                        await transaction.RollbackAsync(context.CancellationToken).ConfigureAwait(false);
                    }
                    catch (Exception rollbackException)
                    {
                        LogContext.Warning?.Log(rollbackException, "Failed to rollback transaction");
                    }
                }

                throw;
            }
            finally
            {
                transaction?.Dispose();

                session.Dispose();
            }
        }

        public async Task SendQuery<T>(ConsumeContext<T> context, ISagaQuery<TSaga> query, IPipe<SagaRepositoryQueryContext<TSaga, T>> next)
            where T : class
        {
            var session = _sessionFactory.OpenSession();

            ITransaction transaction = null;
            try
            {
                transaction = session.BeginTransaction();

                var repositoryContext = new NHibernateSagaRepositoryContext<TSaga, T>(session, context, _factory);

                IList<TSaga> instances = await session.QueryOver<TSaga>()
                    .Where(query.FilterExpression)
                    .ListAsync(repositoryContext.CancellationToken)
                    .ConfigureAwait(false);

                var queryContext = new LoadedSagaRepositoryQueryContext<TSaga, T>(repositoryContext, instances);

                await next.Send(queryContext).ConfigureAwait(false);

                await transaction.CommitAsync(repositoryContext.CancellationToken).ConfigureAwait(false);
            }
            catch (Exception)
            {
                if (transaction != null && transaction.IsActive)
                {
                    try
                    {
                        await transaction.RollbackAsync(context.CancellationToken).ConfigureAwait(false);
                    }
                    catch (Exception rollbackException)
                    {
                        LogContext.Warning?.Log(rollbackException, "Failed to rollback transaction");
                    }
                }

                throw;
            }
            finally
            {
                transaction?.Dispose();

                session.Dispose();
            }
        }

        public async Task<T> Execute<T>(Func<SagaRepositoryContext<TSaga>, Task<T>> asyncMethod, CancellationToken cancellationToken)
            where T : class
        {
            var session = _sessionFactory.OpenSession();
            try
            {
                var repositoryContext = new NHibernateSagaRepositoryContext<TSaga>(session, cancellationToken);

                return await asyncMethod(repositoryContext).ConfigureAwait(false);
            }
            finally
            {
                session.Dispose();
            }
        }

        public void Probe(ProbeContext context)
        {
            context.Add("persistence", "nhibernate");
            context.Add("entities", _sessionFactory.GetAllClassMetadata().Select(x => x.Value.EntityName).ToArray());
        }
    }
}
