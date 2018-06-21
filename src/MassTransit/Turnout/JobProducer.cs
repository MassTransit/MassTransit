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
    using System.Threading.Tasks;
    using Contracts;


    /// <summary>
    /// The consumer that creates the job using the turnout host
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class JobProducer<T> :
        IConsumer<T>,
        IConsumer<SubmitJob<T>>
        where T : class
    {
        readonly IJobFactory<T> _jobFactory;
        readonly IJobService _jobService;

        public JobProducer(IJobService jobService, IJobFactory<T> jobFactory)
        {
            _jobService = jobService;
            _jobFactory = jobFactory;
        }

        public async Task Consume(ConsumeContext<SubmitJob<T>> context)
        {
            JobHandle<T> job = await _jobService.CreateJob(context, context.Message.JobId, context.Message.Command, _jobFactory).ConfigureAwait(false);
        }

        public async Task Consume(ConsumeContext<T> context)
        {
            JobHandle<T> job = await _jobService.CreateJob(context, NewId.NextGuid(), context.Message, _jobFactory).ConfigureAwait(false);
        }
    }
}