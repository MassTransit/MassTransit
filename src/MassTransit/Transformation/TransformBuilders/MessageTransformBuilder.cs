// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Transformation.TransformBuilders
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using GreenPipes;
    using GreenPipes.Internals.Extensions;
    using TransformConfigurators;
    using Transforms;
    using Util;


    public class MessageTransformBuilder<TResult, TInput> :
        ITransformBuilder<TResult, TInput>
    {
        readonly IDictionary<string, IPropertyTransform<TInput, TInput>> _inputTransforms;
        readonly TransformResultFactory<TResult> _resultFactory;
        readonly IDictionary<string, IPropertyTransform<TResult, TInput>> _resultTransforms;

        public MessageTransformBuilder(TransformResultFactory<TResult> resultFactory)
        {
            ImplementationType = typeof(TResult).GetTypeInfo().IsInterface
                ? TypeCache.ImplementationBuilder.GetImplementationType(typeof(TResult))
                : typeof(TResult);

            _resultFactory = resultFactory;

            _resultTransforms = new Dictionary<string, IPropertyTransform<TResult, TInput>>();
            _inputTransforms = new Dictionary<string, IPropertyTransform<TInput, TInput>>();
        }

        public void Set(string propertyName, IPropertyTransform<TResult, TInput> propertyTransform)
        {
            _resultTransforms[propertyName] = propertyTransform;
        }

        public Type ImplementationType { get; }

        public void Set(string propertyName, IPropertyTransform<TInput, TInput> propertyTransform)
        {
            _inputTransforms[propertyName] = propertyTransform;
        }

        public void DefaultAll()
        {
            _resultTransforms.Clear();

            DefaultUnmappedProperties();
        }

        public void RemoveAll()
        {
            _resultTransforms.Clear();
        }

        public ITransform<TResult, TInput> Build()
        {
            if (typeof(TResult).IsAssignableFrom(typeof(TInput)))
            {
                if (_resultTransforms.Count == 0)
                {
                    var transformType = typeof(InputTransform<,>).MakeGenericType(typeof(TResult), typeof(TInput));
                    return (ITransform<TResult, TInput>)Activator.CreateInstance(transformType, _inputTransforms.Values);
                }

                CopyUnmappedProperties();
            }

            DefaultUnmappedProperties();

            return new ResultTransform<TResult, TInput>(_resultFactory, _resultTransforms.Values);
        }

        void CopyUnmappedProperties()
        {
            foreach (var propertyInfo in GetUnmappedInputProperties())
            {
                var specificationType = typeof(CopyPropertyTransformSpecification<,,>).MakeGenericType(typeof(TResult), typeof(TInput),
                    propertyInfo.PropertyType);

                var specification = (IPropertyTransformSpecification<TResult, TInput>)Activator.CreateInstance(specificationType, propertyInfo);
                ValidationResult[] validationResults = specification.Validate().ToArray();

                specification.Configure(this);
            }
        }

        void DefaultUnmappedProperties()
        {
            foreach (var propertyInfo in GetUnmappedProperties())
            {
                var specificationType = typeof(DefaultPropertyTransformSpecification<,,>).MakeGenericType(typeof(TResult), typeof(TInput),
                    propertyInfo.PropertyType);

                var specification = (IPropertyTransformSpecification<TResult, TInput>)Activator.CreateInstance(specificationType, propertyInfo);
                ValidationResult[] validationResults = specification.Validate().ToArray();

                specification.Configure(this);
            }
        }

        IEnumerable<PropertyInfo> GetUnmappedInputProperties()
        {
            Dictionary<string, PropertyInfo> inputProperties = TypeMetadataCache<TInput>.Properties.ToDictionary(x => x.Name);

            return TypeMetadataCache<TResult>.Properties.Where(x => !_resultTransforms.ContainsKey(x.Name))
                .Where(x => inputProperties.ContainsKey(x.Name))
                .Where(x => inputProperties[x.Name].PropertyType == x.PropertyType);
        }

        IEnumerable<PropertyInfo> GetUnmappedProperties()
        {
            return TypeMetadataCache<TResult>.Properties.Where(x => !_resultTransforms.ContainsKey(x.Name));
        }
    }
}