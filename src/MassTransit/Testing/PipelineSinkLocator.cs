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
namespace MassTransit.Testing
{
	using System.Collections.Generic;
	using System.Linq;
	using Pipeline;
	using Pipeline.Inspectors;
	using Pipeline.Sinks;

	public class PipelineSinkLocator<T> :
		PipelineInspectorBase<PipelineSinkLocator<T>>
		where T : class
	{
		public IEnumerable<IPipelineSink<T>> Result { get; private set; }

		public PipelineSinkLocator()
		{
			Result = Enumerable.Empty<IPipelineSink<T>>();
		}

		public bool Inspect<TComponent, TMessage>(ComponentMessageSink<TComponent, TMessage> sink)
			where TComponent : class, Consumes<TMessage>.All
			where TMessage : class
		{
			if (typeof (TMessage) == typeof (T))
			{
				Result = Result.Concat(new[]{sink as IPipelineSink<T>});

				return false;
			}

			return true;
		}

		public bool Inspect<TComponent, TMessage>(SelectedComponentMessageSink<TComponent, TMessage> sink)
			where TComponent : class, Consumes<TMessage>.Selected
			where TMessage : class
		{
			if (typeof (TMessage) == typeof (T))
			{
				Result = Result.Concat(new[] { sink as IPipelineSink<T> });

				return false;
			}

			return true;
		}

		public bool Inspect<TMessage>(InstanceMessageSink<TMessage> sink)
			where TMessage : class
		{
			if (typeof (TMessage) == typeof (T))
			{
				Result = Result.Concat(new[] { sink as IPipelineSink<T> });

				return false;
			}

			return true;
		}

		public bool Inspect<TMessage>(EndpointMessageSink<TMessage> sink)
			where TMessage : class
		{
			if (typeof(TMessage) == typeof(T))
			{
				Result = Result.Concat(new[] { sink as IPipelineSink<T> });

				return false;
			}

			return true;
		}
	}
}