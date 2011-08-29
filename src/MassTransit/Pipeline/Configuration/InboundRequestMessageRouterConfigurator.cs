// Copyright 2007-2011 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Pipeline.Configuration
{
    using Exceptions;
    using Sinks;

    public class InboundRequestMessageRouterConfigurator
    {
        readonly IPipelineSink<IConsumeContext> _sink;

        internal InboundRequestMessageRouterConfigurator(IPipelineSink<IConsumeContext> sink)
        {
            _sink = sink;
        }

        public RequestMessageRouter<IConsumeContext<TMessage>, TMessage> FindOrCreate<TMessage>()
            where TMessage : class
        {
            var configurator = new InboundMessageRouterConfigurator(_sink);

            MessageRouter<IConsumeContext<TMessage>> router = configurator.FindOrCreate<TMessage>();

            var scope = new InboundRequestMessageRouterConfiguratorScope<TMessage>();
            _sink.Inspect(scope);

            return scope.Router ?? ConfigureRouter(router);
        }

        static RequestMessageRouter<IConsumeContext<TMessage>, TMessage> ConfigureRouter<TMessage>(
            MessageRouter<IConsumeContext<TMessage>> inputRouter)
            where TMessage : class
        {
            if (inputRouter == null)
                throw new PipelineException("The input router was not found");

            var outputRouter = new RequestMessageRouter<IConsumeContext<TMessage>, TMessage>(x => x.RequestId);

            inputRouter.Connect(outputRouter);

            return outputRouter;
        }
    }
}