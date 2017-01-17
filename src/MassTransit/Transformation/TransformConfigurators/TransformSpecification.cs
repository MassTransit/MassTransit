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
namespace MassTransit.Transformation.TransformConfigurators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using GreenPipes;
    using Internals.Extensions;
    using TransformBuilders;
    using Util;


    public abstract class TransformSpecification<TMessage> :
        ITransformConfigurator<TMessage>
        where TMessage : class
    {
        readonly IList<IPropertyTransformSpecification<TMessage, TMessage>> _specifications;

        protected TransformSpecification()
        {
            _specifications = new List<IPropertyTransformSpecification<TMessage, TMessage>>();
        }

        public void Copy<TProperty>(Expression<Func<TMessage, TProperty>> propertyExpression)
        {
            var specification = new CopyPropertyTransformSpecification<TMessage, TMessage, TProperty>(propertyExpression.GetPropertyInfo());

            _specifications.Add(specification);
        }

        public void Default<TProperty>(Expression<Func<TMessage, TProperty>> propertyExpression)
        {
            var specification = new DefaultPropertyTransformSpecification<TMessage, TMessage, TProperty>(propertyExpression.GetPropertyInfo());

            _specifications.Add(specification);
        }

        public void Replace<TProperty>(Expression<Func<TMessage, TProperty>> propertyExpression,
            Func<SourceContext<TProperty, TMessage>, TProperty> valueProvider)
        {
            var specification = new InputPropertyTransformSpecification<TMessage, TMessage, TProperty>(propertyExpression.GetPropertyInfo(), valueProvider);

            _specifications.Add(specification);
        }

        public void Replace<TProperty>(PropertyInfo property, IPropertyProvider<TProperty, TMessage> propertyProvider)
        {
            var specification = new InputPropertyTransformSpecification<TMessage, TMessage, TProperty>(property, propertyProvider);

            _specifications.Add(specification);
        }

        public void Set<TProperty>(Expression<Func<TMessage, TProperty>> propertyExpression,
            Func<SourceContext<TProperty, TMessage>, TProperty> valueProvider)
        {
            var specification = new SourcePropertyTransformSpecification<TMessage, TMessage, TProperty>(propertyExpression.GetPropertyInfo(), valueProvider);

            _specifications.Add(specification);
        }

        public void Set<TProperty>(PropertyInfo property, IPropertyProvider<TProperty, TMessage> propertyProvider)
        {
            var specification = new SourcePropertyTransformSpecification<TMessage, TMessage, TProperty>(property, propertyProvider);

            _specifications.Add(specification);
        }

        public IEnumerable<ValidationResult> Validate()
        {
            return _specifications.SelectMany(x => x.Validate());
        }

        protected ITransform<TMessage, TMessage> Build()
        {
            var builder = new MessageTransformBuilder<TMessage, TMessage>(() => TypeMetadataCache<TMessage>.InitializeFromObject(new object()));

            for (var i = 0; i < _specifications.Count; i++)
                _specifications[i].Configure(builder);

            return builder.Build();
        }
    }
}