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
    using NHibernate;
    using NHibernate.Engine;
    using NHibernate.Metadata;
    using NHibernate.Stat;

    class SingleConnectionSessionFactory : 
        ISessionFactory
    {
        readonly ISessionFactory _inner;
        readonly IDbConnection _liveConnection;

        public SingleConnectionSessionFactory(ISessionFactory inner, IDbConnection liveConnection)
        {
            _inner = inner;
            _liveConnection = liveConnection;
        }

        public void Dispose()
        {
            _inner.Dispose();
        }

        public ISession OpenSession(IDbConnection conn)
        {
            return _inner.OpenSession(_liveConnection);
        }

        public ISession OpenSession(IInterceptor sessionLocalInterceptor)
        {
            return _inner.OpenSession(_liveConnection, sessionLocalInterceptor);
        }

        public ISession OpenSession(IDbConnection conn, IInterceptor sessionLocalInterceptor)
        {
            return _inner.OpenSession(_liveConnection, sessionLocalInterceptor);
        }

        public ISession OpenSession()
        {
            return _inner.OpenSession(_liveConnection);
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

        public IStatelessSession OpenStatelessSession(IDbConnection connection)
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

        public IStatistics Statistics
        {
            get { return _inner.Statistics; }
        }

        public bool IsClosed
        {
            get { return _inner.IsClosed; }
        }

        public ICollection<string> DefinedFilterNames
        {
            get { return _inner.DefinedFilterNames; }
        }
    }
}