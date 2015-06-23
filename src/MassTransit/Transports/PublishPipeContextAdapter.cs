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
    using Monitoring.Introspection;
    using Pipeline;


    public class PublishPipeContextAdapter<T> :
        IPipe<SendContext<T>>
        where T : class
    {
        readonly IPublishObserver _observer;
        readonly IPipe<PublishContext<T>> _pipe;

        PublishContext<T> _context;

        public PublishPipeContextAdapter(IPipe<PublishContext<T>> pipe, IPublishObserver observer)
        {
            _pipe = pipe;
            _observer = observer;
        }

        public PublishPipeContextAdapter(IPipe<PublishContext> pipe, IPublishObserver observer)
        {
            _pipe = pipe;
            _observer = observer;
        }

        public PublishPipeContextAdapter(IPublishObserver observer)
        {
            _observer = observer;
            _pipe = Pipe.Empty<PublishContext<T>>();
        }

        Task IProbeSite.Probe(ProbeContext context)
        {
            return _pipe.Probe(context);
        }

        public async Task Send(SendContext<T> context)
        {
            var publishContext = new PublishContextProxy<T>(context);
            bool firstTime = Interlocked.CompareExchange(ref _context, publishContext, null) == null;

            await _pipe.Send(publishContext);

            if (firstTime)
                await _observer.PrePublish(publishContext);
        }

        public async Task PostSend()
        {
            if (_context != null)
                await _observer.PostPublish(_context);
        }

        public async Task SendFaulted(Exception exception)
        {
            if (_context != null)
                await _observer.PublishFault(_context, exception);
        }
    }
}