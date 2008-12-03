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
	using System.Threading;
	using Magnum.Common.Threading;

	public abstract class MessageSinkBase<TInput, TOutput> :
		IMessageSink<TInput>
		where TInput : class
		where TOutput : class, TInput
	{
		private bool _disposed;
		protected ReaderWriterLockedObject<IMessageSink<TOutput>> _outputSink;

		protected MessageSinkBase(IMessageSink<TOutput> outputSink)
		{
			_outputSink = new ReaderWriterLockedObject<IMessageSink<TOutput>>(outputSink, LockRecursionPolicy.SupportsRecursion);
		}

		public abstract IEnumerable<Consumes<TInput>.All> Enumerate(TInput message);
		public abstract bool Inspect(IPipelineInspector inspector);

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (_disposed) return;
			if (disposing)
			{
				_outputSink.Dispose();
				_outputSink = null;
			}

			_disposed = true;
		}

		~MessageSinkBase()
		{
			Dispose(false);
		}
	}
}