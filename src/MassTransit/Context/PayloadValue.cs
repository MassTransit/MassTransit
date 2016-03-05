// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Context
{
    using System;
    using Util;


    /// <summary>
    /// Stores a single scope data value
    /// </summary>
    /// <typeparam name="TPayload"></typeparam>
    public class PayloadValue<TPayload> :
        IPayloadValue<TPayload>
        where TPayload : class
    {
        readonly TPayload _value;

        public PayloadValue(TPayload value)
        {
            if (value == default(TPayload))
                throw new PayloadNotFoundException("The payload was not found: " + TypeMetadataCache<TPayload>.ShortName);

            _value = value;
        }

        Type IPayloadValue.ValueType => typeof(TPayload);
        TPayload IPayloadValue<TPayload>.Value => _value;

        bool IPayloadValue.TryGetValue<T>(out T value)
        {
            value = _value as T;

            return value != null;
        }
    }
}