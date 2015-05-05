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
namespace MassTransit.TransformConfigurators
{
    using System;
    using System.Reflection;
    using Transformation;
    using Transformation.PropertyTransforms;
    using TransformBuilders;


    public class InputPropertyTransformSpecification<TResult, TInput, TProperty> :
        SourcePropertyTransformSpecification<TResult, TInput, TProperty>
    {
        public InputPropertyTransformSpecification(PropertyInfo property, Func<SourceContext<TProperty, TInput>, TProperty> valueProvider)
            : base(property, valueProvider)
        {
        }

        protected override void Build(ITransformBuilder<TResult, TInput> builder, IPropertyProvider<TProperty, TInput> propertyProvider, PropertyInfo property)
        {
            var transform = new InputPropertyTransform<TProperty, TInput>(property, propertyProvider);

            builder.Set(Property.Name, transform);
        }
    }
}