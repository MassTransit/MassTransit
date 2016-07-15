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
namespace MassTransit.Context
{
    using System;
    using System.Threading;


    public class PayloadCacheScope :
        IPayloadCache,
        PipeContext
    {
        readonly PipeContext _context;
        readonly IPayloadCache _payloadCache;

        public PayloadCacheScope(PipeContext context)
        {
            _context = context;
            CancellationToken = context.CancellationToken;

            _payloadCache = new PayloadCache();
        }

        public virtual bool HasPayloadType(Type contextType)
        {
            return _payloadCache.HasPayloadType(contextType) || _context.HasPayloadType(contextType);
        }

        public virtual bool TryGetPayload<TPayload>(out TPayload context)
            where TPayload : class
        {
            return _payloadCache.TryGetPayload(out context) || _context.TryGetPayload(out context);
        }

        public virtual TPayload GetOrAddPayload<TPayload>(PayloadFactory<TPayload> payloadFactory)
            where TPayload : class
        {
            TPayload payload;
            if (_payloadCache.TryGetPayload(out payload))
                return payload;

            if (_context.TryGetPayload(out payload))
                return payload;

            return _payloadCache.GetOrAddPayload(payloadFactory);
        }

        public IPayloadCache CreateScope()
        {
            return new PayloadCacheScope(this);
        }

        public CancellationToken CancellationToken { get; }
    }
}