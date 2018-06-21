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
    using System.Threading.Tasks;
    using GreenPipes;


    public class DelegateJobFactory<TInput> :
        IJobFactory<TInput>
        where TInput : class
    {
        readonly Func<JobContext<TInput>, Task> _factory;

        public DelegateJobFactory(Func<JobContext<TInput>, Task> factory)
        {
            _factory = factory;
        }

        public async Task Execute(JobContext<TInput> context, IPipe<JobContext<TInput>> next)
        {
            await _factory(context).ConfigureAwait(false);

            await next.Send(context).ConfigureAwait(false);
        }
    }


    public class DelegateJobFactory<TInput, TResult> :
        IJobFactory<TInput, TResult>
        where TInput : class
        where TResult : class
    {
        readonly Func<JobContext<TInput>, Task<TResult>> _factory;

        public DelegateJobFactory(Func<JobContext<TInput>, Task<TResult>> factory)
        {
            _factory = factory;
        }

        async Task IJobFactory<TInput, TResult>.Execute(JobContext<TInput, TResult> context, IPipe<JobResultContext<TInput, TResult>> next)
        {
            var result = await _factory(context).ConfigureAwait(false);

            await next.Send(context.FromResult(result)).ConfigureAwait(false);
        }
    }
}