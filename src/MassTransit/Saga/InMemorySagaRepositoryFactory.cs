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
namespace MassTransit.Saga
{
	using System;
	using System.Collections.Generic;

	public class InMemorySagaRepositoryFactory :
		ISagaRepositoryFactory
	{
		readonly object _lock;
		readonly IDictionary<Type, object> _repositories;

		public InMemorySagaRepositoryFactory()
		{
			_lock = new object();
			_repositories = new Dictionary<Type, object>();
		}

		public ISagaRepository<T> GetRepository<T>()
			where T : class, ISaga
		{
			lock (_lock)
			{
				object existing;
				if (_repositories.TryGetValue(typeof (T), out existing))
					return (ISagaRepository<T>) existing;

				var repository = new InMemorySagaRepository<T>();
				_repositories.Add(typeof (T), repository);

				return repository;
			}
		}
	}
}