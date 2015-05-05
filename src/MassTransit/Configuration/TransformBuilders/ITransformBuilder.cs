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
namespace MassTransit.TransformBuilders
{
    using System;
    using Transformation;


    public interface ITransformBuilder<out TInput>
    {
        /// <summary>
        /// Add a property transform that transforms the input instead of the result
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="propertyTransform"></param>
        void Set(string propertyName, IPropertyTransform<TInput, TInput> propertyTransform);

        /// <summary>
        /// Set all properties on the resulting object to the default value
        /// </summary>
        void DefaultAll();

        /// <summary>
        /// Remove all existing transforms, which will do a 1:1 copy of the source to the target
        /// without any modifications
        /// </summary>
        void RemoveAll();
    }


    public interface ITransformBuilder<out TResult, out TInput> :
        ITransformBuilder<TInput>
    {
        Type ImplementationType { get; }

        /// <summary>
        /// Add a property transform for the specified property. If a transform for the property
        /// already exists, an exception is thrown.
        /// </summary>
        /// <param name="propertyName">The property name</param>
        /// <param name="propertyTransform">The property transform</param>
        void Set(string propertyName, IPropertyTransform<TResult, TInput> propertyTransform);
    }
}