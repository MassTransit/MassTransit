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
namespace Automatonymous
{
    using System;
    using System.Linq.Expressions;
    using MassTransit;


    public interface EventCorrelationConfigurator<TInstance, TData>
        where TInstance : class, SagaStateMachineInstance
        where TData : class
    {
        /// <summary>
        /// Correlate to the saga instance by CorrelationId, using the id from the event data
        /// </summary>
        /// <param name="selector">Returns the CorrelationId from the event data</param>
        /// <returns></returns>
        EventCorrelationConfigurator<TInstance, TData> CorrelateById(Func<ConsumeContext<TData>, Guid> selector);

        /// <summary>
        /// Correlate to the saga instance by a single property, matched to the property value of the message
        /// </summary>
        /// <param name="propertyExpression">The instance property</param>
        /// <param name="selector"></param>
        /// <returns></returns>
        EventCorrelationConfigurator<TInstance, TData> CorrelateBy<T>(Expression<Func<TInstance, T?>> propertyExpression,
            Func<ConsumeContext<TData>, T?> selector)
            where T : struct;

        /// <summary>
        /// Correlate to the saga instance by a single property, matched to the property value of the message
        /// </summary>
        /// <param name="propertyExpression">The instance property</param>
        /// <param name="selector"></param>
        /// <returns></returns>
        EventCorrelationConfigurator<TInstance, TData> CorrelateBy<T>(Expression<Func<TInstance, T>> propertyExpression,
            Func<ConsumeContext<TData>, T> selector)
            where T : class;

        /// <summary>
        /// When creating a new saga instance, initialize the saga CorrelationId with the id from the event data
        /// </summary>
        /// <param name="selector">Returns the CorrelationId from the event data</param>
        /// <returns></returns>
        EventCorrelationConfigurator<TInstance, TData> SelectId(Func<ConsumeContext<TData>, Guid> selector);

        /// <summary>
        /// Speicyf the correlation expression for the event
        /// </summary>
        /// <param name="correlationExpression"></param>
        /// <returns></returns>
        EventCorrelationConfigurator<TInstance, TData> CorrelateBy(Expression<Func<TInstance, ConsumeContext<TData>, bool>> correlationExpression);
    }
}