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
    using Util;


    public class AmazonSqsHeaderAdapter :
        SendHeaders
    {
        readonly IDictionary<string, MessageAttributeValue> _attributes;

        public AmazonSqsHeaderAdapter(IDictionary<string, MessageAttributeValue> attributes)
        {
            _attributes = attributes;
        }

        public void Set(string key, string value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (value == null)
                _attributes.Remove(key);
            else
                _attributes[key] = ToValue(value);
        }

        public void Set(string key, object value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (value == null)
                _attributes.Remove(key);
            else
                _attributes[key] = ToValue(value.ToString());
        }

        public bool TryGetHeader(string key, out object value)
        {
            if (_attributes.TryGetValue(key, out var val))
            {
                value = val.StringValue;
                return true;
            }

            value = null;
            return false;
        }

        MessageAttributeValue ToValue(string value)
        {
            return new MessageAttributeValue
            {
                StringValue = value,
                DataType = "String"
            };
        }

        public IEnumerable<KeyValuePair<string, object>> GetAll()
        {
            foreach (string key in _attributes.Keys)
            {
                var value = _attributes[key];

                yield return new KeyValuePair<string, object>(key, value);
            }
        }

        T Headers.Get<T>(string key, T defaultValue)
        {
            if (TryGetHeader(key, out var value))
                return ObjectTypeDeserializer.Deserialize(value, defaultValue);

            return defaultValue;
        }

        public T? Get<T>(string key, T? defaultValue)
            where T : struct
        {
            if (TryGetHeader(key, out var value))
                return ObjectTypeDeserializer.Deserialize(value, defaultValue);

            return defaultValue;
        }
    }
}
