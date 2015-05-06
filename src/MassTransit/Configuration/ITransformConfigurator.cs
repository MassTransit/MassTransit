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
    using System.Linq.Expressions;
    using System.Reflection;
    using Transformation;


    public interface ITransformConfigurator<TInput>
    {
        /// <summary>
        /// Copy the specified message property from the input to the result (all properties are copied by default).
        /// Only use this to copy specific properties, ignoring others. This is really just for completeness, it's
        /// not necessary to use it.
        /// </summary>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="propertyExpression"></param>
        void Copy<TProperty>(Expression<Func<TInput, TProperty>> propertyExpression);

        /// <summary>
        /// Set the specified message property to the default value (ignoring the input value)
        /// </summary>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="propertyExpression"></param>
        void Default<TProperty>(Expression<Func<TInput, TProperty>> propertyExpression);

        /// <summary>
        /// Replace the value on the input with the specified value
        /// </summary>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="propertyExpression"></param>
        /// <param name="valueProvider">The method to return the property</param>
        void Replace<TProperty>(Expression<Func<TInput, TProperty>> propertyExpression, Func<SourceContext<TProperty, TInput>, TProperty> valueProvider);

        /// <summary>
        /// Replace the value on the input with the specified value
        /// </summary>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="property"></param>
        /// <param name="propertyProvider"></param>
        void Replace<TProperty>(PropertyInfo property, IPropertyProvider<TProperty, TInput> propertyProvider);

        /// <summary>
        /// Set the property to the value, using the source context to create/select the value
        /// </summary>
        /// <typeparam name="TProperty">The property type</typeparam>
        /// <param name="propertyExpression">The property select expression</param>
        /// <param name="valueProvider">The method to return the property</param>
        void Set<TProperty>(Expression<Func<TInput, TProperty>> propertyExpression, Func<SourceContext<TProperty, TInput>, TProperty> valueProvider);

        /// <summary>
        /// Set the property to the value, using the source context to create/select the value
        /// </summary>
        /// <typeparam name="TProperty">The property type</typeparam>
        /// <param name="property"></param>
        /// <param name="propertyProvider"></param>
        void Set<TProperty>(PropertyInfo property, IPropertyProvider<TProperty, TInput> propertyProvider);
    }
}