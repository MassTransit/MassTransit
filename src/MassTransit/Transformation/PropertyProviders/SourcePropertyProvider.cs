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
namespace MassTransit.Transformation.PropertyProviders
{
    using System;
    using System.Reflection;
    using Contexts;
    using Internals.Reflection;


    public class SourcePropertyProvider<TProperty, TInput> :
        IPropertyProvider<TProperty, TInput>
    {
        readonly Func<TInput, TProperty> _getValue;
        readonly Func<SourceContext<TProperty, TInput>, TProperty> _valueProvider;

        public SourcePropertyProvider(PropertyInfo inputProperty, Func<SourceContext<TProperty, TInput>, TProperty> valueProvider)
        {
            _valueProvider = valueProvider;
            if (inputProperty != null)
            {
                var getProperty = new ReadOnlyProperty<TInput, TProperty>(inputProperty);
                _getValue = getProperty.Get;
            }
            else
                _getValue = input => default(TProperty);
        }

        public TProperty GetProperty(TransformContext<TInput> context)
        {
            var sourceContext = context.HasInput
                ? new TransformSourceContext<TProperty, TInput>(context, _getValue(context.Input))
                : new TransformSourceContext<TProperty, TInput>(context);

            return _valueProvider(sourceContext);
        }
    }
}