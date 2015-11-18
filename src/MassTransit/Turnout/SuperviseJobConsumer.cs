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
    using Commands;
    using Contracts;
    using Logging;
    using Util;


    /// <summary>
    /// Consumer that handles the SuperviseJob message to check the status of the job
    /// </summary>
    public class SuperviseJobConsumer :
        IConsumer<SuperviseJob>
    {
        static readonly ILog _log = Logger.Get<SuperviseJobConsumer>();
        readonly TimeSpan _checkInterval;

        readonly IJobRoster _roster;

        public SuperviseJobConsumer(IJobRoster roster, TimeSpan checkInterval)
        {
            _roster = roster;

            _checkInterval = checkInterval;
        }

        public Task Consume(ConsumeContext<SuperviseJob> context)
        {
            JobHandle jobHandle;
            if (_roster.TryGetJob(context.Message.JobId, out jobHandle))
            {
                switch (jobHandle.Status)
                {
                    case JobStatus.Created:
                    case JobStatus.Running:
                        var timestamp = DateTime.UtcNow;

                        DateTime scheduledTime = timestamp + _checkInterval;

                        var supervise = new Supervise(context.Message.JobId, timestamp, JobStatus.Running);

                        context.ScheduleMessage(scheduledTime, supervise);

                        if (_log.IsDebugEnabled)
                            _log.DebugFormat("Scheduled next supervise message: {0}", context.Message.JobId);
                        break;

                    case JobStatus.RanToCompletion:
                        _roster.RemoveJob(context.Message.JobId);
                        break;

                    case JobStatus.Faulted:
                        _roster.RemoveJob(context.Message.JobId);
                        break;

                    case JobStatus.Canceled:
                        _roster.RemoveJob(context.Message.JobId);
                        break;
                }
            }
            else
            {
                if (_log.IsWarnEnabled)
                    _log.WarnFormat("JobId not found: {0}", context.Message.JobId);
            }

            return TaskUtil.Completed;
        }
    }
}