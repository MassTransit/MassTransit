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


    public abstract class BasePipeContext
    {
        readonly CancellationTokenSource _cancellationTokenSource;
        readonly IPayloadCache _payloadCache;

        protected BasePipeContext(IPayloadCache payloadCache, CancellationToken cancellationToken)
        {
            CancellationToken = cancellationToken;

            _payloadCache = payloadCache;
        }

        protected BasePipeContext(IPayloadCache payloadCache)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            CancellationToken = _cancellationTokenSource.Token;

            _payloadCache = payloadCache;
        }

        public CancellationToken CancellationToken { get; }

        public virtual bool HasPayloadType(Type contextType)
        {
            if (contextType.IsInstanceOfType(this))
                return true;

            return _payloadCache.HasPayloadType(contextType);
        }

        public virtual bool TryGetPayload<TPayload>(out TPayload context)
            where TPayload : class
        {
            context = this as TPayload;
            if (context != null)
                return true;

            return _payloadCache.TryGetPayload(out context);
        }

        public virtual TPayload GetOrAddPayload<TPayload>(PayloadFactory<TPayload> payloadFactory)
            where TPayload : class
        {
            var context = this as TPayload;
            if (context != null)
                return context;

            return _payloadCache.GetOrAddPayload(payloadFactory);
        }

        /// <summary>
        /// Cancel the cancellation token
        /// </summary>
        public void Cancel()
        {
            _cancellationTokenSource?.Cancel();
        }
    }
}