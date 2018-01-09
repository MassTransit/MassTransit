// Copyright 2007-2018 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Initializers
{
    using System.Threading.Tasks;


    /// <summary>
    /// A message initializer that uses the input
    /// </summary>
    /// <typeparam name="TMessage">The message type</typeparam>
    /// <typeparam name="TInput">The input type</typeparam>
    public interface IMessagePropertyInitializer<in TMessage, in TInput>
        where TMessage : class
        where TInput : class
    {
        Task Apply(InitializeMessageContext<TMessage, TInput> context);
    }


    /// <summary>
    /// A message initializer that doesn't use the input
    /// </summary>
    /// <typeparam name="TMessage">The message type</typeparam>
    public interface IMessagePropertyInitializer<in TMessage>
        where TMessage : class
    {
        /// <summary>
        /// Apply the initializer to the message
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        Task Apply(InitializeMessageContext<TMessage> context);
    }
}