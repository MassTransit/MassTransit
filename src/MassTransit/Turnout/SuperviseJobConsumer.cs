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
    using Logging;


    /// <summary>
    /// Consumer that handles the SuperviseJob message to check the status of the job
    /// </summary>
    public class SuperviseJobConsumer<T> :
        IConsumer<SuperviseJob<T>>
        where T : class
    {
        static readonly ILog _log = Logger.Get<SuperviseJobConsumer<T>>();

        readonly ITurnoutController _controller;
        readonly IJobRoster _roster;

        public SuperviseJobConsumer(IJobRoster roster, ITurnoutController controller)
        {
            _roster = roster;
            _controller = controller;
        }

        public async Task Consume(ConsumeContext<SuperviseJob<T>> context)
        {
            JobHandle jobHandle;
            if (_roster.TryGetJob(context.Message.JobId, out jobHandle))
            {
                if (jobHandle.ExecutionId != context.Message.ExecutionId)
                {
                    // handle not the right job
                    if (_log.IsWarnEnabled)
                        _log.WarnFormat("JobId found, but ExecutionId did not match: {0}/{1}", context.Message.JobId, context.Message.ExecutionId);
                }

                switch (jobHandle.Status)
                {
                    case JobStatus.Created:
                    case JobStatus.Running:
                        await _controller.ScheduleSupervision(context, context.Message.Job, jobHandle).ConfigureAwait(false);
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
        }
    }
}