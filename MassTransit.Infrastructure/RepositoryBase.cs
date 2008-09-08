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
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using System.Linq.Expressions;

	public abstract class RepositoryBase<T, K> :
		IRepository<T, K>
		where T : IAggregateRoot<K>
	{
		protected abstract IQueryable<T> RepositoryQuery { get; }


		public IEnumerator<T> GetEnumerator()
		{
			return RepositoryQuery.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return RepositoryQuery.GetEnumerator();
		}

		public Expression Expression
		{
			get { return RepositoryQuery.Expression; }
		}

		public Type ElementType
		{
			get { return RepositoryQuery.ElementType; }
		}

		public IQueryProvider Provider
		{
			get { return RepositoryQuery.Provider; }
		}

		public virtual void Dispose()
		{
		}

		public abstract T Get(K id);

		public abstract void Save(T item);

		public abstract void Delete(T item);
	}
}