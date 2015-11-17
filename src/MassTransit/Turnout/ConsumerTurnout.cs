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
    using Events;


    public class ConsumerTurnout :
        IConsumerTurnout
    {
        readonly TimeSpan _checkInterval;
        readonly IJobRoster _roster;
        readonly Uri _controlAddress;

        public ConsumerTurnout(IJobRoster jobRoster, Uri controlAddress)
            : this(jobRoster, controlAddress, TimeSpan.FromMinutes(1))
        {
        }

        public ConsumerTurnout(IJobRoster jobRoster, Uri controlAddress, TimeSpan checkInterval)
        {
            _checkInterval = checkInterval;
            _roster = jobRoster;
            _controlAddress = controlAddress;
        }

        async Task<TramJob<T>> IConsumerTurnout.CreateJob<T>(ConsumeContext<T> context, IJobFactory<T> jobFactory)
        {
            var jobContext = new ConsumerJobContext<T>(context);

            var babyTask = Run(jobContext, jobFactory);

            var jobReference = new ConsumerJobHandle(jobContext, babyTask);

            _roster.Add(jobContext.JobId, jobReference);

            DateTime utcNow = DateTime.UtcNow;
            var scheduledTime = utcNow + _checkInterval;

            var check = new Check(jobContext.JobId, utcNow, jobReference.Status);

            await context.ScheduleMessage(_controlAddress, scheduledTime, check);

            return new Job<T>(context, jobReference);
        }

        async Task Run<T>(ConsumerJobContext<T> jobContext, IJobFactory<T> jobFactory)
            where T : class
        {
            try
            {
                var nextPipe = Pipe.ExecuteAsync<JobContext<T>>(async context =>
                {
                    await context.Publish<JobCompleted>(new
                    {
                        context.JobId,
                        Timestamp = DateTime.UtcNow
                    }).ConfigureAwait(false);
                });

                await jobFactory.Execute(jobContext, nextPipe);
            }
            catch (Exception exception)
            {
                JobContext<T> context = jobContext;
                await context.Publish<JobFaulted>(new
                {
                    jobContext.JobId,
                    Timestamp = DateTime.UtcNow,
                    Exceptions = (ExceptionInfo)(new FaultExceptionInfo(exception))
                }).ConfigureAwait(false);
            }
            finally
            {
                jobContext.Dispose();
            }
        }


        class Check :
            CheckJobProgress
        {
            public Check(Guid jobId, DateTime lastStatusCheck, JobStatus lastStatus)
            {
                JobId = jobId;
                LastStatusCheck = lastStatusCheck;
                LastStatus = lastStatus;
            }

            public Guid JobId { get; private set; }

            public DateTime LastStatusCheck { get; private set; }

            public JobStatus LastStatus { get; private set; }
        }
    }


    class Job<T> :
        TramJob<T>
        where T : class
    {
        readonly ConsumeContext<T> _context;
        readonly JobHandle _jobReference;

        public Job(ConsumeContext<T> context, JobHandle jobReference)
        {
            _context = context;
            _jobReference = jobReference;
        }

        public T Command => _context.Message;

        public Guid JobId => _jobReference.JobId;
    }
}