// Copyright 2007-2017 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.RabbitMqTransport.Pipeline
{
    using System;
    using System.Threading.Tasks;
    using Contracts;
    using GreenPipes;
    using Logging;
    using Management;
    using MassTransit.Pipeline;
    using Util;


    /// <summary>
    /// Prepares a queue for receiving messages using the ReceiveSettings specified.
    /// </summary>
    public class PrefetchCountFilter :
        IFilter<ModelContext>,
        ISetPrefetchCount
    {
        static readonly ILog _log = Logger.Get<PrefetchCountFilter>();

        readonly IConsumePipeConnector _managementPipe;
        ushort _prefetchCount;

        public PrefetchCountFilter(IConsumePipeConnector managementPipe, ushort prefetchCount)
        {
            _prefetchCount = prefetchCount;
            _managementPipe = managementPipe;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateFilterScope("prefetchCount");
            scope.Add("prefetchCount", _prefetchCount);
        }

        async Task IFilter<ModelContext>.Send(ModelContext context, IPipe<ModelContext> next)
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("Setting Prefetch Count: {0}", _prefetchCount);

            await context.BasicQos(0, _prefetchCount, true).ConfigureAwait(false);

            using (new SetPrefetchCountConsumer(_managementPipe, context, this))
            {
                await next.Send(context).ConfigureAwait(false);
            }
        }

        public Task SetPrefetchCount(ushort prefetchCount)
        {
            _prefetchCount = prefetchCount;

            return TaskUtil.Completed;
        }


        class SetPrefetchCountConsumer :
            IConsumer<SetPrefetchCount>,
            IDisposable
        {
            readonly ISetPrefetchCount _filter;
            readonly ConnectHandle _handle;
            readonly ModelContext _modelContext;

            public SetPrefetchCountConsumer(IConsumePipeConnector managementPipe, ModelContext modelContext, ISetPrefetchCount filter)
            {
                _modelContext = modelContext;
                _filter = filter;

                _handle = managementPipe.ConnectInstance(this);
            }

            async Task IConsumer<SetPrefetchCount>.Consume(ConsumeContext<SetPrefetchCount> context)
            {
                var prefetchCount = context.Message.PrefetchCount;

                if (_log.IsDebugEnabled)
                    _log.DebugFormat("Setting Prefetch Count: {0}", prefetchCount);

                await _modelContext.BasicQos(0, prefetchCount, true).ConfigureAwait(false);

                await _filter.SetPrefetchCount(prefetchCount).ConfigureAwait(false);
            }

            public void Dispose()
            {
                _handle.Dispose();
            }
        }
    }
}