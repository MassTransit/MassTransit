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
namespace MassTransit.Infrastructure.Saga
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Linq;
	using System.Linq.Expressions;
	using MassTransit.Saga;
	using NHibernate;
	using NHibernate.Linq;

	public class NHibernateSagaRepositoryForContainers<T> :
		ISagaRepository<T>
		where T : class, ISaga
	{
		private readonly ISessionFactory _sessionFactory;
		private ISession _session;

		public NHibernateSagaRepositoryForContainers(ISessionFactory sessionFactory)
		{
			_sessionFactory = sessionFactory;
			_session = _sessionFactory.OpenSession();
		}

		public void Dispose()
		{
			_session.Dispose();
		}

		public IEnumerable<Action<V>> Find<V>(Expression<Func<T, bool>> expression, Action<T, V> action)
		{
			foreach (T saga in _session.Linq<T>().Where(expression))
			{
				using (var transaction = _session.BeginTransaction(IsolationLevel.Serializable))
				{
					T lockedSaga = _session.Load<T>(saga.CorrelationId, LockMode.Upgrade);

					yield return x => action(lockedSaga, x);

					_session.Update(lockedSaga);

					transaction.Commit();
				}
			}
		}

		public IEnumerable<Action> Find(Expression<Func<T, bool>> expression, Action<T> action)
		{
			foreach (var item in Find<int>(expression, (s, m) => action(s)))
			{
				Action<int> actionItem = item;

				yield return () => actionItem(0);
			}
		}

		public IEnumerable<T> Where(Expression<Func<T, bool>> filter)
		{
			return _session.Linq<T>().Where(filter);
		}

		public IEnumerable<Action<V>> Create<V>(Guid sagaId, Action<T, V> action)
		{
			using (var transaction = _session.BeginTransaction(IsolationLevel.Serializable))
			{
				T saga = (T) Activator.CreateInstance(typeof (T), sagaId);

				_session.Save(saga);

				yield return x => action(saga, x);

				_session.Update(saga);

				transaction.Commit();
			}
		}
	}
}