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
namespace MassTransit.PipeConfigurators
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;
    using Configurators;
    using Internals.Extensions;
    using PipeBuilders;
    using Pipeline.Filters;
    using Transformation;
    using Transformation.PropertyTransforms;


    public class TransformPipeSpecification<T> :
        ITransformConfigurator<T>,
        IPipeSpecification<ConsumeContext<T>>
        where T : class
    {
        readonly IList<IPropertyTransform<T>> _propertyTransforms;

        public TransformPipeSpecification()
        {
            _propertyTransforms = new List<IPropertyTransform<T>>();
        }

        public void Apply(IPipeBuilder<ConsumeContext<T>> builder)
        {
            IMessageTransform<T> transform = Build();

            builder.AddFilter(new TransformFilter<T>(transform));
        }

        public IEnumerable<ValidationResult> Validate()
        {
            yield break;
        }

        void ITransformConfigurator<T>.Replace<TProperty>(Expression<Func<T, TProperty>> propertyExpression, TProperty value)
        {
            PropertyInfo property = propertyExpression.GetPropertyInfo();

            var propertyTransform = new ReplacePropertyTransform<TProperty, T>(property, value);

            _propertyTransforms.Add(propertyTransform);
        }

        IMessageTransform<T> Build()
        {
            return new OriginalMessageTransform<T>(_propertyTransforms);
        }
    }
}