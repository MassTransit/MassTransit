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
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Configurators;
    using Internals.Extensions;
    using PipeBuilders;
    using PipeConfigurators;
    using Pipeline.Filters;
    using Transformation;
    using TransformBuilders;
    using Util;


    public class TransformConsumePipeSpecification<TMessage> :
        ITransformConfigurator<TMessage>,
        IPipeSpecification<ConsumeContext<TMessage>>
        where TMessage : class
    {
        readonly IList<ITransformSpecification<TMessage, TMessage>> _specifications;

        public TransformConsumePipeSpecification()
        {
            _specifications = new List<ITransformSpecification<TMessage, TMessage>>();
        }

        void IPipeSpecification<ConsumeContext<TMessage>>.Apply(IPipeBuilder<ConsumeContext<TMessage>> builder)
        {
            ITransform<TMessage, TMessage> transform = Build();

            builder.AddFilter(new TransformFilter<TMessage>(transform));
        }

        IEnumerable<ValidationResult> Configurator.Validate()
        {
            return _specifications.SelectMany(x => x.Validate());
        }

        void ITransformConfigurator<TMessage>.Replace<TProperty>(Expression<Func<TMessage, TProperty>> propertyExpression, Func<SourceContext<TProperty, TMessage>, TProperty> valueProvider)
        {
            var specification = new InputPropertyTransformSpecification<TMessage, TMessage, TProperty>(propertyExpression.GetPropertyInfo(), valueProvider);

            _specifications.Add(specification);
        }

        void ITransformConfigurator<TMessage>.Replace<TProperty>(PropertyInfo property, IPropertyProvider<TProperty, TMessage> propertyProvider)
        {
            var specification = new InputPropertyTransformSpecification<TMessage, TMessage, TProperty>(property, propertyProvider);

            _specifications.Add(specification);
        }

        void ITransformConfigurator<TMessage>.Set<TProperty>(Expression<Func<TMessage, TProperty>> propertyExpression,
            Func<SourceContext<TProperty, TMessage>, TProperty> valueProvider)
        {
            var specification = new SourcePropertyTransformSpecification<TMessage, TMessage, TProperty>(propertyExpression.GetPropertyInfo(), valueProvider);

            _specifications.Add(specification);
        }

        void ITransformConfigurator<TMessage>.Set<TProperty>(PropertyInfo property, IPropertyProvider<TProperty, TMessage> propertyProvider)
        {
            var specification = new SourcePropertyTransformSpecification<TMessage, TMessage, TProperty>(property, propertyProvider);

            _specifications.Add(specification);
        }

        ITransform<TMessage, TMessage> Build()
        {
            var builder = new MessageTransformBuilder<TMessage, TMessage>(() => TypeMetadataCache<TMessage>.InitializeFromObject(new object()));

            for (int i = 0; i < _specifications.Count; i++)
                _specifications[i].Configure(builder);

            return builder.Build();
        }
    }
}