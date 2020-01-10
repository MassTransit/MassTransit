namespace MassTransit.NHibernateIntegration.Saga
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;
    using MassTransit.Saga;
    using NHibernate;


    public class NHibernateSagaRepositoryContextFactory<TSaga> :
        ISagaRepositoryContextFactory<TSaga>
        where TSaga : class, ISaga
    {
        readonly ISagaConsumeContextFactory<NHibernateContext, TSaga> _factory;
        readonly ISessionFactory _sessionFactory;

        public NHibernateSagaRepositoryContextFactory(ISessionFactory sessionFactory, ISagaConsumeContextFactory<NHibernateContext, TSaga> factory)
        {
            _sessionFactory = sessionFactory;
            _factory = factory;
        }

        public async Task<SagaRepositoryContext<TSaga, T>> CreateContext<T>(ConsumeContext<T> context, Guid? correlationId = default)
            where T : class
        {
            var session = _sessionFactory.OpenSession();
            try
            {
                var transaction = session.BeginTransaction();

                return new NHibernateSagaRepositoryContext<TSaga, T>(new Context(session, transaction), context, _factory);
            }
            catch (Exception)
            {
                session.Dispose();

                throw;
            }
        }

        public async Task<SagaRepositoryContext<TSaga>> CreateContext(CancellationToken cancellationToken = default, Guid? correlationId = default)
        {
            var session = _sessionFactory.OpenSession();
            try
            {
                return new NHibernateSagaRepositoryContext<TSaga>(new Context(session, default));
            }
            catch (Exception)
            {
                session.Dispose();

                throw;
            }
        }

        public void Probe(ProbeContext context)
        {
            context.Add("persistence", "nhibernate");
            context.Add("entities", _sessionFactory.GetAllClassMetadata().Select(x => x.Value.EntityName).ToArray());
        }


        class Context :
            NHibernateContext
        {
            public Context(ISession session, ITransaction transaction)
            {
                Session = session;
                Transaction = transaction;
            }

            public ISession Session { get; }
            public ITransaction Transaction { get; }
        }
    }
}
