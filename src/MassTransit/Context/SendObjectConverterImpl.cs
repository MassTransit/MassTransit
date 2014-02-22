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
namespace MassTransit.Context
{
    using System;
    using Magnum.Extensions;


    public class SendObjectConverterImpl<TMessage> :
        SendObjectConverter
        where TMessage : class
    {
        public void Send(IEndpoint endpoint, object message)
        {
            if (endpoint == null)
                throw new ArgumentNullException("endpoint");
            if (message == null)
                throw new ArgumentNullException("message");

            var msg = message as TMessage;
            if (msg == null)
                throw new ArgumentException("Unexpected message type: " + message.GetType().ToShortTypeName());

            endpoint.Send(msg);
        }

        public void Send(IEndpoint endpoint, object message, Action<ISendContext> contextCallback)
        {
            if (endpoint == null)
                throw new ArgumentNullException("endpoint");
            if (message == null)
                throw new ArgumentNullException("message");

            var msg = message as TMessage;
            if (msg == null)
                throw new ArgumentException("Unexpected message type: " + message.GetType().ToShortTypeName());

            endpoint.Send(msg, context => contextCallback(context));
        }
    }
}