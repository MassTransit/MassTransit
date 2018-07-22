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
namespace MassTransit.AmazonSqsTransport.Contexts
{
    using System;
    using System.Collections.Generic;
    using Amazon.SQS.Model;
    using Context;


    public class AmazonSqsHeaderProvider :
        IHeaderProvider
    {
        readonly Message _message;
        readonly AmazonSqsHeaderAdapter _adapter;

        public AmazonSqsHeaderProvider(Message message)
        {
            _message = message;
            _adapter = new AmazonSqsHeaderAdapter(message.MessageAttributes);
        }

        public IEnumerable<KeyValuePair<string, object>> GetAll()
        {
            yield return new KeyValuePair<string, object>("MessageId", _message.MessageId);
            yield return new KeyValuePair<string, object>("CorrelationId", _message.MessageAttributes["CorrelationId"].StringValue);

            foreach (var header in _adapter.GetAll())
                yield return header;
        }

        public bool TryGetHeader(string key, out object value)
        {
            if ("MessageId".Equals(key, StringComparison.OrdinalIgnoreCase))
            {
                value = _message.MessageId;
                return true;
            }

            return _adapter.TryGetHeader(key, out value);
        }
    }
}