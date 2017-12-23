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
    using System.Threading.Tasks;
    using Context;
    using GreenPipes;
    using Latest;


    /// <summary>
    /// Retains the last value that was sent through the filter, usable as a source to a join pipe
    /// </summary>
    public class LatestFilter<T> :
        IFilter<T>,
        ILatestFilter<T>
        where T : class, PipeContext
    {
        readonly TaskCompletionSource<bool> _hasValue;
        T _latest;

        public LatestFilter()
        {
            _hasValue = new TaskCompletionSource<bool>();
        }

        Task IFilter<T>.Send(T context, IPipe<T> next)
        {
            _latest = context;
            _hasValue.TrySetResult(true);

            return next.Send(context);
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            context.CreateFilterScope("last");
        }

        public Task<T> Latest => GetLatest();

        async Task<T> GetLatest()
        {
            await _hasValue.Task.ConfigureAwait(false);

            return _latest;
        }
    }
}