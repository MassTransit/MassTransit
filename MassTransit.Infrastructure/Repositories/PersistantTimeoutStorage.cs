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
    using System.Collections.Generic;
    using System.Linq;
    using Magnum.Common.Repository;
    using ServiceBus.Services.Timeout;
    using ServiceBus.Timeout;
    using ServiceBus.Util;

    public class PersistantTimeoutStorage :
        ITimeoutRepository

	{
        private readonly IRepository<ScheduledTimeout, Guid> _repository;

        public PersistantTimeoutStorage(IRepository<ScheduledTimeout, Guid> repository)
        {
            _repository = repository;
        }

        public void Schedule(Guid id, DateTime timeoutAt)
        {
            ScheduledTimeout to = new ScheduledTimeout(id, timeoutAt);
            _repository.Save(to);
        }

        public void Remove(Guid id)
        {
            _repository.Delete(_repository.Get(id));
        }

        public IList<Tuple<Guid, DateTime>> List()
        {
            var items = _repository.List();
            IList<Tuple<Guid, DateTime>> result = new List<Tuple<Guid, DateTime>>();
            foreach (var item in items)
            {
                result.Add(new Tuple<Guid, DateTime>(item.Id, item.ExpiresAt));
            }
            return result;
        }

    	public IList<Tuple<Guid, DateTime>> List(DateTime lessThan)
    	{
    		var query = from item in _repository where item.ExpiresAt <= lessThan select item;

			IList<Tuple<Guid, DateTime>> result = new List<Tuple<Guid, DateTime>>();
			foreach (ScheduledTimeout item in query)
			{
				result.Add(new Tuple<Guid, DateTime>(item.Id, item.ExpiresAt));
			}
			return result;
		}

        public event Action<Guid> TimeoutAdded;
        public event Action<Guid> TimeoutUpdated;
        public event Action<Guid> TimeoutRemoved;
    }
}