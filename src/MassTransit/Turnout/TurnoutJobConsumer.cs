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
    using Logging;


    public class TurnoutJobConsumer :
        IConsumer<CheckJobProgress>
    {
        static readonly ILog _log = Logger.Get<TurnoutJobConsumer>();
        readonly TimeSpan _checkInterval;

        readonly IJobRoster _roster;

        public TurnoutJobConsumer(IJobRoster roster, TimeSpan checkInterval)
        {
            _roster = roster;

            _checkInterval = checkInterval;
        }

        public async Task Consume(ConsumeContext<CheckJobProgress> context)
        {
            JobHandle jobHandle;
            if (_roster.TryGetJob(context.Message.JobId, out jobHandle))
            {
                switch (jobHandle.Status)
                {
                    case JobStatus.Created:
                    case JobStatus.Running:
                        DateTime scheduledTime = DateTime.UtcNow + _checkInterval;

                        var recheck = new Recheck(context.Message.JobId, JobStatus.Running);

                        await context.ScheduleMessage(scheduledTime, recheck);

                        if (_log.IsDebugEnabled)
                            _log.DebugFormat("Scheduled next job check for {0} at {1}", context.Message.JobId, scheduledTime);
                        break;
                    case JobStatus.RanToCompletion:
                        _roster.RemoveJob(context.Message.JobId);

                        if (_log.IsDebugEnabled)
                            _log.DebugFormat("Removing completed job: {0}", context.Message.JobId);
                        break;
                    default:
                        if (_log.IsDebugEnabled)
                            _log.DebugFormat("Unknown Job Status: {0} ({1})", context.Message.JobId, jobHandle.Status);
                        break;
                }
            }
            else
            {
                if (_log.IsDebugEnabled)
                    _log.DebugFormat("JobId not found: {0}", context.Message.JobId);
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