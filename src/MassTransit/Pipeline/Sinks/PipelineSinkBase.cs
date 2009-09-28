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

	public abstract class PipelineSinkBase<TInput, TOutput> :
		IPipelineSink<TInput>
		where TInput : class
		where TOutput : class, TInput
	{
		private bool _disposed;
		protected IPipelineSink<TOutput> _outputSink;

		protected PipelineSinkBase(IPipelineSink<TOutput> outputSink)
		{
			_outputSink = outputSink;
		}

		public abstract IEnumerable<Action<TInput>> Enumerate(TInput message);
		public abstract bool Inspect(IPipelineInspector inspector);

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public IPipelineSink<TOutput> ReplaceOutputSink(IPipelineSink<TOutput> sink)
		{
			IPipelineSink<TOutput> result = _outputSink;

			_outputSink = sink;

			return result;
		}

		protected virtual void Dispose(bool disposing)
		{
			if (_disposed) return;
			if (disposing)
			{
				if (_outputSink != null)
				{
					_outputSink.Dispose();
					_outputSink = null;
				}
			}

			_disposed = true;
		}

		~PipelineSinkBase()
		{
			Dispose(false);
		}
	}
}