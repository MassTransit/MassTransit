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
    using Contracts;


    public class CheckJobConsumer :
        IConsumer<CheckJobProgress>
    {
        readonly IJobRoster _roster;

        public CheckJobConsumer(IJobRoster roster)
        {
            _roster = roster;
        }

        public async Task Consume(ConsumeContext<CheckJobProgress> context)
        {
            IJobReference jobReference;
            if (_roster.TryGetJob(context.Message.JobId, out jobReference))
            {
                if (jobReference.Task.Status == TaskStatus.Running)
                {
                    var recheck = new Recheck(context.Message.JobId, JobStatus.Running);
                    await context.ScheduleMessage(DateTime.UtcNow + TimeSpan.FromSeconds(60), recheck);
                }
                else if (jobReference.Task.Status == TaskStatus.RanToCompletion)
                {
                }
            }
        }


        class Recheck :
            CheckJobProgress
        {
            public Recheck(Guid jobId, JobStatus status)
            {
                JobId = jobId;

                LastStatusCheck = DateTime.UtcNow;
                LastStatus = status;
            }

            public Guid JobId { get; }
            public DateTime LastStatusCheck { get; }
            public JobStatus LastStatus { get; }
        }
    }
}