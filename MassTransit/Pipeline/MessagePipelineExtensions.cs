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
namespace MassTransit.Pipeline
{
	using System;
	using Configuration;
	using Sinks;

	public static class MessagePipelineExtensions
	{
		public static Func<bool> Filter<TMessage>(this InboundPipeline pipeline, Func<TMessage, bool> allow) 
			where TMessage : class
		{
		    return Filter<TMessage>(pipeline, "", allow);
		}

        public static Func<bool> Filter<TMessage>(this InboundPipeline pipeline, string description, Func<TMessage, bool> allow)
            where TMessage : class
        {
            return pipeline.Configure(x =>
            {
                MessageFilterConfigurator configurator = MessageFilterConfigurator.For(pipeline);

                var filter = configurator.Create(description, allow);

                Func<bool> result = () => false;

                return result;
            });
        }

		public static Func<bool> SubscribeEndpoint<TMessage>(this InboundPipeline pipeline, IEndpoint endpoint) where TMessage : class
		{
			MessagePipeline messagePipeline = pipeline;

			return pipeline.Configure(x =>
				{
					MessageRouterConfigurator routerConfigurator = MessageRouterConfigurator.For(messagePipeline);

					return routerConfigurator.FindOrCreate<TMessage>().Connect(new EndpointMessageSink<TMessage>(endpoint));
				});
		}
	}
}