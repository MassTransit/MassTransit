// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Pipeline.Pipes
{
    using System;
    using System.Threading.Tasks;
    using Filters;


    public class SendPipe :
        ISendPipe
    {
        static readonly ISendPipe _empty = new SendPipe(new MessageTypeSendFilter(), Pipe.Empty<SendContext>());

        readonly MessageTypeSendFilter _filter;
        readonly IPipe<SendContext> _pipe;

        public SendPipe(MessageTypeSendFilter messageTypeSendFilter, IPipe<SendContext> pipe)
        {
            if (messageTypeSendFilter == null)
                throw new ArgumentNullException(nameof(messageTypeSendFilter));
            if (pipe == null)
                throw new ArgumentNullException(nameof(pipe));

            _filter = messageTypeSendFilter;
            _pipe = pipe;
        }

        public static ISendPipe Empty => _empty;

        void IProbeSite.Probe(ProbeContext context)
        {
            ProbeContext scope = context.CreateScope("sendPipe");

            _pipe.Probe(scope);
        }

        Task IPipe<SendContext>.Send(SendContext context)
        {
            return _pipe.Send(context);
        }

        ConnectHandle ISendMessageObserverConnector.ConnectSendMessageObserver<TMessage>(ISendMessageObserver<TMessage> observer)
        {
            return _filter.ConnectSendMessageObserver(observer);
        }

        ConnectHandle ISendObserverConnector.ConnectSendObserver(ISendObserver observer)
        {
            return _filter.ConnectSendObserver(observer);
        }
    }
}