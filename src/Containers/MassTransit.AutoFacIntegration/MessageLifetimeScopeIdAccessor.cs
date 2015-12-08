// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.AutofacIntegration
{
    using System.Reflection;
    using Internals.Reflection;


    public class MessageLifetimeScopeIdAccessor<TMessage, TId> :
        ILifetimeScopeIdAccessor<TMessage, TId>
    {
        readonly ReadOnlyProperty<TMessage, TId> _property;

        public MessageLifetimeScopeIdAccessor(PropertyInfo propertyInfo)
        {
            _property = new ReadOnlyProperty<TMessage, TId>(propertyInfo);
        }

        public bool TryGetScopeId(TMessage input, out TId id)
        {
            if (input != null)
            {
                id = _property.Get(input);
                return true;
            }

            id = default(TId);
            return false;
        }
    }
}