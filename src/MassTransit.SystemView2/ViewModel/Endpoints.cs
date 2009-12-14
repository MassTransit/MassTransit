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
    using Services.Subscriptions.Messages;

    public class Endpoints :
        NotifyCollectionChangedBase<Endpoint>,
        IList<Endpoint>
    {
        private readonly IDictionary<Uri, Endpoint> _endpoints = new Dictionary<Uri, Endpoint>();

        public Endpoint Get(Uri uri)
        {
            return _endpoints[uri];
        }

        public IEnumerator<Endpoint> GetEnumerator()
        {
            return _endpoints.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(Endpoint item)
        {
            if (!Contains(item))
            {
                _endpoints.Add(item.EndpointUri, item);
                OnCollectionChanged(NotifyCollectionChangedAction.Add, item);
            }
        }

        public void Clear()
        {
            _endpoints.Clear();
        }

        public bool Contains(Endpoint item)
        {
            return _endpoints.Keys.Contains(item.EndpointUri);
        }

        public void CopyTo(Endpoint[] array, int arrayIndex)
        {
            _endpoints.Values.CopyTo(array, arrayIndex);
        }

        public bool Remove(Endpoint item)
        {
            item = _endpoints[item.EndpointUri];
            var retValue = _endpoints.Remove(item.EndpointUri);
            OnCollectionChanged(NotifyCollectionChangedAction.Remove, item);
            return retValue;
        }

        public bool Remove(Uri endpointUri, string messageName)
        {
            var retValue = _endpoints[endpointUri].Messages.Remove(messageName);
            if (_endpoints[endpointUri].Messages.Count == 0)
            {
                var endpoint = _endpoints[endpointUri];
                retValue &= _endpoints.Remove(endpointUri);
                OnCollectionChanged(NotifyCollectionChangedAction.Remove, endpoint);
            }
            return retValue;
        }

        public int Count
        {
            get { return _endpoints.Count; }
        }

        public bool IsReadOnly
        {
            get { return _endpoints.IsReadOnly; }
        }

        public int IndexOf(Endpoint item)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, Endpoint item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public Endpoint this[int index]
        {
            get { return _endpoints.Values.ToList()[index]; }
            set { throw new NotImplementedException(); }
        }

        public Endpoint this[Uri uri] { get { return Get(uri); } }

        public void Update(IList<SubscriptionInformation> subscriptions)
        {
            subscriptions.ToList().ForEach(Update);    
        }

        public void Update(SubscriptionInformation si)
        {
            if (_endpoints.Keys.Contains(si.EndpointUri))
            {
                _endpoints[si.EndpointUri].Messages.Update(new Message
                {
                    MessageName = si.MessageName,
                    ClientId = si.ClientId,
                    CorrelationId = si.CorrelationId,
                    SequenceNumber = si.SequenceNumber,
                    SubscriptionId = si.SubscriptionId
                });
            }
            else
            {
                var endpoint = new Endpoint {EndpointUri = si.EndpointUri};
                Add(endpoint);

                endpoint.Messages.Update(new Message
                {
                    MessageName = si.MessageName,
                    ClientId = si.ClientId,
                    CorrelationId = si.CorrelationId,
                    SequenceNumber = si.SequenceNumber,
                    SubscriptionId = si.SubscriptionId
                });
            }
        }
    }
}