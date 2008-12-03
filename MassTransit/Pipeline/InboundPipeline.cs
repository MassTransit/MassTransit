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

	public class InboundPipeline :
		PipelineBase<IInboundInterceptor>,
		IConfigureInboundPipeline
	{
		private readonly Func<bool> _emptyToken = () => false;

		public InboundPipeline(IObjectBuilder builder) :
			base(builder)
		{
			// interceptors are inserted at the front of the list, so do them from least to most specific

			_interceptors.Register(new ConsumesAllPipelineSubscriber());
			_interceptors.Register(new ConsumesSelectedPipelineSubscriber());
		}

		public V Configure<V>(Func<IConfigureInboundPipeline, V> action)
		{
			V result = action(this);

			return result;
		}

		public Func<bool> Subscribe<TComponent>()
			where TComponent : class
		{
			return Subscribe((context, interceptor) => interceptor.Subscribe<TComponent>(context));
		}

		public Func<bool> Subscribe<TComponent>(TComponent instance)
			where TComponent : class
		{
			return Subscribe((context, interceptor) => interceptor.Subscribe(context, instance));
		}

		private Func<bool> Subscribe(Func<IInboundContext, IInboundInterceptor, IEnumerable<Func<bool>>> subscriber)
		{
			var context = new InboundContext(this);

			Func<bool> result = null;

			_interceptors.ForEach(interceptor =>
				{
					foreach (Func<bool> token in subscriber(context, interceptor))
					{
						if (result == null)
							result = token;
						else
							result += token;
					}
				});

			return result ?? _emptyToken;
		}
	}
}