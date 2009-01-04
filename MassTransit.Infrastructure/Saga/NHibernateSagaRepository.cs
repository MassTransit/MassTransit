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
	using Magnum.Infrastructure.Data;
	using MassTransit.Saga;

	public class NHibernateSagaRepository<T> :
		ISagaRepository<T>
		where T : class, ISaga
	{
		public void Dispose()
		{
		}

		public T Create(Guid correlationId)
		{
			using (var repository = new NHibernateRepository())
			{
				T saga = (T) Activator.CreateInstance(typeof (T), new object[] {correlationId});

				repository.Save(saga);

				return saga;
			}
		}

		public T Get(Guid id)
		{
			using (var repository = new NHibernateRepository())
				return repository.Get<T>(id);
		}

		public void Save(T item)
		{
			using (var repository = new NHibernateRepository())
				repository.Update(item);
		}
	}
}