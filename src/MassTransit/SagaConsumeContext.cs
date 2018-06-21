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
    using System.Threading.Tasks;
    using Saga;


    /// <summary>
    /// Consume context including the saga instance consuming the message
    /// </summary>
    /// <typeparam name="TSaga">The saga type</typeparam>
    /// <typeparam name="TMessage">The message type</typeparam>
    public interface SagaConsumeContext<TSaga, out TMessage> :
        SagaConsumeContext<TSaga>,
        ConsumeContext<TMessage>
        where TSaga : class, ISaga
        where TMessage : class
    {
    }


    /// <summary>
    /// Consume context including the saga instance consuming the message. Note
    /// this does not expose the message type, for filters that do not care about message type.
    /// </summary>
    /// <typeparam name="TSaga">The saga type</typeparam>
    public interface SagaConsumeContext<TSaga> :
        ConsumeContext
        where TSaga : class, ISaga
    {
        /// <summary>
        /// The saga instance for the current consume operation
        /// </summary>
        TSaga Saga { get; }

        SagaConsumeContext<TSaga, T> PopContext<T>()
            where T : class;

        /// <summary>
        /// Mark the saga instance as completed, which may remove it from the repository or archive it, etc.
        /// Once completed, a saga instance should never again be visible, even if the same CorrelationId is
        /// specified.
        /// </summary>
        /// <returns></returns>
        Task SetCompleted();

        /// <summary>
        /// True if the saga has been completed, signaling that the repository may remove it.
        /// </summary>
        bool IsCompleted { get; }
    }
}