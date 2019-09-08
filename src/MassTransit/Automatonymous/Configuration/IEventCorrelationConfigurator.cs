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
namespace Automatonymous
{
    using System;
    using System.Linq.Expressions;
    using GreenPipes;
    using MassTransit;
    using MassTransit.Saga;


    public interface IEventCorrelationConfigurator<TInstance, TData>
        where TInstance : class, SagaStateMachineInstance
        where TData : class
    {
        /// <summary>
        /// If set to true, the state machine suggests that the saga instance be inserted blinding prior to the get/lock
        /// using a weaker isolation level. This prevents range locks in the database from slowing inserts.
        /// </summary>
        bool InsertOnInitial { set; }

        /// <summary>
        /// Correlate to the saga instance by CorrelationId, using the id from the event data
        /// </summary>
        /// <param name="selector">Returns the CorrelationId from the event data</param>
        /// <returns></returns>
        IEventCorrelationConfigurator<TInstance, TData> CorrelateById(Func<ConsumeContext<TData>, Guid> selector);

        /// <summary>
        /// Correlate to the saga instance by a single value property, matched to the property value of the message
        /// </summary>
        /// <param name="propertyExpression">The instance property</param>
        /// <param name="selector">The identifier selector for the message</param>
        /// <returns></returns>
        IEventCorrelationConfigurator<TInstance, TData> CorrelateById<T>(Expression<Func<TInstance, T>> propertyExpression,
            Func<ConsumeContext<TData>, T> selector)
            where T : struct;

        /// <summary>
        /// Correlate to the saga instance by a single property, matched to the property value of the message
        /// </summary>
        /// <param name="propertyExpression">The instance property</param>
        /// <param name="selector"></param>
        /// <returns></returns>
        IEventCorrelationConfigurator<TInstance, TData> CorrelateBy<T>(Expression<Func<TInstance, T?>> propertyExpression,
            Func<ConsumeContext<TData>, T?> selector)
            where T : struct;

        /// <summary>
        /// Correlate to the saga instance by a single property, matched to the property value of the message
        /// </summary>
        /// <param name="propertyExpression">The instance property</param>
        /// <param name="selector"></param>
        /// <returns></returns>
        IEventCorrelationConfigurator<TInstance, TData> CorrelateBy<T>(Expression<Func<TInstance, T>> propertyExpression,
            Func<ConsumeContext<TData>, T> selector)
            where T : class;

        /// <summary>
        /// When creating a new saga instance, initialize the saga CorrelationId with the id from the event data
        /// </summary>
        /// <param name="selector">Returns the CorrelationId from the event data</param>
        /// <returns></returns>
        IEventCorrelationConfigurator<TInstance, TData> SelectId(Func<ConsumeContext<TData>, Guid> selector);

        /// <summary>
        /// Specify the correlation expression for the event
        /// </summary>
        /// <param name="correlationExpression"></param>
        /// <returns></returns>
        IEventCorrelationConfigurator<TInstance, TData> CorrelateBy(Expression<Func<TInstance, ConsumeContext<TData>, bool>> correlationExpression);

        /// <summary>
        /// Creates a new instance of the saga, and if appropriate, pre-inserts the saga instance to the database. If the saga already exists, any
        /// exceptions from the insert are suppressed and processing continues normally.
        /// </summary>
        /// <param name="factoryMethod">The factory method for the saga</param>
        /// <returns></returns>
        IEventCorrelationConfigurator<TInstance, TData> SetSagaFactory(SagaFactoryMethod<TInstance, TData> factoryMethod);

        /// <summary>
        /// If an event is consumed that is not matched to an existing saga instance, discard the event without throwing an exception.
        /// The default behavior is to throw an exception, which moves the event into the error queue for later processing
        /// </summary>
        /// <param name="getBehavior">The configuration call to specify the behavior on missing instance</param>
        /// <returns></returns>
        IEventCorrelationConfigurator<TInstance, TData> OnMissingInstance(Func<IMissingInstanceConfigurator<TInstance, TData>, IPipe<ConsumeContext<TData>>>
            getBehavior);
    }
}
