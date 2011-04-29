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
namespace MassTransit.Pipeline.Sinks
{
	using System;
	using System.Collections.Generic;
	using Magnum.Threading;

	/// <summary>
	/// Routes a message to all of the connected message sinks without modification
	/// </summary>
	/// <typeparam name="TMessage">The type of the message to be routed</typeparam>
	public class MessageRouter<TMessage> :
		IPipelineSink<TMessage>
		where TMessage : class
	{
		readonly ReaderWriterLockedObject<List<IPipelineSink<TMessage>>> _sinks;
		bool _disposed;

		public MessageRouter()
		{
			_sinks = new ReaderWriterLockedObject<List<IPipelineSink<TMessage>>>(new List<IPipelineSink<TMessage>>());
		}

		public int SinkCount
		{
			get { return _sinks.ReadLock(x => x.Count); }
		}

		public IEnumerable<Action<TMessage>> Enumerate(TMessage message)
		{
			foreach (var sink in _sinks.ReadLock(x => x.ToArray()))
			{
				foreach (var consumer in sink.Enumerate(message))
				{
					yield return consumer;
				}
			}
		}

		public bool Inspect(IPipelineInspector inspector)
		{
			return inspector.Inspect(this, () =>
				{
					foreach (var sink in _sinks.ReadLock(x => x.ToArray()))
					{
						if (sink.Inspect(inspector) == false)
							return false;
					}

					return true;
				});
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Connects a message sink to the router
		/// </summary>
		/// <param name="sink">The sink to be connected</param>
		/// <returns>A function to disconnect the sink from the router</returns>
		public UnsubscribeAction Connect(IPipelineSink<TMessage> sink)
		{
			_sinks.WriteLock(sinks => sinks.Add(sink));

			return () => _sinks.WriteLock(sinks => sinks.Remove(sink));
		}

		void Dispose(bool disposing)
		{
			if (_disposed) return;
			if (disposing)
			{
				_sinks.Dispose();
			}

			_disposed = true;
		}

		~MessageRouter()
		{
			Dispose(false);
		}
	}
}