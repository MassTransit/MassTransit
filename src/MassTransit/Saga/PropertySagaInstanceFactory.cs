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
namespace MassTransit.Saga
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;
    using GreenPipes.Internals.Extensions;
    using GreenPipes.Internals.Reflection;
    using Util;


    /// <summary>
    /// Creates a saga instance using the constructor, via a compiled expression. This class
    /// is built asynchronously and hot-wrapped to replace the basic Activator style.
    /// </summary>
    /// <typeparam name="TSaga"></typeparam>
    public class PropertySagaInstanceFactory<TSaga>
        where TSaga : class, ISaga
    {
        public PropertySagaInstanceFactory()
        {
            var constructorInfo = typeof(TSaga).GetConstructor(Type.EmptyTypes);
            ReadWriteProperty<TSaga> property;
            if (constructorInfo == null)
                throw new ArgumentException($"The saga {TypeMetadataCache<TSaga>.ShortName} does not have a default public constructor");

            if (!TypeCache<TSaga>.ReadWritePropertyCache.TryGetValue("CorrelationId", out property))
                throw new ArgumentException($"The saga {TypeMetadataCache<TSaga>.ShortName} does not have a writeable CorrelationId property");

            ParameterExpression correlationId = Expression.Parameter(typeof(Guid), "correlationId");

            NewExpression newSaga = Expression.New(constructorInfo);

            var saga = Expression.Variable(typeof(TSaga), "saga");

            var assign = Expression.Assign(saga, newSaga);

            MethodCallExpression call = Expression.Call(saga, property.Property.SetMethod, correlationId);

            LabelTarget returnTarget = Expression.Label(typeof(TSaga));

            GotoExpression returnExpression = Expression.Return(returnTarget,
                saga, typeof(TSaga));

            LabelExpression returnLabel = Expression.Label(returnTarget, Expression.Default(typeof(TSaga)));

            var block = Expression.Block(new[] {saga},
                assign,
                call,
                returnExpression,
                returnLabel);

            FactoryMethod = Expression.Lambda<SagaInstanceFactoryMethod<TSaga>>(block, correlationId).Compile();
        }

        public SagaInstanceFactoryMethod<TSaga> FactoryMethod { get; }
    }
}