// Copyright 2007-2017 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Topology.Conventions.CorrelationId
{
    using System;
    using Context;


    public class PropertyCorrelationIdSelector<T> :
        ICorrelationIdSelector<T>
        where T : class
    {
        readonly string _propertyName;

        public PropertyCorrelationIdSelector(string propertyName)
        {
            _propertyName = propertyName;
        }

        public bool TryGetSetCorrelationId(out ISetCorrelationId<T> setCorrelationId)
        {
            var propertyInfo = typeof(T).GetProperty(_propertyName);
            if (propertyInfo != null && propertyInfo.PropertyType == typeof(Guid))
            {
                setCorrelationId = new PropertySetCorrelationId<T>(propertyInfo);
                return true;
            }

            if (propertyInfo != null && propertyInfo.PropertyType == typeof(Guid?))
            {
                setCorrelationId = new NullablePropertySetCorrelationId<T>(propertyInfo);
                return true;
            }
            setCorrelationId = null;
            return false;
        }
    }
}