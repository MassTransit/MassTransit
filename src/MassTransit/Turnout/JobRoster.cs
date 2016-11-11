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
    using System.Collections.Concurrent;
    using Logging;


    /// <summary>
    /// Maintains the jobs for a turnout
    /// </summary>
    public class JobRoster :
        IJobRoster
    {
        static readonly ILog _log = Logger.Get<JobRoster>();

        readonly ConcurrentDictionary<Guid, JobHandle> _jobs;

        public JobRoster()
        {
            _jobs = new ConcurrentDictionary<Guid, JobHandle>();
        }

        public bool TryGetJob(Guid jobId, out JobHandle jobReference)
        {
            return _jobs.TryGetValue(jobId, out jobReference);
        }

        public void Add(Guid jobId, JobHandle jobReference)
        {
            if (!_jobs.TryAdd(jobId, jobReference))
                throw new JobAlreadyExistsException(jobId);
        }

        public void RemoveJob(Guid jobId)
        {
            JobHandle jobHandle;
            bool removed = _jobs.TryRemove(jobId, out jobHandle);

            if (removed)
            {
                if (_log.IsDebugEnabled)
                    _log.DebugFormat("Removed job: {0} ({1})", jobId, jobHandle.Status);
            }
        }
    }
}