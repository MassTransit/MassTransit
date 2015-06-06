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
    using System.Threading.Tasks;


    /// <summary>
    /// Policy that is applied
    /// </summary>
    /// <typeparam name="TSaga"></typeparam>
    /// <typeparam name="TMessage"></typeparam>
    public interface ISagaRepositoryContext<TSaga, in TMessage>
        where TSaga : class, ISaga
        where TMessage : class
    {
        /// <summary>
        /// Save the saga instance, updating and changes in the saga state
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        Task Save(SagaConsumeContext<TSaga, TMessage> context);

        /// <summary>
        /// Mark the saga instance as complete, which may remove it from the repository or archive it, etc.
        /// Once completed, a saga instance should never again be visible, even if the same CorrelationId is
        /// specified.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        Task Complete(SagaConsumeContext<TSaga, TMessage> context);
    }


    /// <summary>
    /// Policy that is applied
    /// </summary>
    /// <typeparam name="TSaga"></typeparam>
    public interface ISagaCompletionPolicy<TSaga>
        where TSaga : class, ISaga
    {
        Task Send<TMessage>(SagaConsumeContext<TSaga, TMessage> context, ISagaRepositoryContext<TSaga, TMessage> context2)
            where TMessage : class;
    }
}