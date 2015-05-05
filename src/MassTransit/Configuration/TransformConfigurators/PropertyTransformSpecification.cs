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
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Configurators;
    using Transformation.PropertyTransforms;
    using TransformBuilders;


    public abstract class PropertyTransformSpecification<TResult, TInput, TProperty> :
        ITransformSpecification<TResult, TInput>
    {
        readonly PropertyInfo _property;
        readonly bool _validateInputProperty;
        PropertyInfo _inputProperty;

        protected PropertyTransformSpecification(PropertyInfo property, bool validateInputProperty)
        {
            _validateInputProperty = validateInputProperty;
            _property = property;
        }

        protected PropertyInfo Property
        {
            get { return _property; }
        }

        protected PropertyInfo InputProperty
        {
            get { return _inputProperty; }
        }

        IEnumerable<ValidationResult> Configurator.Validate()
        {
            return Validate().Concat(ValidateConfiguration());
        }

        void ITransformSpecification<TResult, TInput>.Configure(ITransformBuilder<TResult, TInput> builder)
        {
            IPropertyProvider<TProperty, TInput> propertyProvider = GetPropertyProvider(builder);

            var property = builder.ImplementationType == typeof(TResult)
                ? Property
                : builder.ImplementationType.GetProperty(Property.Name);

            Build(builder, propertyProvider, property);
        }

        IEnumerable<ValidationResult> Validate()
        {
            if (_property == null)
            {
                yield return this.Failure("Property", "must not be null");
                yield break;
            }

            if (typeof(TInput) != typeof(TResult))
            {
                PropertyInfo inputProperty = typeof(TInput).GetProperty(_property.Name);
                if (inputProperty == null)
                {
                    if (_validateInputProperty)
                        yield return this.Failure("Input." + _property.Name, "Property not found");
                }
                else
                {
                    if (inputProperty.PropertyType != _property.PropertyType)
                    {
                        if (_validateInputProperty)
                            yield return this.Failure("Input." + _property.Name, "The property type does not match");
                    }
                    else
                        _inputProperty = inputProperty;
                }
            }
            else
                _inputProperty = Property;
        }

        protected virtual void Build(ITransformBuilder<TResult, TInput> builder, IPropertyProvider<TProperty, TInput> propertyProvider, PropertyInfo property)
        {
            var transform = new ResultPropertyTransform<TProperty, TResult, TInput>(property, propertyProvider);

            builder.Set(property.Name, transform);
        }

        protected abstract IPropertyProvider<TProperty, TInput> GetPropertyProvider(ITransformBuilder<TResult, TInput> builder);

        protected abstract IEnumerable<ValidationResult> ValidateConfiguration();
    }
}