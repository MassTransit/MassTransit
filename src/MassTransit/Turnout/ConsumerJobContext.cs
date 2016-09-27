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
        ConsumeContextProxyScope<TInput>,
        JobContext<TInput>,
        IDisposable
        where TInput : class
    {
        readonly CancellationTokenSource _source;
        readonly Stopwatch _stopwatch;

        public ConsumerJobContext(ConsumeContext<TInput> context)
            : base(context)
        {
            JobId = NewId.NextGuid();
            _stopwatch = Stopwatch.StartNew();

            _source = new CancellationTokenSource();
        }

        public void Dispose()
        {
            _source.Dispose();
        }

        public override CancellationToken CancellationToken => _source.Token;

        public Guid JobId { get; }

        public TimeSpan ElapsedTime => _stopwatch.Elapsed;

        public void Cancel()
        {
            _source.Cancel();
        }
    }
}