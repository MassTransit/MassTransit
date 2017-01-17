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
namespace MassTransit.Transformation.TransformConfigurators
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using GreenPipes;
    using PropertyProviders;
    using TransformBuilders;


    public class ValuePropertyTransformSpecification<TResult, TInput, TValue> :
        PropertyTransformSpecification<TResult, TInput, TValue>
    {
        readonly Func<IPropertyProvider<TValue, TInput>> _propertyProvider;

        public ValuePropertyTransformSpecification(PropertyInfo property, TValue value)
            : base(property, false)
        {
            _propertyProvider = () => new ValuePropertyProvider<TValue, TInput>(value);
        }

        protected override IPropertyProvider<TValue, TInput> GetPropertyProvider(ITransformBuilder<TResult, TInput> builder)
        {
            return _propertyProvider();
        }

        protected override IEnumerable<ValidationResult> ValidateConfiguration()
        {
            if (_propertyProvider == null)
                yield return this.Failure("PropertyProvider", "must not be null");
        }
    }
}