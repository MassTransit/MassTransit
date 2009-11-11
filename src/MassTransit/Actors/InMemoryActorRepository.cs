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
namespace MassTransit.Actors
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Linq.Expressions;
	using Saga;
	using Util;

	public class InMemoryActorRepository<TSaga> :
		AbstractSagaRepository<TSaga>,
		IActorRepository<TSaga>
		where TSaga : class, ISaga
	{
		private readonly IndexedCollection<TSaga> _collection = new IndexedCollection<TSaga>();
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

		private void RemoveSaga(TSaga saga)
		{
			lock (_collection)
				_collection.Remove(saga);
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

		public void Add(TSaga newItem)
		{
			lock (_collection)
				_collection.Add(newItem);
		}

		public void Remove(TSaga item)
		{
			lock (_collection)
				_collection.Remove(item);
		}

		public int Count()
		{
			return _collection.Count;
		}

		private void Dispose(bool disposing)
		{
			if (_disposed) return;
			if (disposing)
			{
				_collection.Clear();
			}

			_disposed = true;
		}

		~InMemoryActorRepository()
		{
			Dispose(false);
		}
	}
}