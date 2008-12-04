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
			return Create(string.Empty, allow);
		}

		public MessageFilter<TMessage> Create<TMessage>(string description, Func<TMessage, bool> allow)
			where TMessage : class
		{
			EnsureRouterExists<TMessage>(_sink);

			var scope = new MessageFilterConfiguratorScope<TMessage>();
			_sink.Inspect(scope);

			return ConfigureFilter(description, scope.InsertAfter, allow);
		}

		private static MessageFilter<TMessage> ConfigureFilter<TMessage>(string description,
		                                                                 Func<IMessageSink<TMessage>, IMessageSink<TMessage>> insertAfter,
		                                                                 Func<TMessage, bool> allow)
			where TMessage : class
		{
			if (insertAfter == null)
				throw new PipelineException("Unable to insert filter into pipeline for message type " + typeof (TMessage).FullName);

			MessageFilter<TMessage> filter = new MessageFilter<TMessage>(description, insertAfter, allow);

			return filter;
		}

		private static void EnsureRouterExists<TMessage>(IMessageSink<object> sink) where TMessage : class
		{
			MessageRouterConfigurator routerConfigurator = MessageRouterConfigurator.For(sink);

			routerConfigurator.FindOrCreate<TMessage>();
		}

		public static MessageFilterConfigurator For(MessagePipeline sink)
		{
			return new MessageFilterConfigurator(sink);
		}
	}
}