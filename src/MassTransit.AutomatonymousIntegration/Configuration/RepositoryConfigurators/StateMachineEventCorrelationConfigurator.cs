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


    public interface StateMachineEventCorrelationConfigurator<TInstance, out TData>
        where TInstance : class, SagaStateMachineInstance
        where TData : class
    {
        StateMachineEventCorrelationConfigurator<TInstance, TData> SelectCorrelationId(Func<TData, Guid> correlationIdSelector);

        /// <summary>
        /// Specify the number of automatic retries to perform using the RetryLater
        /// mechanism (which moves the message to the end of the queue) before deferring
        /// retries to the built-in MassTransit mechanism.
        /// </summary>
        /// <param name="retryLimit"></param>
        /// <returns></returns>
        StateMachineEventCorrelationConfigurator<TInstance, TData> RetryLimit(int retryLimit);
    }
}