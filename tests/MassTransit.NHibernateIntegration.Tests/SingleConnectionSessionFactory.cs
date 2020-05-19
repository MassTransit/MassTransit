// Copyright 2007-2012 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.NHibernateIntegration.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Data;
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

        public void Dispose() => _inner.Dispose();

        public ISession OpenSession(DbConnection conn) => _inner.WithOptions().Connection(_liveConnection).OpenSession();

        public Task CloseAsync(CancellationToken cancellationToken = new CancellationToken()) => 
            _inner.CloseAsync(cancellationToken);

        public Task EvictAsync(Type persistentClass, CancellationToken cancellationToken = new CancellationToken()) => 
            _inner.EvictAsync(persistentClass, cancellationToken);

        public Task EvictAsync(Type persistentClass, object id, CancellationToken cancellationToken = new CancellationToken()) => 
            _inner.EvictAsync(persistentClass, id, cancellationToken);

        public Task EvictEntityAsync(string entityName, CancellationToken cancellationToken = new CancellationToken()) => 
            _inner.EvictEntityAsync(entityName, cancellationToken);

        public Task EvictEntityAsync(string entityName, object id, CancellationToken cancellationToken = new CancellationToken()) => 
            _inner.EvictEntityAsync(entityName, id, cancellationToken);

        public Task EvictCollectionAsync(string roleName, CancellationToken cancellationToken = new CancellationToken()) => 
            _inner.EvictCollectionAsync(roleName, cancellationToken);

        public Task EvictCollectionAsync(string roleName, object id, CancellationToken cancellationToken = new CancellationToken()) => 
            _inner.EvictCollectionAsync(roleName, id, cancellationToken);

        public Task EvictQueriesAsync(CancellationToken cancellationToken = new CancellationToken()) => 
            _inner.EvictQueriesAsync(cancellationToken);

        public Task EvictQueriesAsync(string cacheRegion, CancellationToken cancellationToken = new CancellationToken()) => 
            _inner.EvictQueriesAsync(cacheRegion, cancellationToken);

        public ISessionBuilder WithOptions() => _inner.WithOptions();

        public ISession OpenSession(IInterceptor sessionLocalInterceptor) => 
            _inner.WithOptions().Connection(_liveConnection).Interceptor(sessionLocalInterceptor).OpenSession();

        public ISession OpenSession(DbConnection conn, IInterceptor sessionLocalInterceptor) => 
            _inner.WithOptions().Connection(_liveConnection).Interceptor(sessionLocalInterceptor).OpenSession();

        public ISession OpenSession() => _inner.WithOptions().Connection(_liveConnection).OpenSession();

        public IStatelessSessionBuilder WithStatelessOptions()
        {
            throw new NotImplementedException();
        }

        public IClassMetadata GetClassMetadata(Type persistentClass) => 
            _inner.GetClassMetadata(persistentClass);

        public IClassMetadata GetClassMetadata(string entityName) => 
            _inner.GetClassMetadata(entityName);

        public ICollectionMetadata GetCollectionMetadata(string roleName) => 
            _inner.GetCollectionMetadata(roleName);

        public IDictionary<string, IClassMetadata> GetAllClassMetadata() => 
            _inner.GetAllClassMetadata();

        public IDictionary<string, ICollectionMetadata> GetAllCollectionMetadata() => 
            _inner.GetAllCollectionMetadata();

        public void Close() => _inner.Close();

        public void Evict(Type persistentClass) => _inner.Evict(persistentClass);

        public void Evict(Type persistentClass, object id) => _inner.Evict(persistentClass, id);

        public void EvictEntity(string entityName) => _inner.EvictEntity(entityName);

        public void EvictEntity(string entityName, object id) => _inner.EvictEntity(entityName, id);

        public void EvictCollection(string roleName) => _inner.EvictCollection(roleName);

        public void EvictCollection(string roleName, object id) => _inner.EvictCollection(roleName, id);

        public void EvictQueries() => _inner.EvictQueries();

        public void EvictQueries(string cacheRegion) => _inner.EvictQueries(cacheRegion);

        public IStatelessSession OpenStatelessSession() => _inner.OpenStatelessSession();

        public IStatelessSession OpenStatelessSession(DbConnection connection) => 
            _inner.OpenStatelessSession(connection);

        public FilterDefinition GetFilterDefinition(string filterName) => 
            _inner.GetFilterDefinition(filterName);

        public ISession GetCurrentSession() => _inner.GetCurrentSession();

        public IStatistics Statistics => _inner.Statistics;

        public bool IsClosed => _inner.IsClosed;

        public ICollection<string> DefinedFilterNames => _inner.DefinedFilterNames;
    }
}