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


    public class BasePipeContextProxyScope
    {
        readonly PipeContext _context;
        readonly PayloadCache _payloadCache;
        readonly object _self;

        public BasePipeContextProxyScope(object self, PipeContext context)
        {
            _self = self;
            _context = context;
            CancellationToken = context.CancellationToken;

            _payloadCache = new PayloadCache();
        }

        public BasePipeContextProxyScope(PipeContext context, CancellationToken cancellationToken)
        {
            _context = context;
            CancellationToken = cancellationToken;

            _payloadCache = new PayloadCache();
        }

        public CancellationToken CancellationToken { get; }

        public virtual bool HasPayloadType(Type contextType)
        {
            if (contextType.IsInstanceOfType(_self))
                return true;

            return _payloadCache.HasPayloadType(contextType) || _context.HasPayloadType(contextType);
        }

        public virtual bool TryGetPayload<TPayload>(out TPayload context)
            where TPayload : class
        {
            context = _self as TPayload;
            if (context != null)
                return true;

            return _payloadCache.TryGetPayload(out context) || _context.TryGetPayload(out context);
        }

        public virtual TPayload GetOrAddPayload<TPayload>(PayloadFactory<TPayload> payloadFactory)
            where TPayload : class
        {
            var context = _self as TPayload;
            if (context != null)
                return context;

            TPayload payload;
            if (_payloadCache.TryGetPayload(out payload))
                return payload;

            if (_context.TryGetPayload(out payload))
                return payload;

            return _payloadCache.GetOrAddPayload(payloadFactory);
        }
    }
}