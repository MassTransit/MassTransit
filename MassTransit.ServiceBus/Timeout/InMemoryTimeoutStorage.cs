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
namespace MassTransit.ServiceBus.Timeout
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading;

    public class InMemoryTimeoutStorage :
        ITimeoutStorage
    {
        private static readonly TimeSpan _interval = TimeSpan.FromSeconds(1);

        private readonly ManualResetEvent _stopped = new ManualResetEvent(false);
        private readonly AutoResetEvent _trigger = new AutoResetEvent(true);
        private volatile Dictionary<Guid, DateTime> _schedule = new Dictionary<Guid, DateTime>();

        IEnumerator<Guid> IEnumerable<Guid>.GetEnumerator()
        {
            WaitHandle[] handles = new WaitHandle[] {_trigger, _stopped};

            int waitResult;
            while ((waitResult = WaitHandle.WaitAny(handles, _interval, true)) != 1)
            {
                DateTime present = DateTime.UtcNow;

                Guid matched = Guid.Empty;

                lock (_schedule)
                {
                    foreach (KeyValuePair<Guid, DateTime> pair in _schedule)
                    {
                        if (pair.Value > present) continue;

                        matched = pair.Key;
                        break;
                    }
                }

                if (matched != Guid.Empty)
                {
                    _schedule.Remove(matched);
                    yield return matched;
                }
            }
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable<Guid>) this).GetEnumerator();
        }

        public void Schedule(Guid id, DateTime timeoutAt)
        {
            lock (_schedule)
            {
                if (_schedule.ContainsKey(id))
                    _schedule[id] = timeoutAt;
                else
                    _schedule.Add(id, timeoutAt);
            }

            _trigger.Set();
        }

        public void Remove(Guid id)
        {
            lock (_schedule)
            {
                if (_schedule.ContainsKey(id))
                    _schedule.Remove(id);
            }
        }

        public void Start()
        {
            _stopped.Reset();
        }

        public void Stop()
        {
            _stopped.Set();
        }
    }
}