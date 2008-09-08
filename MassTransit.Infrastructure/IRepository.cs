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
namespace MassTransit.Infrastructure
{
	using System;
	using System.Linq;

	public interface IRepository :
		IDisposable
	{
		T Get<T>(object id);

		void Save<T>(T item);

		void Delete<T>(T item);
	}

	public interface IRepository<T, K> :
		IQueryable<T>,
		IDisposable
		where T : IAggregateRoot<K>
	{
		T Get(K id);

		void Save(T item);

		void Delete(T item);
	}

	public interface IRepository<T> :
		IRepository<T, Guid>
		where T : IAggregateRoot<Guid>
	{
	}
}