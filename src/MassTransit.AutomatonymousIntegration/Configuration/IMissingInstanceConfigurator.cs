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
    using System.Threading.Tasks;
    using GreenPipes;
    using MassTransit;


    public interface IMissingInstanceConfigurator<TInstance, TData>
        where TInstance : SagaStateMachineInstance
        where TData : class
    {
        /// <summary>
        /// Discard the event, silently ignoring the missing instance for the event
        /// </summary>
        IPipe<ConsumeContext<TData>> Discard();

        /// <summary>
        /// Fault the saga consumer, which moves the message to the error queue
        /// </summary>
        IPipe<ConsumeContext<TData>> Fault();

        /// <summary>
        /// Execute an asynchronous method when the instance is missed, allowing a custom behavior to be specified.
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        IPipe<ConsumeContext<TData>> ExecuteAsync(Func<ConsumeContext<TData>, Task> action);

        /// <summary>
        /// Execute a method when the instance is missed, allowing a custom behavior to be specified.
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        IPipe<ConsumeContext<TData>> Execute(Action<ConsumeContext<TData>> action);
    }
}
