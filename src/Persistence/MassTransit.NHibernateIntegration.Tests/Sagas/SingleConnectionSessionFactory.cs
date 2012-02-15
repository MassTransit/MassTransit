using System;
using System.Collections.Generic;
using System.Data;
using NHibernate;
using NHibernate.Engine;
using NHibernate.Metadata;
using NHibernate.Stat;

namespace MassTransit.NHibernateIntegration.Tests.Sagas
{
	class SingleConnectionSessionFactory : ISessionFactory
	{
		private readonly ISessionFactory _inner;
		private readonly IDbConnection _liveConnection;

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