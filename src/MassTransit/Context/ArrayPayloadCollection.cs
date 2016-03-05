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
    using System.Linq;


    public class ArrayPayloadCollection :
        BasePayloadCollection
    {
        readonly IReadOnlyPayloadCollection _parent;
        readonly IPayloadValue[] _payloads;

        public ArrayPayloadCollection(IReadOnlyPayloadCollection parent, params IPayloadValue[] payloads)
            : base(parent)
        {
            _payloads = payloads;
            _parent = parent;
        }

        ArrayPayloadCollection(IReadOnlyPayloadCollection parent, IPayloadValue payload, IPayloadValue[] payloads)
            : base(parent)
        {
            _parent = parent;

            _payloads = new IPayloadValue[payloads.Length + 1];
            _payloads[0] = payload;
            Array.Copy(payloads, 0, _payloads, 1, payloads.Length);
        }

        public override bool HasPayloadType(Type propertyType)
        {
            if (_payloads.Any(x => propertyType.IsAssignableFrom(x.ValueType)))
                return true;

            return base.HasPayloadType(propertyType);
        }

        public override bool TryGetPayload<T>(out T value)
        {
            for (var i = 0; i < _payloads.Length; i++)
            {
                if (_payloads[i].TryGetValue(out value))
                {
                    return true;
                }
            }

            return base.TryGetPayload(out value);
        }

        public override IPayloadCollection Add(IPayloadValue payload)
        {
            return new ArrayPayloadCollection(_parent, payload, _payloads);
        }
    }
}