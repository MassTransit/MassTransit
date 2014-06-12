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
    ///     Defines a class that is a consumer of a message. The message is wrapped in an IConsumeContext
    ///     interface to allow access to details surrounding the inbound message, including headers.
    /// </summary>
    /// <typeparam name="TMessage">The message type</typeparam>
    public interface IConsumer<in TMessage> 
        where TMessage : class
    {
        Task Consume(ConsumeContext<TMessage> context);
    }


    /// <summary>
    ///     Marker interface used to assist identification in IoC containers.
    ///     Not to be used directly as it does not contain the message type of the
    ///     consumer
    /// </summary>
    /// <remarks>
    ///     Not to be used directly by application code, for internal reflection only
    /// </remarks>
    public interface IConsumer
    {
    }
}