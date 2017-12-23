// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using GreenPipes;


    public interface IReceiveTransport :
        IReceiveObserverConnector,
        IReceiveTransportObserverConnector,
        IPublishObserverConnector,
        ISendObserverConnector,
        IProbeSite
    {
        /// <summary>
        /// Start receiving on a transport, sending messages to the specified pipe.
        /// </summary>
        /// <param name="receivePipe">The receiving pipe</param>
        /// <returns></returns>
        ReceiveTransportHandle Start(IPipe<ReceiveContext> receivePipe);
    }
}