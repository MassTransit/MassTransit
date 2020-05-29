namespace MassTransit.NHibernateIntegration.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Threading;
    using System.Threading.Tasks;
    using NHibernate;
    using NHibernate.Engine;
    using NHibernate.Metadata;
    using NHibernate.Stat;


    class SingleConnectionSessionFactory :
        ISessionFactory
    {
        readonly ISessionFactory _inner;
        readonly DbConnection _liveConnection;

        public SingleConnectionSessionFactory(ISessionFactory inner, DbConnection liveConnection)
        {
            _inner = inner;
            _liveConnection = liveConnection;
        }

        public void Dispose()
        {
            _inner.Dispose();
        }

        public ISession OpenSession(DbConnection conn)
        {
            return _inner.WithOptions().Connection(_liveConnection).OpenSession();
        }

        public Task CloseAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            return _inner.CloseAsync(cancellationToken);
        }

        public Task EvictAsync(Type persistentClass, CancellationToken cancellationToken = new CancellationToken())
        {
            return _inner.EvictAsync(persistentClass, cancellationToken);
        }

        public Task EvictAsync(Type persistentClass, object id, CancellationToken cancellationToken = new CancellationToken())
        {
            return _inner.EvictAsync(persistentClass, id, cancellationToken);
        }

        public Task EvictEntityAsync(string entityName, CancellationToken cancellationToken = new CancellationToken())
        {
            return _inner.EvictEntityAsync(entityName, cancellationToken);
        }

        public Task EvictEntityAsync(string entityName, object id, CancellationToken cancellationToken = new CancellationToken())
        {
            return _inner.EvictEntityAsync(entityName, id, cancellationToken);
        }

        public Task EvictCollectionAsync(string roleName, CancellationToken cancellationToken = new CancellationToken())
        {
            return _inner.EvictCollectionAsync(roleName, cancellationToken);
        }

        public Task EvictCollectionAsync(string roleName, object id, CancellationToken cancellationToken = new CancellationToken())
        {
            return _inner.EvictCollectionAsync(roleName, id, cancellationToken);
        }

        public Task EvictQueriesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            return _inner.EvictQueriesAsync(cancellationToken);
        }

        public Task EvictQueriesAsync(string cacheRegion, CancellationToken cancellationToken = new CancellationToken())
        {
            return _inner.EvictQueriesAsync(cacheRegion, cancellationToken);
        }

        public ISessionBuilder WithOptions()
        {
            return _inner.WithOptions();
        }

        public ISession OpenSession(IInterceptor sessionLocalInterceptor)
        {
            return _inner.WithOptions().Connection(_liveConnection).Interceptor(sessionLocalInterceptor).OpenSession();
        }

        public ISession OpenSession(DbConnection conn, IInterceptor sessionLocalInterceptor)
        {
            return _inner.WithOptions().Connection(_liveConnection).Interceptor(sessionLocalInterceptor).OpenSession();
        }

        public ISession OpenSession()
        {
            return _inner.WithOptions().Connection(_liveConnection).OpenSession();
        }

        public IStatelessSessionBuilder WithStatelessOptions()
        {
            throw new NotImplementedException();
        }

        public IClassMetadata GetClassMetadata(Type persistentClass)
        {
            return _inner.GetClassMetadata(persistentClass);
        }

        public IClassMetadata GetClassMetadata(string entityName)
        {
            return _inner.GetClassMetadata(entityName);
        }

        public ICollectionMetadata GetCollectionMetadata(string roleName)
        {
            return _inner.GetCollectionMetadata(roleName);
        }

        public IDictionary<string, IClassMetadata> GetAllClassMetadata()
        {
            return _inner.GetAllClassMetadata();
        }

        public IDictionary<string, ICollectionMetadata> GetAllCollectionMetadata()
        {
            return _inner.GetAllCollectionMetadata();
        }

        public void Close()
        {
            _inner.Close();
        }

        public void Evict(Type persistentClass)
        {
            _inner.Evict(persistentClass);
        }

        public void Evict(Type persistentClass, object id)
        {
            _inner.Evict(persistentClass, id);
        }

        public void EvictEntity(string entityName)
        {
            _inner.EvictEntity(entityName);
        }

        public void EvictEntity(string entityName, object id)
        {
            _inner.EvictEntity(entityName, id);
        }

        public void EvictCollection(string roleName)
        {
            _inner.EvictCollection(roleName);
        }

        public void EvictCollection(string roleName, object id)
        {
            _inner.EvictCollection(roleName, id);
        }

        public void EvictQueries()
        {
            _inner.EvictQueries();
        }

        public void EvictQueries(string cacheRegion)
        {
            _inner.EvictQueries(cacheRegion);
        }

        public IStatelessSession OpenStatelessSession()
        {
            return _inner.OpenStatelessSession();
        }

        public IStatelessSession OpenStatelessSession(DbConnection connection)
        {
            return _inner.OpenStatelessSession(connection);
        }

        public FilterDefinition GetFilterDefinition(string filterName)
        {
            return _inner.GetFilterDefinition(filterName);
        }

        public ISession GetCurrentSession()
        {
            return _inner.GetCurrentSession();
        }

        public IStatistics Statistics => _inner.Statistics;

        public bool IsClosed => _inner.IsClosed;

        public ICollection<string> DefinedFilterNames => _inner.DefinedFilterNames;
    }
}
