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
	using Context;
	using Exceptions;
	using Sinks;

	public class InboundCorrelatedMessageRouterConfigurator
	{
		readonly IPipelineSink<IConsumeContext> _sink;

		internal InboundCorrelatedMessageRouterConfigurator(IPipelineSink<IConsumeContext> sink)
		{
			_sink = sink;
		}

		public CorrelatedMessageRouter<IConsumeContext<TMessage>, TMessage, TKey> FindOrCreate<TMessage, TKey>()
			where TMessage : class, CorrelatedBy<TKey>
		{
			var configurator = new InboundMessageRouterConfigurator(_sink);

			MessageRouter<IConsumeContext<TMessage>> router = configurator.FindOrCreate<TMessage>();

			var scope = new InboundCorrelatedMessageRouterConfiguratorScope<TMessage, TKey>();
			_sink.Inspect(scope);

			return scope.Router ?? ConfigureRouter<TMessage, TKey>(router);
		}

		static CorrelatedMessageRouter<IConsumeContext<TMessage>, TMessage, TKey> ConfigureRouter<TMessage, TKey>(
			MessageRouter<IConsumeContext<TMessage>> inputRouter)
			where TMessage : class, CorrelatedBy<TKey>
		{
			if (inputRouter == null)
				throw new PipelineException("The input router was not found");

			var outputRouter = new CorrelatedMessageRouter<IConsumeContext<TMessage>, TMessage, TKey>();

			inputRouter.Connect(outputRouter);

			return outputRouter;
		}
	}
}