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


    public class PublishPipeContextAdapter<T> :
        IPipe<SendContext<T>>
        where T : class
    {
        readonly PublishObservable _observer;
        readonly IPipe<PublishContext<T>> _pipe;
        readonly Uri _sourceAddress;
        PublishContext<T> _context;

        public PublishPipeContextAdapter(IPipe<PublishContext<T>> pipe, PublishObservable observer, Uri sourceAddress)
        {
            _pipe = pipe;
            _observer = observer;
            _sourceAddress = sourceAddress;
        }

        public PublishPipeContextAdapter(IPipe<PublishContext> pipe, PublishObservable observer, Uri sourceAddress)
        {
            _pipe = pipe;
            _observer = observer;
            _sourceAddress = sourceAddress;
        }

        public PublishPipeContextAdapter(PublishObservable observer, Uri sourceAddress)
        {
            _observer = observer;
            _sourceAddress = sourceAddress;
            _pipe = Pipe.Empty<PublishContext<T>>();
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            _pipe.Probe(context);
        }

        public async Task Send(SendContext<T> context)
        {
            context.SourceAddress = _sourceAddress;

            var publishContext = new PublishContextProxy<T>(context);
            bool firstTime = Interlocked.CompareExchange(ref _context, publishContext, null) == null;

            await _pipe.Send(publishContext);

            if (firstTime)
                _observer.NotifyPrePublish(publishContext);
        }

        public void PostPublish()
        {
            if (_context != null)
                _observer.NotifyPostPublish(_context);
        }

        public void PublishFaulted(Exception exception)
        {
            if (_context != null)
                _observer.NotifyPublishFault(_context, exception);
        }
    }
}