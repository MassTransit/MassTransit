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
namespace MassTransit
{
	using Context;
	using Pipeline;
	using Pipeline.Configuration;

	public static class MessageRouterConfiguratorExtensions
	{
		public static UnsubscribeAction Connect<TOutput>(this IInboundMessagePipeline pipeline,
		                                                 IPipelineSink<IConsumeContext<TOutput>> sink)
			where TOutput : class
		{
			var routerConfigurator = new InboundMessageRouterConfigurator(pipeline);

			return routerConfigurator.FindOrCreate<TOutput>().Connect(sink);
		}

		public static UnsubscribeAction Connect<TOutput>(this IOutboundMessagePipeline pipeline,
		                                                 IPipelineSink<IBusPublishContext<TOutput>> sink)
			where TOutput : class
		{
			var routerConfigurator = new OutboundMessageRouterConfigurator(pipeline);

			return routerConfigurator.FindOrCreate<TOutput>().Connect(sink);
		}
	}
}