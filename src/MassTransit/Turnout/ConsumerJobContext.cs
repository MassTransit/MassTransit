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
namespace MassTransit.Turnout
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using Context;


    /// <summary>
    /// A job context created from a consumer's consume context
    /// </summary>
    /// <typeparam name="TInput">The input type for the job</typeparam>
    public class ConsumerJobContext<TInput> :
        ConsumeContextProxy<TInput>,
        JobContext<TInput>,
        IDisposable
        where TInput : class
    {
        readonly CancellationTokenSource _cancellationTokenSource;
        readonly ConsumeContext<TInput> _context;
        readonly PayloadCache _payloadCache;
        readonly Stopwatch _stopwatch;

        public ConsumerJobContext(ConsumeContext<TInput> context)
            : base(context)
        {
            _context = context;

            _cancellationTokenSource = new CancellationTokenSource();
            _payloadCache = new PayloadCache();

            JobId = NewId.NextGuid();
            _stopwatch = Stopwatch.StartNew();
        }

        public void Dispose()
        {
            _cancellationTokenSource.Dispose();
        }

        public Guid JobId { get; }

        public TimeSpan ElapsedTime => _stopwatch.Elapsed;

        public override CancellationToken CancellationToken => _cancellationTokenSource.Token;

        bool PipeContext.HasPayloadType(Type contextType)
        {
            return _payloadCache.HasPayloadType(contextType) || _context.HasPayloadType(contextType);
        }

        bool PipeContext.TryGetPayload<TPayload>(out TPayload context)
        {
            if (_payloadCache.TryGetPayload(out context))
                return true;

            return _context.TryGetPayload(out context);
        }

        TPayload PipeContext.GetOrAddPayload<TPayload>(PayloadFactory<TPayload> payloadFactory)
        {
            TPayload payload;
            if (_payloadCache.TryGetPayload(out payload))
                return payload;

            if (_context.TryGetPayload(out payload))
                return payload;

            return _payloadCache.GetOrAddPayload(payloadFactory);
        }
    }
}