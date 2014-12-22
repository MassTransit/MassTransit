// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace Automatonymous.RepositoryConfigurators
{
    using System;
    using System.Linq.Expressions;


    public interface StateMachineSagaRepositoryConfigurator<TSaga>
        where TSaga : class, SagaStateMachineInstance
    {
        StateMachine<TSaga> StateMachine { get; }

        StateMachineEventCorrelationConfigurator<TSaga, TData> Correlate<TData>(Event<TData> @event,
            Expression<Func<TSaga, TData, bool>> correlationExpression)
            where TData : class;

        void RemoveWhen(Expression<Func<TSaga, bool>> removeExpression);
    }
}