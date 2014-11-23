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
namespace MassTransit.Testing
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;


    public class ObservedSentMessageList :
        ISentMessageList
    {
        readonly List<Guid> _index;
        readonly Dictionary<Guid, ISentMessage> _messages;
        readonly int _timeout;

        public ObservedSentMessageList()
            : this(TimeSpan.FromSeconds(8))
        {
        }

        public ObservedSentMessageList(TimeSpan timeout)
        {
            _messages = new Dictionary<Guid, ISentMessage>();
            _index = new List<Guid>();
            _timeout = (int)timeout.TotalMilliseconds;
        }

        public IEnumerator<ISentMessage> GetEnumerator()
        {
            return _messages.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerable<ISentMessage> Select()
        {
            return Select(x => true);
        }

        public IEnumerable<ISentMessage<T>> Select<T>()
            where T : class
        {
            return Select(x => typeof(T).IsAssignableFrom(x.MessageType))
                .Cast<ISentMessage<T>>();
        }

        public IEnumerable<ISentMessage<T>> Select<T>(Func<ISentMessage<T>, bool> filter) where T : class
        {
            return Select(x => typeof(T).IsAssignableFrom(x.MessageType))
                .Cast<ISentMessage<T>>()
                .Where(filter);
        }

        public IEnumerable<ISentMessage> Select(Func<ISentMessage, bool> filter)
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

        public void Add(ISentMessage message)
        {
            if (!message.Context.MessageId.HasValue)
                return;

            lock (_messages)
            {
                ISentMessage existing;
                if (_messages.TryGetValue(message.Context.MessageId.Value, out existing))
                {
                }
                else
                {
                    _messages.Add(message.Context.MessageId.Value, message);
                    _index.Add(message.Context.MessageId.Value);
                }

                Monitor.PulseAll(_messages);
            }
        }
    }
}