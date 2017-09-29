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
namespace MassTransit
{
    using System.Threading.Tasks;
    using GreenPipes;


    /// <summary>
    /// Maps an instance of a consumer to one or more Consume methods for the specified message type
    /// 
    /// The whole purpose for this interface is to allow the creator of the consumer to manage the lifecycle
    /// of the consumer, along with anything else that needs to be managed by the factory, container, etc.
    /// </summary>
    /// <typeparam name="TConsumer">The Consumer type</typeparam>
    public interface IConsumerFactory<out TConsumer> :
        IProbeSite
        where TConsumer : class
    {
        Task Send<T>(ConsumeContext<T> context, IPipe<ConsumerConsumeContext<TConsumer, T>> next)
            where T : class;
    }
}