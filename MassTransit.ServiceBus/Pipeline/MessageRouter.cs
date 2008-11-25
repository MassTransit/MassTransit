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
	using System.Collections.Generic;

	/// <summary>
	/// Routes a message to all of the connected message sinks without modification
	/// </summary>
	/// <typeparam name="TMessage">The type of the message to be routed</typeparam>
	public class MessageRouter<TMessage> :
		IMessageSink<TMessage>
		where TMessage : class
	{
		private readonly List<IMessageSink<TMessage>> _sinks = new List<IMessageSink<TMessage>>();

		public IEnumerable<Consumes<TMessage>.All> Enumerate(TMessage message)
		{
			foreach (IMessageSink<TMessage> sink in _sinks)
			{
				foreach (Consumes<TMessage>.All consumer in sink.Enumerate(message))
				{
					yield return consumer;
				}
			}
		}

		public bool Inspect(IPipelineInspector inspector)
		{
			inspector.Inspect(this);

			foreach (IMessageSink<TMessage> sink in _sinks)
			{
				if (sink.Inspect(inspector) == false)
					return false;
			}

			return true;
		}

		/// <summary>
		/// Connects a message sink to the router
		/// </summary>
		/// <param name="sink">The sink to be connected</param>
		/// <returns>A function to disconnect the sink from the router</returns>
		public Func<bool> Connect(IMessageSink<TMessage> sink)
		{
			_sinks.Add(sink);

			return () => _sinks.Remove(sink);
		}
	}
}