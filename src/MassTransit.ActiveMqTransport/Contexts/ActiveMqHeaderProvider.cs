// Copyright 2007-2018 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.ActiveMqTransport.Contexts
{
    using System;
    using System.Collections.Generic;
    using Apache.NMS;
    using Context;


    public class ActiveMqHeaderProvider :
        IHeaderProvider
    {
        readonly IMessage _message;
        readonly ActiveMqHeaderAdapter _adapter;

        public ActiveMqHeaderProvider(IMessage message)
        {
            _message = message;
            _adapter = new ActiveMqHeaderAdapter(message.Properties);
        }

        public IEnumerable<KeyValuePair<string, object>> GetAll()
        {
            yield return new KeyValuePair<string, object>("MessageId", _message.NMSMessageId);
            yield return new KeyValuePair<string, object>("CorrelationId", _message.NMSCorrelationID);

            foreach (var header in _adapter.GetAll())
                yield return header;
        }

        public bool TryGetHeader(string key, out object value)
        {
            if ("MessageId".Equals(key, StringComparison.OrdinalIgnoreCase))
            {
                value = _message.NMSMessageId;
                return true;
            }

            if ("CorrelationId".Equals(key, StringComparison.OrdinalIgnoreCase))
            {
                value = _message.NMSCorrelationID;
                return true;
            }

            return _adapter.TryGetHeader(key, out value);
        }
    }
}