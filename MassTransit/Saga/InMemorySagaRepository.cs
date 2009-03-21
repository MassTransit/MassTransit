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
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using Exceptions;
	using log4net;
	using Magnum.Threading;

	public class InMemorySagaRepository<T> :
		ISagaRepository<T>
		where T : class, ISaga
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof(InMemorySagaRepository<T>).ToFriendlyName());

		private readonly ReaderWriterLockedDictionary<Guid, T> _sagas = new ReaderWriterLockedDictionary<Guid, T>();

		public InMemorySagaRepository()
		{
			_log.InfoFormat("Creating saga repository for {0}", typeof (T).FullName);
		}

		public void Dispose()
		{
			_sagas.Clear();
		}

		public IEnumerator<T> InitiateNewSaga(Guid id)
		{
			T saga;
			if(_sagas.TryGetValue(id, out saga))
				throw new SagaException("The saga already exists and cannot be initiated", typeof (T), typeof (T), id);

			if (_log.IsDebugEnabled)
				_log.DebugFormat("Creating saga [{0}] - {1}", typeof(T).FullName, id);

			saga = (T) Activator.CreateInstance(typeof (T), new object[] {id});

			_sagas.Add(id, saga);

			yield return saga;
		}

		public IEnumerator<T> OrchestrateExistingSaga(Guid id)
		{
			if(_log.IsDebugEnabled)
				_log.DebugFormat("Loading saga [{0}] - {1}", typeof (T).FullName, id);

			T saga;
			if (!_sagas.TryGetValue(id, out saga))
				throw new SagaException("The saga was not found and cannot be loaded", typeof (T), typeof (T), id);

			yield return saga;
		}

		public IEnumerator<T> GetEnumerator()
		{
			return _sagas.Values.ToList().GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}