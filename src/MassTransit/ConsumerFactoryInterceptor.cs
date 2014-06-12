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
    using System;
    using System.Threading.Tasks;


    /// <summary>
    /// An interceptor delegate that is passed the consumer, allowing the interceptor to
    /// establish scope before returning the Task
    /// </summary>
    /// <typeparam name="TConsumer">The consumer type</typeparam>
    /// <param name="consumer">The consumer instance</param>
    /// <param name="context">The consume context</param>
    /// <param name="callback">The callback to invoke to process the consumer</param>
    /// <returns></returns>
    public delegate Task ConsumerFactoryInterceptor<in TConsumer>(TConsumer consumer,
        ConsumeContext context, Func<Task> callback)
        where TConsumer : class;
}