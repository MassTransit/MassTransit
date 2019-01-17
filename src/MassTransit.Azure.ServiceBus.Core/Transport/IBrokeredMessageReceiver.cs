// Copyright 2007-2019 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Azure.ServiceBus.Core.Transport
{
    using System;
    using System.Threading.Tasks;
    using MassTransit.Pipeline;
    using Microsoft.Azure.ServiceBus;
    using Transports;


    public interface IBrokeredMessageReceiver :
        IMessageReceiver,
        IConsumeMessageObserverConnector,
        IConsumeObserverConnector
    {
        /// <summary>
        /// Handles the <paramref name="message"/>
        /// </summary>
        /// <param name="message"></param>
        /// <param name="contextCallback">Callback to adjust the context</param>
        /// <returns></returns>
        Task Handle(Message message, Action<ReceiveContext> contextCallback = null);
    }
}