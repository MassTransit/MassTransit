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
	using System.Linq;
	using Magnum.Concurrency;

	/// <summary>
	/// Routes a message to all of the connected message sinks without modification
	/// </summary>
	public class MessageRouter<T> :
		IPipelineSink<T>
		where T : class
	{
		readonly Atomic<List<IPipelineSink<T>>> _output;

		public MessageRouter()
		{
			_output = Atomic.Create(new List<IPipelineSink<T>>());
		}

		public MessageRouter(IEnumerable<IPipelineSink<T>> sinks)
		{
			_output = Atomic.Create(new List<IPipelineSink<T>>(sinks));
		}

		public int SinkCount
		{
			get { return _output.Value.Count; }
		}

		public IList<IPipelineSink<T>> Sinks
		{
			get { return _output.Value; }
		}

		public IEnumerable<Action<T>> Enumerate(T context)
		{
			return _output.Value.SelectMany(x => x.Enumerate(context));
		}

		public bool Inspect(IPipelineInspector inspector)
		{
			return inspector.Inspect(this, () => _output.Value.All(x => x.Inspect(inspector)));
		}

		public UnsubscribeAction Connect(IPipelineSink<T> sink)
		{
			_output.Set(sinks => new List<IPipelineSink<T>>(sinks) {sink});

			return () => _output.Set(sinks => sinks.Where(x => x != sink).ToList()) != null;
		}
	}
}