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
namespace MassTransit
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;
    using Configurators;
    using PipeBuilders;
    using PipeConfigurators;
    using Transformation;
    using TransformConfigurators;


    /// <summary>
    /// Used to build a transform configuration that can be reused easily.
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    public abstract class TransformConfiguration<TMessage> :
        ITransformConfiguration<TMessage>
        where TMessage : class
    {
        readonly TransformConsumePipeSpecification<TMessage> _configurator;

        protected TransformConfiguration()
        {
            _configurator = new TransformConsumePipeSpecification<TMessage>();
        }

        void IPipeSpecification<ConsumeContext<TMessage>>.Apply(IPipeBuilder<ConsumeContext<TMessage>> builder)
        {
            ((IPipeSpecification<ConsumeContext<TMessage>>)_configurator).Apply(builder);
        }

        IEnumerable<ValidationResult> Configurator.Validate()
        {
            return ((Configurator)_configurator).Validate();
        }

        protected void Copy<TProperty>(Expression<Func<TMessage, TProperty>> propertyExpression)
        {
            ((ITransformConfigurator<TMessage>)_configurator).Copy(propertyExpression);
        }

        protected void Default<TProperty>(Expression<Func<TMessage, TProperty>> propertyExpression)
        {
            ((ITransformConfigurator<TMessage>)_configurator).Default(propertyExpression);
        }

        protected void Replace<TProperty>(Expression<Func<TMessage, TProperty>> propertyExpression,
            Func<SourceContext<TProperty, TMessage>, TProperty> valueProvider)
        {
            ((ITransformConfigurator<TMessage>)_configurator).Replace(propertyExpression, valueProvider);
        }

        protected void Replace<TProperty>(PropertyInfo property, IPropertyProvider<TProperty, TMessage> propertyProvider)
        {
            ((ITransformConfigurator<TMessage>)_configurator).Replace(property, propertyProvider);
        }

        protected void Set<TProperty>(Expression<Func<TMessage, TProperty>> propertyExpression,
            Func<SourceContext<TProperty, TMessage>, TProperty> valueProvider)
        {
            ((ITransformConfigurator<TMessage>)_configurator).Set(propertyExpression, valueProvider);
        }

        protected void Set<TProperty>(PropertyInfo property, IPropertyProvider<TProperty, TMessage> propertyProvider)
        {
            ((ITransformConfigurator<TMessage>)_configurator).Set(property, propertyProvider);
        }
    }
}