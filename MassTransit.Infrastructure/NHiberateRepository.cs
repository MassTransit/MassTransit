// Copyright 2007-2008 The Apache Software Foundation.
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
namespace MassTransit.Infrastructure
{
	using System;
	using System.Linq;
	using NHibernate;
	using NHibernate.Linq;

	public class NHibernateRepository :
		IRepository
	{
		private readonly ISession _session;

        public NHibernateRepository(ISessionFactory sessionFactory)
		{
			_session = sessionFactory.OpenSession();
			_session.FlushMode = FlushMode.Commit;
		}

		public void Dispose()
		{
			_session.Dispose();
		}

		public T Get<T>(object id)
		{
			return _session.Get<T>(id);
		}

		public void Save<T>(T item)
		{
			_session.WithinTransaction(x => x.SaveOrUpdate(item));
		}

		public void Delete<T>(T item)
		{
			_session.WithinTransaction(x => x.Delete(item));
		}
	}

	public static class NHibernateExtensions
	{
		public static void WithinTransaction(this ISession session, Action<ISession> action)
		{
			ITransaction transaction = session.BeginTransaction();
			try
			{
				action(session);
				transaction.Commit();
			}
			catch (Exception)
			{
				transaction.Rollback();
				throw;
			}
			finally
			{
				transaction.Dispose();
			}
		}
	}

	public class NHibernateRepository<T, K> :
		RepositoryBase<T, K>
		where T : IAggregateRoot<K>
	{
		private readonly ISession _session;

		public NHibernateRepository(ISession session)
		{
			_session = session;
			_session.FlushMode = FlushMode.Commit;
		}

		protected override IQueryable<T> RepositoryQuery
		{
			get { return _session.Linq<T>(); }
		}

		public override T Get(K id)
		{
			return _session.Get<T>(id);
		}

		public override void Save(T item)
		{
			_session.WithinTransaction(x => x.SaveOrUpdate(item));
		}

		public override void Delete(T item)
		{
			_session.WithinTransaction(x => x.Delete(item));
		}
	}
}