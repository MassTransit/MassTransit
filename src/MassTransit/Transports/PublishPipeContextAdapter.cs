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
namespace MassTransit.Transports
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using Pipeline;
    using Util;


    public class PublishPipeContextAdapter<T> :
        IPipe<SendContext<T>>
        where T : class
    {
        readonly Guid? _conversationId;
        readonly Guid? _correlationId;
        readonly IPublishPipe _publishPipe;
        readonly IPublishObserver _observer;
        readonly IPipe<PublishContext<T>> _pipe;
        readonly Uri _sourceAddress;
        PublishContext<T> _context;

        public PublishPipeContextAdapter(IPipe<PublishContext<T>> pipe, IPublishPipe publishPipe, IPublishObserver observer, Uri sourceAddress, Guid? correlationId, Guid? conversationId)
        {
            _pipe = pipe;
            _publishPipe = publishPipe;
            _observer = observer;
            _sourceAddress = sourceAddress;
            _correlationId = correlationId;
            _conversationId = conversationId;
        }

        public PublishPipeContextAdapter(IPipe<PublishContext> pipe, IPublishPipe publishPipe, IPublishObserver observer, Uri sourceAddress, Guid? correlationId, Guid? conversationId)
        {
            _pipe = pipe;
            _publishPipe = publishPipe;
            _observer = observer;
            _sourceAddress = sourceAddress;
            _correlationId = correlationId;
            _conversationId = conversationId;
        }

        public PublishPipeContextAdapter(IPublishPipe publishPipe, IPublishObserver observer, Uri sourceAddress, Guid? correlationId, Guid? conversationId)
        {
            _pipe = Pipe.Empty<PublishContext<T>>();
            _publishPipe = publishPipe;
            _observer = observer;
            _sourceAddress = sourceAddress;
            _correlationId = correlationId;
            _conversationId = conversationId;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            _pipe.Probe(context);
        }

        public async Task Send(SendContext<T> context)
        {
            context.SourceAddress = _sourceAddress;

            if (_conversationId.HasValue)
                context.ConversationId = _conversationId;

            if (_correlationId.HasValue)
                context.InitiatorId = _correlationId;

            var publishContext = new PublishContextProxy<T>(context, context.Message);
            bool firstTime = Interlocked.CompareExchange(ref _context, publishContext, null) == null;

            await _publishPipe.Send(publishContext);

            await _pipe.Send(publishContext);

            if (firstTime)
                await _observer.PrePublish(publishContext);
        }

        public Task PostPublish()
        {
            if (_context != null)
                return _observer.PostPublish(_context);

            return TaskUtil.Completed;
        }

        public Task PublishFaulted(Exception exception)
        {
            if (_context != null)
                return _observer.PublishFault(_context, exception);

            return TaskUtil.Completed;
        }
    }
}