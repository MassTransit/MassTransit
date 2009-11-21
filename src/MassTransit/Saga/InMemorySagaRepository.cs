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
	using Util;

	public class InMemorySagaRepository<TSaga> :
		AbstractSagaRepository<TSaga>,
		ISagaRepository<TSaga>
		where TSaga : class, ISaga
	{
		private IndexedCollection<TSaga> _collection = new IndexedCollection<TSaga>();
		private bool _disposed;

		public void Send<TMessage>(Expression<Func<TSaga, bool>> filter, ISagaPolicy<TSaga, TMessage> policy, TMessage message, Action<TSaga> consumerAction)
			where TMessage : class
		{
			IEnumerable<TSaga> existingSagas = _collection.Where(filter).ToList();

			if (SendMessageToExistingSagas(existingSagas, policy, consumerAction, message, RemoveSaga))
				return;

			SendMessageToNewSaga(policy, message, saga =>
				{
					lock (_collection)
						_collection.Add(saga);

					consumerAction(saga);
				}, RemoveSaga);
		}

		public IEnumerable<TSaga> Where(Expression<Func<TSaga, bool>> filter)
		{
			lock (_collection)
			{
				return _collection.Where(filter).ToList();
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public void Add(TSaga newSaga)
		{
			lock (_collection)
				_collection.Add(newSaga);
		}

		private void RemoveSaga(TSaga saga)
		{
			lock (_collection)
				_collection.Remove(saga);
		}

		private void Dispose(bool disposing)
		{
			if (_disposed) return;
			if (disposing)
			{
				_collection = null;
			}

			_disposed = true;
		}

		~InMemorySagaRepository()
		{
			Dispose(false);
		}
	}
}