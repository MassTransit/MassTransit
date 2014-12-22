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
namespace Automatonymous
{
    using System;
    using System.Threading;
    using MassTransit;


    public class ConsumeEventContextImpl<TInstance, TData> :
        ConsumeEventContext<TInstance, TData>
        where TData : class
    {
        readonly ConsumeContext<TData> _consumeContext;
        readonly BehaviorContext<TInstance, TData> _context;

        public ConsumeEventContextImpl(BehaviorContext<TInstance, TData> context, ConsumeContext<TData> consumeContext)
        {
            _context = context;
            _consumeContext = consumeContext;
        }

        public CancellationToken CancellationToken
        {
            get { return _context.CancellationToken; }
        }

        public TInstance Instance
        {
            get { return _context.Instance; }
        }

        public bool HasPayloadType(Type contextType)
        {
            return _context.HasPayloadType(contextType);
        }

        public bool TryGetPayload<TPayload>(out TPayload payload) where TPayload : class
        {
            return _context.TryGetPayload(out payload);
        }

        public TPayload GetOrAddPayload<TPayload>(PayloadFactory<TPayload> payloadFactory) where TPayload : class
        {
            return _context.GetOrAddPayload(payloadFactory);
        }

        public Event<TData> Event
        {
            get { return _context.Event; }
        }

        public TData Data
        {
            get { return _context.Data; }
        }

        public ConsumeContext ConsumeContext
        {
            get { return _consumeContext; }
        }

        Event EventContext<TInstance>.Event
        {
            get { return Event; }
        }
    }


    public class ConsumeEventContextImpl<TInstance> :
        ConsumeEventContext<TInstance>
    {
        readonly ConsumeContext _consumeContext;
        readonly BehaviorContext<TInstance> _context;

        public ConsumeEventContextImpl(BehaviorContext<TInstance> context, ConsumeContext consumeContext)
        {
            _context = context;
            _consumeContext = consumeContext;
        }

        public CancellationToken CancellationToken
        {
            get { return _context.CancellationToken; }
        }

        public TInstance Instance
        {
            get { return _context.Instance; }
        }

        public bool HasPayloadType(Type contextType)
        {
            return _context.HasPayloadType(contextType);
        }

        public bool TryGetPayload<TPayload>(out TPayload payload) where TPayload : class
        {
            return _context.TryGetPayload(out payload);
        }

        public TPayload GetOrAddPayload<TPayload>(PayloadFactory<TPayload> payloadFactory) where TPayload : class
        {
            return _context.GetOrAddPayload(payloadFactory);
        }

        public ConsumeContext ConsumeContext
        {
            get { return _consumeContext; }
        }

        public Event Event
        {
            get { return _context.Event; }
        }
    }
}