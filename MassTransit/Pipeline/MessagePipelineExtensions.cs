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

	public static class MessagePipelineExtensions
	{
		/// <summary>
		/// Subscribe an indiscriminant consumer to the pipeline
		/// </summary>
		/// <typeparam name="TMessage"></typeparam>
		/// <param name="pipeline">The pipeline instance</param>
		/// <param name="consumer">The consumer for the message</param>
		/// <returns>Function to unsubscribe when complete</returns>
		public static Func<bool> Subscribe<TMessage>(this MessagePipeline pipeline, Consumes<TMessage>.All consumer)
			where TMessage : class
		{
			return pipeline.Configure(x =>
				{
					var sink = new MessageSink<TMessage>(message => consumer);

					return ConfigureMessageRouter<TMessage>.Connect(x, sink);
				});
		}

		public static Func<bool> Subscribe<TMessage>(this MessagePipeline pipeline, Consumes<TMessage>.Selected consumer)
			where TMessage : class
		{
			return pipeline.Configure(x =>
				{
					var sink = new MessageSink<TMessage>(message =>
					                                     consumer.Accept(message) ? consumer : Consumes<TMessage>.Null);

					return ConfigureMessageRouter<TMessage>.Connect(x, sink);
				});
		}

		public static Func<bool> Subscribe<TComponent>(this MessagePipeline pipeline)
		{
			return pipeline.Configure(x => ConfigureComponent<TComponent>.Subscribe(x));
		}
	}
}