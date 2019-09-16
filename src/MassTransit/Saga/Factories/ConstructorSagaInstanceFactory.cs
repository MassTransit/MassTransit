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
namespace MassTransit.Saga.Factories
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;
    using Internals.Reflection;
    using Metadata;
    using Util;


    /// <summary>
    /// Creates a saga instance using the constructor, via a compiled expression. This class
    /// is built asynchronously and hot-wrapped to replace the basic Activator style.
    /// </summary>
    /// <typeparam name="TSaga"></typeparam>
    public class ConstructorSagaInstanceFactory<TSaga>
        where TSaga : class, ISaga
    {
        public ConstructorSagaInstanceFactory()
        {
            ConstructorInfo constructorInfo = typeof(TSaga).GetConstructor(new[] {typeof(Guid)});
            if (constructorInfo == null)
            {
                throw new ArgumentException("The saga does not have a public constructor with a single Guid correlationId parameter: "
                    + TypeMetadataCache<TSaga>.ShortName);
            }

            ParameterExpression correlationId = Expression.Parameter(typeof(Guid), "correlationId");
            NewExpression @new = Expression.New(constructorInfo, correlationId);

            FactoryMethod = Expression.Lambda<SagaInstanceFactoryMethod<TSaga>>(@new, correlationId).CompileFast();
        }

        public SagaInstanceFactoryMethod<TSaga> FactoryMethod { get; }
    }
}