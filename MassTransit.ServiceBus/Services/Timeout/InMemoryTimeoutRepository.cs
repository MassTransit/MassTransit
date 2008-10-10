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
namespace MassTransit.ServiceBus.Services.Timeout
{
    using System;
    using System.Collections.Generic;
    using Util;

    public class InMemoryTimeoutRepository :
        ITimeoutRepository
    {
        private volatile Dictionary<Guid, DateTime> _schedule = new Dictionary<Guid, DateTime>();

        public void Schedule(Guid id, DateTime timeoutAt)
        {
            bool added = false;
            bool updated = false;
            lock (_schedule)
            {
                if (_schedule.ContainsKey(id))
                {
                    _schedule[id] = timeoutAt;
                    updated = true;
                }
                else
                {
                    _schedule.Add(id, timeoutAt);
                    added = true;
                }
            }

            if (added)
                TimeoutAdded(id);
            if (updated)
                TimeoutUpdated(id);
        }

        public void Remove(Guid id)
        {
            bool notify = false;
            lock (_schedule)
            {
                if (_schedule.ContainsKey(id))
                {
                    _schedule.Remove(id);
                    notify = true;
                }
            }

            if (notify)
                TimeoutRemoved(id);
        }

        public event Action<Guid> TimeoutAdded = delegate { };

        public event Action<Guid> TimeoutUpdated = delegate { };

        public event Action<Guid> TimeoutRemoved = delegate { };

        public IList<Tuple<Guid, DateTime>> List()
        {
            List<Tuple<Guid, DateTime>> result = new List<Tuple<Guid, DateTime>>();

            foreach (KeyValuePair<Guid, DateTime> pair in _schedule)
            {
                result.Add(new Tuple<Guid, DateTime>(pair));
            }

            return result;
        }

        public IList<Tuple<Guid, DateTime>> List(DateTime lessThan)
        {
            List<Tuple<Guid, DateTime>> result = new List<Tuple<Guid, DateTime>>();

            foreach (KeyValuePair<Guid, DateTime> pair in _schedule)
            {
                if(pair.Value <= lessThan)
                    result.Add(new Tuple<Guid, DateTime>(pair));
            }

            return result;
        }
    }
}