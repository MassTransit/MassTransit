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
    /// The consumer that handles the control messages for the job
    /// </summary>
    public class CancelJobConsumer :
        IConsumer<CancelJob>

    {
        readonly IJobRoster _roster;

        public CancelJobConsumer(IJobRoster roster)
        {
            _roster = roster;
        }

        public async Task Consume(ConsumeContext<CancelJob> context)
        {
            JobHandle jobHandle;
            if (!_roster.TryGetJob(context.Message.JobId, out jobHandle))
                throw new JobNotFoundException($"The JobId {context.Message.JobId} was not found.");

            await jobHandle.Cancel().ConfigureAwait(false);
        }
    }
}