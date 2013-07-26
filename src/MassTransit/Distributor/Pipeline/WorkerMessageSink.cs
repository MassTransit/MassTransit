// Copyright 2007-2012 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Distributor.Pipeline
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Context;
    using Magnum.Reflection;
    using MassTransit.Pipeline;
    using Messages;

    public class WorkerMessageSink<TMessage> :
        IPipelineSink<IConsumeContext<Distributed<TMessage>>>
        where TMessage : class
    {
        readonly IPipelineSink<IConsumeContext<TMessage>> _output;
        readonly IWorkerLoad<TMessage> _workerLoad;

        public WorkerMessageSink(IWorkerLoad<TMessage> workerLoad, IPipelineSink<IConsumeContext<TMessage>> output)
        {
            _workerLoad = workerLoad;
            _output = output;
        }

        public IEnumerable<Action<IConsumeContext<Distributed<TMessage>>>> Enumerate(
            IConsumeContext<Distributed<TMessage>> context)
        {
            return _workerLoad.GetWorker(context, Handle).DefaultIfEmpty(RetryLaterHandler);
        }

        public bool Inspect(IPipelineInspector inspector)
        {
            return inspector.Inspect(this);
        }

        void RetryLaterHandler(IConsumeContext<Distributed<TMessage>> context)
        {
            context.RetryLater();
        }

        IEnumerable<Action<IConsumeContext<Distributed<TMessage>>>> Handle(
            IConsumeContext<Distributed<TMessage>> context)
        {
            TMessage payload = context.Message.Payload;
            if (payload == null)
                payload = FastActivator<TMessage>.Create();

            var payloadContext = new ConsumeContext<TMessage>(context.BaseContext, payload);

            return _output.Enumerate(payloadContext)
                .Select(handler => (Action<IConsumeContext<Distributed<TMessage>>>)(x => handler(payloadContext)));
        }
    }
}