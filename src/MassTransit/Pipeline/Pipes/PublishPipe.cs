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
    using GreenPipes;


    public class PublishPipe :
        IPublishPipe
    {
        static readonly IPublishPipe _empty = new PublishPipe(new MessageTypePublishFilter(), Pipe.Empty<PublishContext>());
        readonly MessageTypePublishFilter _filter;
        readonly IPipe<PublishContext> _pipe;

        public PublishPipe(MessageTypePublishFilter messageTypePublishFilter, IPipe<PublishContext> pipe)
        {
            if (messageTypePublishFilter == null)
                throw new ArgumentNullException(nameof(messageTypePublishFilter));
            if (pipe == null)
                throw new ArgumentNullException(nameof(pipe));

            _filter = messageTypePublishFilter;
            _pipe = pipe;
        }

        public static IPublishPipe Empty => _empty;

        void IProbeSite.Probe(ProbeContext context)
        {
            ProbeContext scope = context.CreateScope("sendPipe");

            _pipe.Probe(scope);
        }

        Task IPipe<PublishContext>.Send(PublishContext context)
        {
            return _pipe.Send(context);
        }

        ConnectHandle IPublishMessageObserverConnector.ConnectPublishMessageObserver<TMessage>(IPublishMessageObserver<TMessage> observer)
        {
            return _filter.ConnectPublishMessageObserver(observer);
        }

        ConnectHandle IPublishObserverConnector.ConnectPublishObserver(IPublishObserver observer)
        {
            return _filter.ConnectPublishObserver(observer);
        }
    }
}