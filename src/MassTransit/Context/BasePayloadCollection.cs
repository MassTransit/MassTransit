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


    public abstract class BasePayloadCollection :
        IPayloadCollection
    {
        readonly IReadOnlyPayloadCollection _parent;

        protected BasePayloadCollection(IReadOnlyPayloadCollection parent)
        {
            _parent = parent;
        }

        protected IReadOnlyPayloadCollection Parent => _parent;

        public virtual bool HasPayloadType(Type propertyType)
        {
            return _parent?.HasPayloadType(propertyType) ?? false;
        }

        public virtual bool TryGetPayload<T>(out T value) where T : class
        {
            if (_parent != null)
                return _parent.TryGetPayload(out value);

            value = null;
            return false;
        }

        public abstract IPayloadCollection Add(IPayloadValue payload);
    }
}