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
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;


    /// <summary>
    /// A message initializer that doesn't use the input
    /// </summary>
    /// <typeparam name="TMessage">The message type</typeparam>
    public interface IMessageInitializer<in TMessage>
        where TMessage : class
    {
    }


    /// <summary>
    /// Initialize a message type using the input specified, and use it in the various methods
    /// </summary>
    /// <typeparam name="TMessage">The message type</typeparam>
    /// <typeparam name="TInput">The input type</typeparam>
    public interface IMessageInitializer<TMessage, in TInput> :
        IMessageInitializer<TMessage>
        where TMessage : class
        where TInput : class
    {
        /// <summary>
        /// Initialize the message, using the input
        /// </summary>
        /// <param name="input"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<TMessage> Initialize(TInput input, CancellationToken cancellationToken);

        /// <summary>
        /// Initialize the message using the input and send it to the endpoint.
        /// </summary>
        /// <param name="endpoint">The destination endpoint</param>
        /// <param name="input">The input object</param>
        /// <param name="pipe"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task Send(ISendEndpoint endpoint, TInput input, IPipe<SendContext> pipe, CancellationToken cancellationToken);

        /// <summary>
        /// Initialize the message using the input and send it to the endpoint.
        /// </summary>
        /// <param name="endpoint">The destination endpoint</param>
        /// <param name="input">The input object</param>
        /// <param name="pipe"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task Send(ISendEndpoint endpoint, TInput input, IPipe<SendContext<TMessage>> pipe, CancellationToken cancellationToken);

        /// <summary>
        /// Initialize the message using the input and publish it
        /// </summary>
        /// <param name="endpoint"></param>
        /// <param name="input"></param>
        /// <param name="pipe"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task Publish(IPublishEndpoint endpoint, TInput input, IPipe<PublishContext<TMessage>> pipe, CancellationToken cancellationToken);

        /// <summary>
        /// Initialize the message using the input and publish it
        /// </summary>
        /// <param name="endpoint"></param>
        /// <param name="input"></param>
        /// <param name="pipe"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task Publish(IPublishEndpoint endpoint, TInput input, IPipe<PublishContext> pipe, CancellationToken cancellationToken);
    }
}