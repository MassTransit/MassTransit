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
namespace MassTransit.Saga
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Linq.Expressions;
	using log4net;
	using Magnum.Data;

	public class InMemorySagaRepository<T> :
		ISagaRepository<T>
		where T : class, ISaga
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof (InMemorySagaRepository<T>).ToFriendlyName());
		private IRepository<T> _repository;

		public InMemorySagaRepository()
		{
			_log.InfoFormat("Creating saga repository for {0}", typeof (T).FullName);

			_repository = new InMemoryRepository<T, Guid>(x => x.CorrelationId);
		}

		public void Dispose()
		{
			_repository.Dispose();
			_repository = null;
		}

		public IEnumerable<Action<V>> Create<V>(Guid sagaId, Action<T, V> action)
		{
			T saga = (T) Activator.CreateInstance(typeof (T), new object[] {sagaId});

			_repository.Save(saga);

			lock (saga)
			{
				yield return x => action(saga, x);
			}
		}

		public IEnumerable<Action<V>> Find<V>(Expression<Func<T, bool>> expression, Action<T, V> action)
		{
			foreach (T saga in _repository.Where(expression))
			{
				T consumer = saga;
				lock (consumer)
				{
					yield return x => action(consumer, x);
				}
			}
		}

		public IEnumerable<Action> Find(Expression<Func<T, bool>> expression, Action<T> action)
		{
			foreach (T saga in _repository.Where(expression))
			{
				T consumer = saga;
				lock (consumer)
				{
					yield return () => action(consumer);
				}
			}
		}

		public IEnumerable<T> Where(Expression<Func<T, bool>> filter)
		{
			return _repository.Where(filter);
		}
	}
}