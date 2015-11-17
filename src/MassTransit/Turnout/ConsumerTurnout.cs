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
        readonly IJobRoster _roster;

        public ConsumerTurnout()
        {
            _roster = new JobRoster();
        }

        Task<TramJob<T>> IConsumerTurnout.CreateJob<T>(ConsumeContext<T> context, IJobFactory<T> jobFactory)
        {
            var jobContext = new ConsumerJobContext<T>(context);

            var babyTask = Run(jobContext, jobFactory);

            var jobReference = new JobReference(jobContext, babyTask);

            _roster.Add(jobContext.JobId, jobReference);

            return Task.FromResult<TramJob<T>>(new Job<T>(context, jobReference));
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
    }


    class Job<T> :
        TramJob<T>
        where T : class
    {
        readonly ConsumeContext<T> _context;
        readonly IJobReference _jobReference;

        public Job(ConsumeContext<T> context, IJobReference jobReference)
        {
            _context = context;
            _jobReference = jobReference;
        }

        public Task Task => _jobReference.Task;

        public T Command => _context.Message;

        public Guid JobId => _jobReference.JobId;
    }
}