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
    using Microsoft.Extensions.Logging;


    /// <summary>
    /// Consumer that handles the SuperviseJob message to check the status of the job
    /// </summary>
    public class JobSupervisor<T> :
        IConsumer<SuperviseJob<T>>,
        IConsumer<CancelJob>
        where T : class
    {
        static readonly ILogger _logger = Logger.Get<JobSupervisor<T>>();

        readonly IJobRegistry _registry;
        readonly IJobService _service;

        public JobSupervisor(IJobService service, IJobRegistry registry)
        {
            _registry = registry;
            _service = service;
        }

        public async Task Consume(ConsumeContext<CancelJob> context)
        {
            if (!_registry.TryGetJob(context.Message.JobId, out var jobHandle))
                throw new JobNotFoundException($"The JobId {context.Message.JobId} was not found.");

            _logger.LogDebug("Cancelling job: {0}", jobHandle.JobId);

            await jobHandle.Cancel().ConfigureAwait(false);

            JobHandle removed;
            _registry.TryRemoveJob(jobHandle.JobId, out removed);

            await jobHandle.NotifyCanceled("Job Service Stopped").ConfigureAwait(false);
        }

        public async Task Consume(ConsumeContext<SuperviseJob<T>> context)
        {
            if (_registry.TryGetJob(context.Message.JobId, out var jobHandle))
            {
                JobHandle removed;
                switch (jobHandle.Status)
                {
                    case JobStatus.Created:
                    case JobStatus.Running:
                        await _service.ScheduleSupervision(context, context.Message.Command, jobHandle).ConfigureAwait(false);
                        break;

                    case JobStatus.RanToCompletion:
                        _registry.TryRemoveJob(jobHandle.JobId, out removed);
                        break;

                    case JobStatus.Faulted:
                        _registry.TryRemoveJob(jobHandle.JobId, out removed);
                        break;

                    case JobStatus.Canceled:
                        _registry.TryRemoveJob(jobHandle.JobId, out removed);
                        break;
                }
            }
            else
            {
                _logger.LogWarning("JobId not found: {0}", context.Message.JobId);
            }
        }
    }
}
