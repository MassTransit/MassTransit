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
	using Exceptions;
	using Sinks;

	public class MessageFilterConfigurator
	{
		private readonly IMessageSink<object> _sink;

		private MessageFilterConfigurator(IMessageSink<object> sink)
		{
			_sink = sink;
		}

		public MessageFilter<TMessage> Create<TMessage>(Func<TMessage, bool> allow)
			where TMessage : class
		{
			// we pull the router just to make sure it exists
			MessageRouterConfigurator routerConfigurator = MessageRouterConfigurator.For(_sink);

			var router = routerConfigurator.FindOrCreate<TMessage>();

			var scope = new MessageFilterConfiguratorScope<TMessage>();

			_sink.Inspect(scope);

			return ConfigureFilter(scope.InsertAfter, allow);
		}

		private static MessageFilter<TMessage> ConfigureFilter<TMessage>(Func<IMessageSink<TMessage>, IMessageSink<TMessage>> insertAfter,
		                                                                 Func<TMessage, bool> allow)
			where TMessage : class
		{
			if (insertAfter == null)
				throw new PipelineException("Unable to insert filter into pipeline for message type " + typeof (TMessage).FullName);

			MessageFilter<TMessage> filter = new MessageFilter<TMessage>(insertAfter, allow);

			return filter;
		}

		public static MessageFilterConfigurator For(MessagePipeline sink)
		{
			return new MessageFilterConfigurator(sink);
		}
	}
}