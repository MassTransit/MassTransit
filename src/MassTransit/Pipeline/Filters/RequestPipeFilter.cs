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
namespace MassTransit.Pipeline.Filters
{
    using System.Diagnostics;
    using System.Threading.Tasks;
    using GreenPipes;


    /// <summary>
    /// Captures the pipe for a request response message so that it can be dispatched upon receipt of
    /// a correlated message.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    public class RequestPipeFilter<T, TKey> :
        IFilter<ConsumeContext<T>>
        where T : class
    {
        readonly TKey _key;
        readonly IPipe<ConsumeContext<T>> _pipe;

        public RequestPipeFilter(TKey key, IPipe<ConsumeContext<T>> pipe)
        {
            _key = key;
            _pipe = pipe;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            ProbeContext scope = context.CreateFilterScope("request");
            scope.Add("key", _key);
        }

        [DebuggerNonUserCode]
        public async Task Send(ConsumeContext<T> context, IPipe<ConsumeContext<T>> next)
        {
            await _pipe.Send(context).ConfigureAwait(false);

            await next.Send(context).ConfigureAwait(false);
        }
    }
}