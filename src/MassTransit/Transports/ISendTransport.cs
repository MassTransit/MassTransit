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
namespace MassTransit.Transports
{
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;


    public interface ISendTransport :
        ISendObserverConnector
    {
        /// <summary>
        ///     Send a message to the transport. The transport creates the OldSendContext, and calls back to
        ///     allow the context to be modified to customize the message delivery.
        ///     The transport specifies the defaults for the message as configured, and then allows the
        ///     caller to modify the send context to include the required settings (durable, mandatory, etc.).
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="message">The message</param>
        /// <param name="pipe">The pipe invoked when sending a message, to do extra stuff</param>
        /// <param name="cancelSend">Cancel the send operation (if possible)</param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        Task Send<T>(T message, IPipe<SendContext<T>> pipe, CancellationToken cancelSend)
            where T : class;

        /// <summary>
        /// Move a message from a receive transport to the transport
        /// </summary>
        /// <param name="context">The original receive context</param>
        /// <param name="pipe"></param>
        /// <returns>A task completed once the message has been sent</returns>
        Task Move(ReceiveContext context, IPipe<SendContext> pipe);

        /// <summary>
        /// Close the transport, releasing any resources it may own
        /// </summary>
        /// <returns></returns>
        Task Close();
    }
}