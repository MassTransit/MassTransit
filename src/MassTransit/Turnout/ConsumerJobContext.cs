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
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using Contracts;
    using Courier;
    using Events;
    using Newtonsoft.Json.Linq;


    /// <summary>
    /// A job context created from a consumer's consume context
    /// </summary>
    /// <typeparam name="TCommand">The input type for the job</typeparam>
    public class ConsumerJobContext<TCommand> :
        ConsumeContextProxy,
        JobContext<TCommand>,
        IDisposable
        where TCommand : class
    {
        readonly ConsumeContext _context;
        readonly CancellationTokenSource _source;
        readonly Stopwatch _stopwatch;

        public ConsumerJobContext(ConsumeContext context, Guid jobId, TCommand command)
            : base(context)
        {
            _context = context;
            JobId = jobId;
            Command = command;

            _stopwatch = Stopwatch.StartNew();

            _source = new CancellationTokenSource();
        }

        public void Dispose()
        {
            _source.Dispose();
        }

        public override CancellationToken CancellationToken => _source.Token;

        public Guid JobId { get; }
        public TCommand Command { get; }
        public TimeSpan ElapsedTime => _stopwatch.Elapsed;

        public Task NotifyCanceled(string reason = null)
        {
            return _context.Publish<JobCanceled<TCommand>>(new JobCanceledEvent<TCommand>(JobId, Command));
        }

        public Task NotifyStarted(Uri managementAddress)
        {
            return _context.Publish<JobStarted>(new JobStartedEvent(JobId, 0, managementAddress, GetObjectAsDictionary(Command)));
        }

        public Task NotifyCompleted()
        {
            return _context.Publish<JobCompleted>(new JobCompletedEvent(JobId, GetObjectAsDictionary(Command), new Dictionary<string, object>()));
        }

        public Task NotifyFaulted(Exception exception)
        {
            return _context.Publish<JobFaulted>(new JobFaultedEvent<TCommand>(JobId, Command, exception));
        }

        public void Cancel()
        {
            _source.Cancel();
        }

        static IDictionary<string, object> GetObjectAsDictionary(object values)
        {
            if (values == null)
                return new Dictionary<string, object>();

            var dictionary = JObject.FromObject(values, SerializerCache.Serializer);

            return dictionary.ToObject<IDictionary<string, object>>();
        }
    }
}