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
namespace MassTransit.Pipeline.Configuration
{
    using Inspectors;
	using Sinks;
	using Util;

	public class InboundMessageRouterConfiguratorScope<TOutput> :
		PipelineInspectorBase<InboundMessageRouterConfiguratorScope<TOutput>>
		where TOutput : class
	{
		public MessageRouter<IConsumeContext> InputRouter { get; private set; }
		public MessageRouter<TOutput> OutputRouter { get; private set; }

		[UsedImplicitly]
		protected bool Inspect<T>(MessageRouter<T> router)
			where T : class
		{
			if (typeof (T) == typeof (TOutput))
			{
				OutputRouter = router.TranslateTo<MessageRouter<TOutput>>();

				return false;
			}

			if (typeof (T) == typeof (IConsumeContext))
			{
				InputRouter = router.TranslateTo<MessageRouter<IConsumeContext>>();
			}

			return true;
		}
	}
}