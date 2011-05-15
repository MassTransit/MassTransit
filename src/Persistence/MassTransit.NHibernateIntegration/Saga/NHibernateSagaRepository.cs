// Copyright 2007-2011 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
	using System.Linq.Expressions;
	using log4net;
	using MassTransit.Saga;
	using NHibernate;
	using NHibernate.Linq;
	using Util;

	public class NHibernateSagaRepository<T> :
		AbstractSagaRepository<T>,
		ISagaRepository<T>
		where T : class, ISaga
	{
		static readonly ILog _log = LogManager.GetLogger(typeof (NHibernateSagaRepository<T>).ToFriendlyName());
		volatile bool _disposed;
		ISessionFactory _sessionFactory;

		public NHibernateSagaRepository(ISessionFactory sessionFactory)
		{
			_sessionFactory = sessionFactory;
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public void Send<TMessage>(Expression<Func<T, bool>> filter, ISagaPolicy<T, TMessage> policy, TMessage message,
		                           Action<T> consumerAction)
			where TMessage : class
		{
			using (ISession session = _sessionFactory.OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				T[] existingSagas = session.Query<T>()
					.Where(filter).ToArray();

				bool foundExistingSagas = SendMessageToExistingSagas(existingSagas, policy, consumerAction, message, session.Delete);
				if (foundExistingSagas)
				{
					transaction.Commit();
					return;
				}

				SendMessageToNewSaga(policy, message, saga =>
					{
						consumerAction(saga);
						session.Save(saga);
					}, session.Delete);

				transaction.Commit();
			}
		}

		public IEnumerable<T> Where(Expression<Func<T, bool>> filter)
		{
			using (ISession session = _sessionFactory.OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				List<T> result = session.Query<T>().Where(filter).ToList();

				transaction.Commit();

				return result;
			}
		}

		public void Dispose(bool disposing)
		{
			if (_disposed) return;
			if (disposing)
			{
				_sessionFactory = null;
			}
			_disposed = true;
		}

		~NHibernateSagaRepository()
		{
			Dispose(false);
		}
	}
}