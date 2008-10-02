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
namespace MassTransit.Infrastructure.Repositories
{
	using System;
	using Magnum.Infrastructure.Repository;
	using Services;

	public class PersistantDeferredMessageRepository :
		IDeferredMessageRepository
	{
		private NHibernateRepository _repository;

		public void Dispose()
		{
			_repository.Dispose();
		}

		public void Add(DeferredMessage message)
		{
			_repository.Save(message);
		}

		public DeferredMessage Get(Guid id)
		{
			DeferredMessage msg = _repository.Get<DeferredMessage>(id);
			return msg;
		}

		public bool Contains(Guid id)
		{
			return _repository.Get<DeferredMessage>(id) != null;
		}

		public void Remove(Guid id)
		{
			//TODO: Need a delete by Id method
			_repository.Delete(_repository.Get<DeferredMessage>(id));
		}
	}
}