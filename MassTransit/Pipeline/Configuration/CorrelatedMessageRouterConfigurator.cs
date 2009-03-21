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
namespace MassTransit.Pipeline.Configuration
{
	using Exceptions;
	using Sinks;

	public class CorrelatedMessageRouterConfigurator
	{
		private readonly IPipelineSink<object> _sink;

		private CorrelatedMessageRouterConfigurator(IPipelineSink<object> sink)
		{
			_sink = sink;
		}

		public CorrelatedMessageRouter<TMessage, TKey> FindOrCreate<TMessage, TKey>()
			where TMessage : class, CorrelatedBy<TKey>
		{
			MessageRouterConfigurator configurator = MessageRouterConfigurator.For(_sink);

			var router = configurator.FindOrCreate<TMessage>();

			var scope = new CorrelatedMessageRouterConfiguratorScope<TMessage, TKey>();

			router.Inspect(scope);

			return scope.Router ?? ConfigureRouter<TMessage, TKey>(router);
		}

		private static CorrelatedMessageRouter<TMessage, TKey> ConfigureRouter<TMessage, TKey>(MessageRouter<TMessage> messageRouter)
			where TMessage : class, CorrelatedBy<TKey>
		{
			if (messageRouter == null)
				throw new PipelineException("The base object router was not found");

			CorrelatedMessageRouter<TMessage, TKey> router = new CorrelatedMessageRouter<TMessage, TKey>();

			messageRouter.Connect(router);

			return router;
		}

		public static CorrelatedMessageRouterConfigurator For<TMessage>(IPipelineSink<TMessage> sink)
			where TMessage : class
		{
			return new CorrelatedMessageRouterConfigurator(TranslateTo<IPipelineSink<object>>.From(sink));
		}
	}
}