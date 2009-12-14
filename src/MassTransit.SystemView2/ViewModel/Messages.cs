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
namespace MassTransit.SystemView.ViewModel
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;

    public class Messages :
        NotifyCollectionChangedBase<Message>,
        IList<Message>
    {
        private readonly IDictionary<string, Message> _messages = new Dictionary<string, Message>();

        public Message this[string message] { get { return Get(message); } }

        public Message Get(string message)
        {
            return _messages[message];
        }

        public IEnumerator<Message> GetEnumerator()
        {
            return _messages.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(Message item)
        {
            if (!Contains(item))
            {
                _messages.Add(item.MessageName, item);
                OnCollectionChanged(NotifyCollectionChangedAction.Add, item);
            }
        }

        public void Clear()
        {
            _messages.Clear();
        }

        public bool Contains(Message item)
        {
            return _messages.Keys.Contains(item.MessageName);
        }

        public void CopyTo(Message[] array, int arrayIndex)
        {
            _messages.Values.CopyTo(array, arrayIndex);
        }

        public bool Remove(Message item)
        {
            var retValue = _messages.Remove(item.MessageName);
            OnCollectionChanged(NotifyCollectionChangedAction.Remove, item);
            return retValue;
        }

        public bool Remove(string messageName)
        {
            var message = _messages[messageName];
            var retValue = _messages.Remove(messageName);
            OnCollectionChanged(NotifyCollectionChangedAction.Remove, message);
            return retValue;
        }

        public int Count
        {
            get { return _messages.Count; }
        }

        public bool IsReadOnly
        {
            get { return _messages.IsReadOnly; }
        }

        public int IndexOf(Message item)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, Message item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public Message this[int index]
        {
            get { return _messages.Values.ToList()[index]; }
            set { throw new NotImplementedException(); }
        }

        public void Update(Message message)
        {
            if (_messages.Keys.Contains(message.MessageName))
            {
                Remove(message);
                Add(message);
            }
            else
            {
                Add(message);
            }
        }
    }
}