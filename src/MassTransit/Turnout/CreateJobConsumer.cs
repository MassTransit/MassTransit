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
    using System.Threading.Tasks;


    public interface TramJob<out T>
        where T : class
    {
        /// <summary>
        /// The message that initiated the job
        /// </summary>
        T Command { get; }

        /// <summary>
        /// The identifier assigned to the job
        /// </summary>
        Guid JobId { get; }


        
    }


    public class CreateJobConsumer<T> :
        IConsumer<T>
        where T : class

    {
        readonly IConsumerTurnout _consumerTurnout;
        readonly IJobFactory<T> _jobFactory;

        public CreateJobConsumer(IConsumerTurnout consumerTurnout, IJobFactory<T> jobFactory)
        {
            _consumerTurnout = consumerTurnout;
            _jobFactory = jobFactory;
        }

        public async Task Consume(ConsumeContext<T> context)
        {
            var job = await _consumerTurnout.CreateJob(context, _jobFactory);
        }
    }
}