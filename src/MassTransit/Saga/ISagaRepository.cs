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
    using MassTransit.Pipeline;


    /// <summary>
    /// A saga repository is used by the service bus to dispatch messages to sagas
    /// </summary>
    /// <typeparam name="TSaga"></typeparam>
    public interface ISagaRepository<TSaga>
        where TSaga : class, ISaga
    {
        /// <summary>
        /// Send the message to the saga repository where the context.CorrelationId has the CorrelationId
        /// of the saga instance.
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="context">The message consume context</param>
        /// <param name="policy">The saga policy for the message</param>
        /// <param name="next">The saga consume pipe</param>
        /// <returns></returns>
        Task Send<T>(ConsumeContext<T> context, ISagaPolicy<TSaga, T> policy, IPipe<SagaConsumeContext<TSaga, T>> next)
            where T : class;

        /// <summary>
        /// Send the message to the saga repository where the query is used to find matching saga instances,
        /// which are invoked concurrently.
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="context">The saga query consume context</param>
        /// <param name="policy">The saga policy for the message</param>
        /// <param name="next">The saga consume pipe</param>
        /// <returns></returns>
        Task SendQuery<T>(SagaQueryConsumeContext<TSaga, T> context, ISagaPolicy<TSaga, T> policy, IPipe<SagaConsumeContext<TSaga, T>> next)
            where T : class;
    }
}