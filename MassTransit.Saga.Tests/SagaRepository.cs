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
namespace MassTransit.Saga.Tests
{
	using System;
	using Magnum.Common;

	public class SagaRepository<T> :
		ISagaRepository<T>
		where T : class, IAggregateRoot
	{
		private readonly IRepository<T, Guid> _repository;

		public SagaRepository(IRepository<T, Guid> repository)
		{
			_repository = repository;
		}

		public T Create(Guid correlationId)
		{
			T saga = (T) Activator.CreateInstance(typeof (T), correlationId);

			_repository.Save(saga);

			return saga;
		}

		public T Get(Guid id)
		{
			return _repository.Get(id);
		}

		public void Save(T item)
		{
			_repository.Save(item);
		}

		public void Update(T item)
		{
			_repository.Save(item);
		}

		public void Complete(T item)
		{
			_repository.Save(item);
		}

		public void Dispose()
		{
			_repository.Dispose();
		}
	}
}