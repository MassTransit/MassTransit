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
	using System;
	using Sinks;

	public static class ExtensionsForPipelineConfiguration
	{
		public static UnsubscribeAction ConnectToRouter<T>(this IMessagePipeline pipeline, IPipelineSink<T> sink)
			where T : class
		{
			MessageRouterConfigurator routerConfigurator = MessageRouterConfigurator.For(pipeline);

			MessageRouter<T> router = routerConfigurator.FindOrCreate<T>();

			UnsubscribeAction result = router.Connect(sink);

			return () => result() && (router.SinkCount == 0);
		}

		private static UnsubscribeAction ConnectToRouter<T, V>(this IPipelineSink<V> pipeline, IPipelineSink<T> sink)
			where T : class
			where V : class
		{
			MessageRouterConfigurator routerConfigurator = MessageRouterConfigurator.For(pipeline);

			MessageRouter<T> router = routerConfigurator.FindOrCreate<T>();

			UnsubscribeAction result = router.Connect(sink);

			return () => result() && (router.SinkCount == 0);
		}

		public static UnsubscribeAction ConnectToRouter<T>(this IMessagePipeline pipeline, IPipelineSink<T> sink, Func<UnsubscribeAction> subscribedTo)
			where T : class
		{
			UnsubscribeAction result = pipeline.ConnectToRouter(sink);

			UnsubscribeAction remove = subscribedTo();

			return () => result() && remove();
		}

		public static UnsubscribeAction ConnectToRouter<T>(this IPipelineSink<T> pipeline, IPipelineSink<T> sink, Func<bool> unsubscribe)
			where T : class
		{
			UnsubscribeAction result = pipeline.ConnectToRouter(sink);

			return () => result() && unsubscribe();
		}
	}
}