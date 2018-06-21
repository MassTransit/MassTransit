// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Testing.MessageObservers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading;


    public class MessageList<T> :
        IEnumerable<T>
    {
        readonly List<Guid> _index;
        readonly Dictionary<Guid, T> _messages;
        readonly int _timeout;

        protected MessageList(int timeout)
        {
            _timeout = timeout;
            _messages = new Dictionary<Guid, T>();
            _index = new List<Guid>();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _messages.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerable<T> Select()
        {
            return Select(x => true);
        }

        public IEnumerable<T> Select(Func<T, bool> filter)
        {
            lock (_messages)
            {
                int i = 0;
                for (; i < _index.Count; i++)
                {
                    if (filter(_messages[_index[i]]))
                        yield return _messages[_index[i]];
                }

                while (Monitor.Wait(_messages, _timeout))
                {
                    for (; i < _index.Count; i++)
                    {
                        if (filter(_messages[_index[i]]))
                            yield return _messages[_index[i]];
                    }
                }
            }
        }

        public void Add(T message, Guid? messageId)
        {
            if (!messageId.HasValue)
                return;

            lock (_messages)
            {
                T existing;
                if (_messages.TryGetValue(messageId.Value, out existing))
                {
                }
                else
                {
                    _messages.Add(messageId.Value, message);
                    _index.Add(messageId.Value);
                }

                Monitor.PulseAll(_messages);
            }
        }
    }
}