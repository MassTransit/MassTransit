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
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Commands;
    using Contracts;
    using Courier;
    using Events;
    using GreenPipes;
    using Logging;
    using Newtonsoft.Json.Linq;
    using Util;


    public class TurnoutController :
        ITurnoutController
    {
        static readonly ILog _log = Logger.Get<TurnoutController>();

        readonly Uri _controlAddress;
        readonly IJobRoster _roster;
        readonly TimeSpan _superviseInterval;

        public TurnoutController(IJobRoster jobRoster, Uri controlAddress, TimeSpan superviseInterval)
        {
            _superviseInterval = superviseInterval;
            _roster = jobRoster;
            _controlAddress = controlAddress;
        }

        async Task<JobHandle<T>> ITurnoutController.CreateJob<T>(ConsumeContext<T> context, IJobFactory<T> jobFactory)
        {
            var jobContext = new ConsumerJobContext<T>(context);

            var babyTask = Run(jobContext, jobFactory);

            var jobHandle = new ConsumerJobHandle<T>(jobContext, babyTask);

            _roster.Add(jobContext.JobId, jobHandle);

            await ScheduleSupervision(context, context.Message, jobHandle).ConfigureAwait(false);

            return jobHandle;
        }

        public Task ScheduleSupervision<T>(ConsumeContext context, T job, JobHandle jobHandle)
            where T : class
        {
            var utcNow = DateTime.UtcNow;
            var scheduledTime = utcNow + _superviseInterval;

            var check = new SuperviseJobCommand<T>(jobHandle.JobId, jobHandle.ExecutionId, job, utcNow, jobHandle.Status);

            if (_log.IsDebugEnabled)
                _log.DebugFormat("Scheduled Job Supervision: {0}-{1}", jobHandle.JobId.ToString("N"), typeof(T).Name);

            return context.ScheduleSend(_controlAddress, scheduledTime, check);
        }

        async Task Run<T>(ConsumerJobContext<T> jobContext, IJobFactory<T> jobFactory)
            where T : class
        {
            try
            {
                IPipe<JobContext<T>> nextPipe = Pipe.ExecuteAsync<JobContext<T>>(async context =>
                {
                    var arguments = GetObjectAsDictionary(context.Message);

                    await context.Publish<JobCompleted>(new JobCompletedEvent(context.JobId, arguments, new Dictionary<string, object>())).ConfigureAwait(false);
                });

                await jobFactory.Execute(jobContext, nextPipe).ConfigureAwait(false);
            }
            catch (TaskCanceledException)
            {
                await NotifyCanceled(jobContext).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                await NotifyCanceled(jobContext).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                JobContext<T> context = jobContext;

                await context.Publish<JobFaulted<T>>(new Faulted<T>(jobContext.JobId, jobContext.Message, exception)).ConfigureAwait(false);
            }
            finally
            {
                jobContext.Dispose();
            }
        }

        static async Task NotifyCanceled<T>(ConsumerJobContext<T> jobContext) where T : class
        {
            JobContext<T> context = jobContext;

            await context.Publish<JobCanceled<T>>(new Canceled<T>(jobContext.JobId, jobContext.Message)).ConfigureAwait(false);
        }

        IDictionary<string, object> GetObjectAsDictionary(object values)
        {
            if (values == null)
                return new Dictionary<string, object>();

            var dictionary = JObject.FromObject(values, SerializerCache.Serializer);

            return dictionary.ToObject<IDictionary<string, object>>();
        }
    }
}