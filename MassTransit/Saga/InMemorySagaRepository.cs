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
	using Exceptions;
	using log4net;

	public class InMemorySagaRepository<T> :
		ISagaRepository<T>
		where T : class, ISaga
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof(InMemorySagaRepository<T>));

		private readonly Dictionary<Guid, T> _sagas = new Dictionary<Guid, T>();

		public InMemorySagaRepository()
		{
			_log.Info("Creating new saga repository");
		}

		public T Create(Guid correlationId)
		{
			if (_sagas.ContainsKey(correlationId))
				throw new SagaException("The saga already exists and cannot be initiated", typeof (T), typeof (T), correlationId);

			_log.InfoFormat("Creating [{0}] with id {1}", typeof (T).Name, correlationId);

			T saga = (T) Activator.CreateInstance(typeof (T), new object[] { correlationId });

			_sagas.Add(saga.CorrelationId, saga);

			return saga;
		}

		T ISagaRepository<T>.Get(Guid id)
		{
			if (!_sagas.ContainsKey(id))
				throw new SagaException("The saga was not found and cannot be loaded", typeof(T), typeof(T), id);

			return _sagas[id];
		}

		public void Save(T item)
		{
			if (!_sagas.ContainsKey(item.CorrelationId))
				throw new SagaException("The saga was not found and cannot be saved", typeof(T), typeof(T), item.CorrelationId);

			_sagas[item.CorrelationId] = item;
		}

		public void Dispose()
		{
			_sagas.Clear();
		}
	}
}