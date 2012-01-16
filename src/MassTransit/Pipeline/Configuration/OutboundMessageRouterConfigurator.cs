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

	public class OutboundMessageRouterConfigurator
	{
		readonly IPipelineSink<ISendContext> _sink;

		internal OutboundMessageRouterConfigurator(IPipelineSink<ISendContext> sink)
		{
			_sink = sink;
		}

		public MessageRouter<IBusPublishContext<TOutput>> FindOrCreate<TOutput>()
			where TOutput : class
		{
			var scope = new OutboundMessageRouterConfiguratorScope<IBusPublishContext<TOutput>>();
			_sink.Inspect(scope);

			return scope.OutputRouter ?? ConfigureRouter<TOutput>(scope.InputRouter);
		}

		static MessageRouter<IBusPublishContext<TOutput>> ConfigureRouter<TOutput>(MessageRouter<ISendContext> inputRouter)
			where TOutput : class
		{
			if (inputRouter == null)
				throw new PipelineException("The input router was not found");

			var router = new MessageRouter<IBusPublishContext<TOutput>>();

			var translator = new OutboundConvertMessageSink<TOutput>(router);

			inputRouter.Connect(translator);

			return router;
		}
	}
}