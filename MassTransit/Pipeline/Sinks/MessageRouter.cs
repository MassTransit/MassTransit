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
namespace MassTransit.Pipeline.Sinks
{
	using System;
	using System.Collections.Generic;
	using Magnum.Common.Threading;

	/// <summary>
	/// Routes a message to all of the connected message sinks without modification
	/// </summary>
	/// <typeparam name="TMessage">The type of the message to be routed</typeparam>
	public class MessageRouter<TMessage> :
		IMessageSink<TMessage>
		where TMessage : class
	{
		private readonly ReaderWriterLockedObject<List<IMessageSink<TMessage>>> _sinks;

		public MessageRouter()
		{
			_sinks = new ReaderWriterLockedObject<List<IMessageSink<TMessage>>>(new List<IMessageSink<TMessage>>());
		}

		public IEnumerable<Consumes<TMessage>.All> Enumerate(TMessage message)
		{
			foreach (var sink in _sinks.ReadLock(x => x.ToArray()))
			{
				foreach (Consumes<TMessage>.All consumer in sink.Enumerate(message))
				{
					yield return consumer;
				}
			}
		}

		public bool Inspect(IPipelineInspector inspector)
		{
			return inspector.Inspect(this, () =>
				{
					foreach (IMessageSink<TMessage> sink in _sinks.ReadLock(x => x.ToArray()))
					{
						if (sink.Inspect(inspector) == false)
							return false;
					}

					return true;
				});
		}

		public int SinkCount
		{
			get { return _sinks.ReadLock(x => x.Count); }
		}

		/// <summary>
		/// Connects a message sink to the router
		/// </summary>
		/// <param name="sink">The sink to be connected</param>
		/// <returns>A function to disconnect the sink from the router</returns>
		public Func<bool> Connect(IMessageSink<TMessage> sink)
		{
			_sinks.WriteLock(sinks => sinks.Add(sink));

			return () => _sinks.WriteLock(sinks => sinks.Remove(sink));
		}

		public void Dispose()
		{
			_sinks.Dispose();
		}
	}
}