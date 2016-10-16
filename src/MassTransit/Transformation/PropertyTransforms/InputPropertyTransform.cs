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
namespace MassTransit.Transformation.PropertyTransforms
{
    using System.Reflection;
    using GreenPipes.Internals.Reflection;


    /// <summary>
    /// A transform that writes to the input model, instead of the result model
    /// </summary>
    /// <typeparam name="TProperty"></typeparam>
    /// <typeparam name="TInput"></typeparam>
    public class InputPropertyTransform<TProperty, TInput> :
        IPropertyTransform<TInput, TInput>
    {
        readonly ReadWriteProperty _property;
        readonly IPropertyProvider<TProperty, TInput> _propertyProvider;

        public InputPropertyTransform(PropertyInfo property, IPropertyProvider<TProperty, TInput> propertyProvider)
        {
            _propertyProvider = propertyProvider;
            _property = new ReadWriteProperty(property);
        }

        public void Apply(TInput result, TransformContext<TInput> context)
        {
            if (context.HasInput)
            {
                TProperty value = _propertyProvider.GetProperty(context);
                _property.Set(result, value);
            }
        }
    }
}