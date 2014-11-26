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
namespace MassTransit.Pipeline
{
	using System;
	using System.IO;
	using Configuration;
	using Context;
	using MassTransit.Configuration;
	using Sinks;


    /// <summary>
	/// Extensions for the message pipeline.
	/// </summary>
	public static class MessagePipelineExtensions
	{
		/// <summary>
		/// Dispatch a message through the pipeline
		/// </summary>
		/// <param name="pipeline">The pipeline instance</param>
		/// <param name="message">The message to dispatch</param>
		public static bool Dispatch<T>(this IInboundMessagePipeline pipeline, T message)
			where T : class
		{
			return pipeline.Dispatch(message, x => true);
		}


		/// <summary>
		/// Dispatch a message through the pipeline. If the message will be consumed, the accept function
		/// is called to allow the endpoint to acknowledge the message reception if applicable
		/// </summary>
		/// <param name="pipeline">The pipeline instance</param>
		/// <param name="message">The message to dispatch</param>
		/// <param name="acknowledge">The function to call if the message will be consumed by the pipeline</param>
		/// <returns>whether the message was consumed</returns>
		public static bool Dispatch<T>(this IInboundMessagePipeline pipeline, T message, Func<T, bool> acknowledge)
			where T : class
		{
			bool consumed = false;

			using (var bodyStream = new MemoryStream())
			{
				OldReceiveContext receiveContext = OldReceiveContext.FromBodyStream(bodyStream);
				pipeline.Configure(x => receiveContext.SetBus(x.Bus));
				var context = new OldConsumeContext<T>(receiveContext, message);

				using (context.CreateScope())
				{
					foreach (var consumer in pipeline.Enumerate(context))
					{
						if (!acknowledge(message))
							return false;

						acknowledge = x => true;

						consumed = true;

						consumer(context);
					}
				}
			}

			return consumed;
		}


		/// <summary>
		/// <see cref="Dispatch{T}(MassTransit.Pipeline.IInboundMessagePipeline,T)"/>: this one is for the outbound pipeline.
		/// </summary>
		public static bool Dispatch<T>(this IOutboundMessagePipeline pipeline, T message, Func<T, bool> acknowledge)
			where T : class
		{
			bool consumed = false;

			var context = new OldSendContext<T>(message);

			foreach (var consumer in pipeline.Enumerate(context))
			{
				if (!acknowledge(message))
					return false;

				acknowledge = x => true;

				consumed = true;

				consumer(context);
			}

			return consumed;
		}
	}
}