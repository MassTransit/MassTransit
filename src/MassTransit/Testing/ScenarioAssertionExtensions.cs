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
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading;
	using Magnum.Extensions;
	using Pipeline;

	public static class ScenarioAssertionExtensions
	{
		public static IEnumerable<IPipelineSink<TMessage>> HasSubscription<TMessage>(this IServiceBus bus)
			where TMessage : class
		{
			return HasSubscription<TMessage>(bus, 8.Seconds());
		}

		public static IEnumerable<IPipelineSink<TMessage>> HasSubscription<TMessage>(this IServiceBus bus, TimeSpan timeout)
			where TMessage : class
		{
			DateTime giveUpAt = DateTime.Now + timeout;

			while (DateTime.Now < giveUpAt)
			{
				var inspector = new PipelineSinkLocator<TMessage>();

				bus.OutboundPipeline.Inspect(inspector);

				if (inspector.Result.Any())
					return inspector.Result;

				Thread.Sleep(10);
			}

			return Enumerable.Empty<IPipelineSink<TMessage>>();
		}
	}
}