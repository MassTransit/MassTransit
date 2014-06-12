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
namespace MassTransit
{
    using System.Threading.Tasks;


    /// <summary>
    /// Invoked by a consumer factory within the context of the consumer (which gives lifecycle
    /// control of the consumer to the factory itself).
    /// </summary>
    /// <typeparam name="TConsumer">The consumer type</typeparam>
    /// <typeparam name="TMessage">The message type</typeparam>
    /// <param name="consumer">The consumer instance</param>
    /// <param name="context">The message consume context</param>
    /// <returns></returns>
    public delegate Task ConsumerFactoryCallback<in TConsumer, in TMessage>(TConsumer consumer,
        ConsumeContext<TMessage> context)
        where TConsumer : class
        where TMessage : class;
}