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
	using Magnum.Threading;

	public abstract class MessageRouterBase<TMessage, TKey> :
		IPipelineSink<TMessage>
		where TMessage : class
	{
		private bool _disposed;
		protected ReaderWriterLockedObject<Dictionary<TKey, IPipelineSink<TMessage>>> _sinks;

		protected MessageRouterBase()
		{
			_sinks = new ReaderWriterLockedObject<Dictionary<TKey, IPipelineSink<TMessage>>>(new Dictionary<TKey, IPipelineSink<TMessage>>());
		}

		public abstract IEnumerable<Action<TMessage>> Enumerate(TMessage message);

		public virtual bool Inspect(IPipelineInspector inspector)
		{
			return inspector.Inspect(this, () =>
				{
					foreach (IPipelineSink<TMessage> sink in _sinks.ReadLock(x => x.Values))
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

		protected IEnumerable<Action<TMessage>> EnumerateSinks(TMessage message, TKey batchId)
		{
			IPipelineSink<TMessage> sink = null;

			if (_sinks.ReadLock(x => x.TryGetValue(batchId, out sink)) == false)
				yield break;

			if (sink == null)
				yield break;

			foreach (var consumer in sink.Enumerate(message))
			{
				yield return consumer;
			}
		}

		protected virtual void Dispose(bool disposing)
		{
			if (_disposed) return;
			if (disposing)
			{
				_sinks.Dispose();
				_sinks = null;
			}

			_disposed = true;
		}

		~MessageRouterBase()
		{
			Dispose(false);
		}
	}
}