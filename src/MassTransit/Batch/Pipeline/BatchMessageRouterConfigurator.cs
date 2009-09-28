// Copyright 2007-2008 The Apache Software Foundation.
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
namespace MassTransit.Batch.Pipeline
{
    using Exceptions;
    using MassTransit.Pipeline;
    using MassTransit.Pipeline.Configuration;
    using MassTransit.Pipeline.Sinks;

    public class BatchMessageRouterConfigurator
    {
        private readonly IPipelineSink<object> _sink;

        private BatchMessageRouterConfigurator(IPipelineSink<object> sink)
        {
            _sink = sink;
        }

        public BatchMessageRouter<TMessage, TBatchId> FindOrCreate<TMessage, TBatchId>()
            where TMessage : class, BatchedBy<TBatchId>
        {
            MessageRouterConfigurator configurator = MessageRouterConfigurator.For(_sink);

            var router = configurator.FindOrCreate<TMessage>();

            var scope = new BatchMessageRouterConfiguratorScope<TMessage, TBatchId>();

            router.Inspect(scope);

            return scope.Router ?? ConfigureRouter<TMessage, TBatchId>(router);
        }

        private static BatchMessageRouter<TMessage, TBatchId> ConfigureRouter<TMessage, TBatchId>(MessageRouter<TMessage> messageRouter)
            where TMessage : class, BatchedBy<TBatchId>
        {
            if (messageRouter == null)
                throw new PipelineException("The base object router was not found");

            BatchMessageRouter<TMessage, TBatchId> router = new BatchMessageRouter<TMessage, TBatchId>();

            messageRouter.Connect(router);

            return router;
        }

        public static BatchMessageRouterConfigurator For<TMessage>(IPipelineSink<TMessage> sink)
            where TMessage : class
        {
            return new BatchMessageRouterConfigurator(sink.TranslateTo<IPipelineSink<object>>());
        }
    }
}